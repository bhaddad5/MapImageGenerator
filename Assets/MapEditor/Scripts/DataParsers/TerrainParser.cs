using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			Texture = "GenericOcean.png",
			Height = 0f,
			Id = "Ocean",
			LookupColor = new Color(0, 0, .8f, 0),
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

	public string Texture;
	public float Height;
	public float Difficulty = 0.1f;
	public List<string> Traits = new List<string>();
	public Color LookupColor;
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

	private OverlayDisplayHandler.ColorMap texture = null;

	public OverlayDisplayHandler.ColorMap GetTerrainTexture()
	{
		if (texture == null)
		{
			Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + Texture);
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(file);
			tex.Apply();
			texture = new OverlayDisplayHandler.ColorMap(1, 1, tex.width);
			texture.SetPixels(0, 0, tex.GetPixels());
		}
		
		return texture;
	}
}