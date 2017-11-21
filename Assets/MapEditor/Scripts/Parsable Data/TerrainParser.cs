using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

public static class TerrainParser
{
	public static Dictionary<string, TerrainModel> TerrainData = new Dictionary<string, TerrainModel>();

	public static void LoadTerrainTypes()
	{
		TerrainData = ParserHelpers.ParseTypes<TerrainModel>("terrain");

		TerrainData["Ocean"] = new TerrainModel()
		{
			Difficulty = .05f,
			Texture = new StoredTexture()
			{
				TexturePath = "GenericOcean.png"
			},
			Height = 0f,
			Id = "Ocean",
			Traits = new List<string>()
			{
				"Water",
				"Impassable",
				"Ocean"
			},
			Overlays = new List<string>()
			{
				"RiverWater",
				"RiverBanks",
				"OceanWater",
				"OceanShore"
			}
		};
	}
}

[Serializable]
public class TerrainModel : ParsableData
{
	public static float MinGroundHeight()
	{
		return 0.1f;
	}

	private Dictionary<string, float> Defense = new Dictionary<string, float>()
	{
		{MapTileModel.TileTraits.Water.ToString(), .3f },
		{MapTileModel.TileTraits.Impassable.ToString(), .2f },
		{MapTileModel.TileTraits.Fertile.ToString(), -.5f },
		{MapTileModel.TileTraits.Forest.ToString(), -.3f },
	};

	
	public float Height;
	public StoredTexture Texture;
	public float Difficulty = 0.1f;
	public List<string> Traits = new List<string>();
	public List<EntityPlacementModel> EntityPlacements = new List<EntityPlacementModel>();
	public List<string> Overlays = new List<string>();

	public float GetDefensibility()
	{
		float defense = 0;
		foreach (string trait in Traits)
		{
			if (Defense.ContainsKey(trait))
				defense += Defense[trait];
		}
		return defense;
	}
}

[Serializable]
public class StoredTexture
{
	public static Dictionary<string, Map2D<Color>> ParsedTextures = new Dictionary<string, Map2D<Color>>();

	public string TexturePath;

	public Map2D<Color> GetTexture()
	{
		if (!ParsedTextures.ContainsKey(TexturePath))
		{
			if(TexturePath == null)
				Debug.Log("HIT!");

			Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + TexturePath);
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(file);
			tex.Apply();
			OverlayDisplayHandler.ColorMap colorMap = new OverlayDisplayHandler.ColorMap(1, 1, tex.width);
			colorMap.SetPixels(0, 0, tex.GetPixels());
			ParsedTextures[TexturePath] = colorMap.Colors;
		}

		return ParsedTextures[TexturePath];
	}
}