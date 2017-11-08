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
	}
}

[Serializable]
public class TerrainModel : ParsableData
{
	public enum GroundTraits
	{
		Water,
		Impassable,
		Mountain,
		Fertile,
		Swamp,
		Forest,
	}

	private Dictionary<string, float> Defense = new Dictionary<string, float>()
	{
		{GroundTraits.Water.ToString(), .3f },
		{GroundTraits.Impassable.ToString(), .2f },
		{GroundTraits.Fertile.ToString(), -.5f },
		{GroundTraits.Forest.ToString(), -.3f },
	};

	public string Texture;
	public float Difficulty = 0.1f;
	public string[] StoredTraits = new string[0];
	public Color LookupColor;
	public EntityPlacementModel[] EntityPlacements = new EntityPlacementModel[0];

	public bool HasTrait(GroundTraits trait)
	{
		return StoredTraits.Contains(trait.ToString());
	}

	public float GetDefensibility()
	{
		float defense = 0;
		foreach (string trait in StoredTraits)
		{
			if (Defense.ContainsKey(trait))
				defense += Defense[trait];
		}
		return defense;
	}

	public Texture2D GetTerrainTexture()
	{
		Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + Texture);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(file);
		return tex;
	}
}