using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Settlement
{
	public enum CityTrait
	{
		Mountains,
		Port,
		Landlocked,
		Small,
		Medium,
		Large,
		Forest,
		Fertile,
		River
	}

	public string name = "PLACEHOLDER";
	public SortedDupList<Settlement> adjacentSettlements = new SortedDupList<Settlement>();

	public List<Int2> cityTiles = new List<Int2>();
	public Kingdom kingdom;
	public Settlement(Int2 cityTile, Kingdom k)
	{
		cityTiles.Add(cityTile);
		kingdom = k;
	}

	public void ExpandSettlement(float regionValue)
	{
		MapGenerator.Terrain.Set(cityTiles[0], MapGenerator.Environment.City);

		float valuePerNewTile = 10;
		while (cityTiles.Count < regionValue / valuePerNewTile)
		{
			var expansionTiles = GetPossibleExpnasionTiles();
			if (expansionTiles.Count == 0)
				break;
			cityTiles.Add(expansionTiles.TopValue());
			MapGenerator.Terrain.Set(expansionTiles.TopValue(), MapGenerator.Environment.City);
		}
	}

	private SortedDupList<Int2> GetPossibleExpnasionTiles()
	{
		SortedDupList<Int2> possibleExpansions = new SortedDupList<Int2>();
		foreach (Int2 cityTile in cityTiles)
		{
			foreach (Int2 neighbor in MapGenerator.Terrain.GetAdjacentPoints(cityTile))
			{
				var neighborType = MapGenerator.Terrain.Get(neighbor);
				if (!possibleExpansions.ContainsValue(neighbor) &&
					!neighborType.HasTrait(GroundInfo.GroundTraits.City) &&
					!neighborType.HasTrait(GroundInfo.GroundTraits.Impassable) &&
					RegionsGen.Map.Get(neighbor).settlement == this &&
					!BordersUnfriendlyCity(neighbor))
				{
					possibleExpansions.Insert(neighbor, kingdom.culture.TileAreaValue(neighbor, true));
				}
			}
		}
		return possibleExpansions;
	}

	private bool BordersUnfriendlyCity(Int2 tile)
	{
		bool bordersUnfriendlyCity = false;
		foreach(var border in MapGenerator.Terrain.GetAllNeighboringPoints(tile))
		{
			if(MapGenerator.Terrain.Get(border).HasTrait(GroundInfo.GroundTraits.City))
			{
				if(!cityTiles.Contains(border))
					bordersUnfriendlyCity = true;
			}
		}
		return bordersUnfriendlyCity;
	}

	public List<Settlement.CityTrait> GetCityTraits()
	{
		List<GroundInfo> neighboringTerrainTypes = new List<GroundInfo>();
		foreach (Int2 tile in cityTiles)
		{
			foreach (var neighbor in MapGenerator.Terrain.GetAdjacentValues(tile))
			{
				neighboringTerrainTypes.Add(neighbor);
			}
		}

		List<Settlement.CityTrait> traits = new List<CityTrait>();
		if (neighboringTerrainTypes.Contains(MapGenerator.Environment.GetGround("Mountain")))
			traits.Add(CityTrait.Mountains);
		if (neighboringTerrainTypes.Contains(MapGenerator.Environment.GetGround("Forest")))
			traits.Add(CityTrait.Forest);
		if (neighboringTerrainTypes.Contains(MapGenerator.Environment.GetGround("Fertile")))
			traits.Add(CityTrait.Fertile);


		if (neighboringTerrainTypes.Contains(MapGenerator.Environment.Ocean))
			traits.Add(CityTrait.Port);
		else if (neighboringTerrainTypes.Contains(MapGenerator.Environment.River))
			traits.Add(CityTrait.River);
		else traits.Add(CityTrait.Landlocked);

		traits.Add(GetSettlementSize());

		return traits;
	}

	public CityTrait GetSettlementSize()
	{
		if (cityTiles.Count <= 2)
			return CityTrait.Small;
		else if (cityTiles.Count <= 4)
			return CityTrait.Medium;
		else return CityTrait.Large;
	}

	public float GetSettlementValue()
	{
		float val = cityTiles.Count;
		foreach(var tile in cityTiles)
		{
			foreach (var adj in MapGenerator.Terrain.GetAdjacentValues(tile))
			{
				if(adj == MapGenerator.Environment.Ocean || adj == MapGenerator.Environment.River || adj == MapGenerator.Environment.Road)
					val += .5f;
			}
		}

		return val;
	}

	public float GetSettlementDefensibility()
	{
		float defensibility = GetSettlementValue();
		foreach (var tile in cityTiles)
		{
			foreach (var adj in MapGenerator.Terrain.GetAdjacentValues(tile))
			{
				if (adj == MapGenerator.Environment.GetGround("Swamp") || adj == MapGenerator.Environment.GetGround("Mountain"))
					defensibility += .5f;
				if (adj == MapGenerator.Environment.Ocean)
					defensibility += .3f;
				if (adj == MapGenerator.Environment.River)
					defensibility += .2f;
				if (adj == MapGenerator.Environment.GetGround("Fertile") || adj == MapGenerator.Environment.GetGround("Wilderness"))
					defensibility -= .5f;
				if (adj == MapGenerator.Environment.Road || adj == MapGenerator.Environment.GetGround("Forest"))
					defensibility -= 1f;
			}
		}
		return defensibility;
	}

	public Int2 GetInfoPlacementPos()
	{
		Int2 minCityTile = cityTiles[0];
		foreach(var tile in cityTiles)
		{
			if(tile.Y < minCityTile.Y)
			{
				minCityTile = tile;
			}
		}

		return minCityTile - new Int2(0, 1);
	}
}
