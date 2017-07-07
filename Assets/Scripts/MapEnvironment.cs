using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacementInfo
{
	public enum PlacementMode
	{
		Scattered,
		ScatteredBordered,
		Edges,
		Corners,
	}

	public PlacementMode Mode;
	public GameObject Model;
	private int min;
	private int max;
	public int NumToPlace { get { return UnityEngine.Random.Range(min, max + 1); } }

	public ModelPlacementInfo(string model, string mode, int mi, int ma)
	{
		min = mi;
		max = ma;
		Model = EnvironmentParser.modelLookup.LookupModel(model);
		Mode = (PlacementMode)System.Enum.Parse(typeof(PlacementMode), mode);
	}
}

public class GroundInfo
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
		Road
	}

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
}

public class MapEnvironment
{
	public GroundInfo Ocean
	{
		get { return groundTypes["Ocean"]; }
	}

	public GroundInfo River
	{
		get { return groundTypes["River"]; }
	}

	public GroundInfo City
	{
		get { return GetFirstWithTrait(GroundInfo.GroundTraits.City); }
	}

	public GroundInfo Road
	{
		get { return GetFirstWithTrait(GroundInfo.GroundTraits.Road); }
	}

	public string displayName;
	public Dictionary<string, GroundInfo> groundTypes = new Dictionary<string, GroundInfo>();
	public IMapGenerator HeightGenerator;

	public GroundInfo GetGround(string type)
	{
		return groundTypes[type];
	}

	public GroundInfo GetFirstWithTrait(GroundInfo.GroundTraits trait)
	{
		foreach (var groundType in groundTypes)
		{
			if (groundType.Value.traits.Contains(trait))
				return groundType.Value;
		}
		return null;
	}
}