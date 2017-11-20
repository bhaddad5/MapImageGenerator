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
		Fertile,
		Swamp,
		Forest,
		River,
		Desert,
		Ocean,
		Settled,
	}

	public List<string> Traits = new List<string>();
	public List<string> Overlays = new List<string>();
	public float Height { get { return Mathf.Min(Terrain().Height, maxHeight); } }
	private float maxHeight = 1000;

	public void SetMaxHeight(float newMaxHeight)
	{
		maxHeight = newMaxHeight;
	}

	public List<string> GetOverlays()
	{
		return Terrain().Overlays.Concat(Overlays).ToList();
	}

	public bool HasTrait(TileTraits trait)
	{
		return Traits.Contains(trait.ToString()) || Terrain().Traits.Contains(trait.ToString());
	}

	public bool HasTrait(string trait)
	{
		return Traits.Contains(trait) || Terrain().Traits.Contains(trait);
	}
}