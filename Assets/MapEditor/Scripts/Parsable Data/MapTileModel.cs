using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MapTileModel
{
	public string TerrainId;

	public TerrainModel Terrain()
	{
		return TerrainParser.TerrainData[TerrainId];
	}
	public string KingdomId;
	public List<EntityPlacementModel> Entities = new List<EntityPlacementModel>();
	public SettlementTextModel TextEntry;
	public PortModel Port;

	public List<EntityPlacementModel> GetEntites()
	{
		if (Entities.Count > 0)
			return Entities;
		else return Terrain().EntityPlacements;
	}

	public enum TileTraits
	{
		Water,
		Impassable,
		Mountain,
		River,
		Ocean,
		Settled,
		Port,
		Road,
	}

	public List<string> Traits = new List<string>();
	public List<string> Overlays = new List<string>();
	public float Height { get { return Mathf.Min(Terrain().Height, MaxHeight); } }
	public float MaxHeight = 1000;
	public float Difficulty { get { return Mathf.Min(Terrain().Difficulty, MaxDifficulty); } }
	public float MaxDifficulty = 1000;

	public void SetMaxHeight(float newMaxHeight)
	{
		MaxHeight = newMaxHeight;
	}

	public List<string> GetOverlays()
	{
		return Terrain().Overlays.Concat(Overlays).ToList();
	}

	public List<string> GetTraits()
	{
		return Traits.Concat(Terrain().Traits).ToList();
	}

	public bool HasTrait(TileTraits trait)
	{
		return GetTraits().Contains(trait.ToString());
	}

	public bool HasTrait(string trait)
	{
		return GetTraits().Contains(trait);
	}
}

[Serializable]
public class SettlementTextModel
{
	public string Text;
	public string SettlementDescription;
	public string KingdomName;
	public bool Capitol;
	public HeraldryModel SettlementHeraldry;
	public HeraldryModel KingdomHeraldry;
}

[Serializable]
public class HeraldryModel
{
	public StoredTexture BackgroundTexture;
	public Vector4 BackgroundColor1;
	public Vector4 BackgroundColor2;
	public StoredTexture ForegroundTexture;
	public Vector4 ForegroundColor;
	public StoredTexture OverlayTexture;

	public string GetKey()
	{
		return "" + BackgroundColor1 + BackgroundColor2 + ForegroundColor +
		       BackgroundTexture + ForegroundTexture;
	}
}

[Serializable]
public class PortModel
{
	public List<List<Int2>> SeaLanes = new List<List<Int2>>();

	public List<Int2> GetSeaLane()
	{
		return SeaLanes[UnityEngine.Random.Range(0, SeaLanes.Count)];
	}
}