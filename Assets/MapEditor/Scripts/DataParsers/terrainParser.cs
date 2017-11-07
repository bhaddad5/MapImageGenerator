using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class TerrainParser
{
	public static Dictionary<string, TerrainInfo> TerrainData = new Dictionary<string, TerrainInfo>();

	public static void LoadTerrainTypes()
	{
		var terrains = ParserHelpers.ParseTypes<StoredTerrain>("terrain");
		foreach (StoredTerrain terrain in terrains.Values)
		{
			TerrainData[terrain.Id] = terrain.ToDisplayInfo();
		}
	}
}

[Serializable]
public class StoredTerrain : ParsableData
{
	public string Texture;
	public float Difficulty = 0.1f;
	public string[] StoredTraits = new string[0];
	public StoredModelPlacementInfo[] DefaultModelPlacement = new StoredModelPlacementInfo[0];

	public TerrainInfo ToDisplayInfo()
	{
		var gdi = new TerrainInfo();
		gdi.lookupColor = Helpers.RandomColor();

		Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + Texture);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(file);
		gdi.texture = tex;
		gdi.groundType = Id;
		gdi.difficulty = Difficulty;

		gdi.traits = new List<TerrainInfo.GroundTraits>();
		foreach (string trait in StoredTraits)
		{
			gdi.traits.Add((TerrainInfo.GroundTraits)System.Enum.Parse(typeof(TerrainInfo.GroundTraits), trait));
		}

		gdi.placementInfos = new List<ModelPlacementInfo>();
		foreach (StoredModelPlacementInfo info in DefaultModelPlacement)
		{
			gdi.placementInfos.Add(new ModelPlacementInfo(info.model, info.placementMode, info.min, info.max));
		}

		return gdi;
	}
}

public class TerrainInfo
{
	public enum GroundTraits
	{
		Water,
		Impassable,
		Rocky,
		Fertile,
		Muddy,
		Hunting,
		City,
		SmallCity,
		MediumCity,
		LargeCity,
		Road,
		Forest
	}

	private Dictionary<TerrainInfo.GroundTraits, float> Defense = new Dictionary<GroundTraits, float>()
	{
		{GroundTraits.Water, .3f },
		{GroundTraits.Impassable, .2f },
		{GroundTraits.Rocky, .5f },
		{GroundTraits.Fertile, -.5f },
		{GroundTraits.Muddy, .5f },
		{GroundTraits.Hunting, -.3f },
		{GroundTraits.Forest, -.3f },
		{GroundTraits.City, 0 },
		{GroundTraits.Road, -1f },
	};

	public string groundType;
	public Color lookupColor;
	public Texture2D texture;
	public List<GroundTraits> traits;
	public List<ModelPlacementInfo> placementInfos;
	public float difficulty;

	public bool HasTrait(GroundTraits trait)
	{
		return traits.Contains(trait);
	}

	public float GetDefensibility()
	{
		float defense = 0;
		foreach (GroundTraits trait in traits)
		{
			if (Defense.ContainsKey(trait))
				defense += Defense[trait];
		}
		return defense;
	}
}