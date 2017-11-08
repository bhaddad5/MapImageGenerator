using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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
		MapGenerator.Terrain.Set(cityTiles[0], MapGenerator.RealmModel.GetFirstWithTrait(TerrainInfo.GroundTraits.City));

		float valuePerNewTile = 10;
		while (cityTiles.Count < regionValue / valuePerNewTile)
		{
			var expansionTiles = GetPossibleExpnasionTiles();
			if (expansionTiles.Count == 0)
				break;
			cityTiles.Add(expansionTiles.TopValue());
			MapGenerator.Terrain.Set(expansionTiles.TopValue(), MapGenerator.RealmModel.GetFirstWithTrait(TerrainInfo.GroundTraits.City));
		}

		var traits = GetCityTraits();
		foreach (Int2 tile in cityTiles)
		{
			if(traits.Contains(CityTrait.Large))
				MapGenerator.Terrain.Set(tile, MapGenerator.RealmModel.GetFirstWithTrait(TerrainInfo.GroundTraits.LargeCity));
			if (traits.Contains(CityTrait.Medium))
				MapGenerator.Terrain.Set(tile, MapGenerator.RealmModel.GetFirstWithTrait(TerrainInfo.GroundTraits.MediumCity));
			if (traits.Contains(CityTrait.Small))
				MapGenerator.Terrain.Set(tile, MapGenerator.RealmModel.GetFirstWithTrait(TerrainInfo.GroundTraits.SmallCity));
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
					!neighborType.HasTrait(TerrainInfo.GroundTraits.City) &&
					!neighborType.HasTrait(TerrainInfo.GroundTraits.Impassable) &&
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
			if(MapGenerator.Terrain.Get(border).HasTrait(TerrainInfo.GroundTraits.City))
			{
				if(!cityTiles.Contains(border))
					bordersUnfriendlyCity = true;
			}
		}
		return bordersUnfriendlyCity;
	}

	public List<Settlement.CityTrait> GetCityTraits()
	{
		List<TerrainInfo> neighboringTerrainTypes = new List<TerrainInfo>();
		foreach (Int2 tile in cityTiles)
		{
			foreach (var neighbor in MapGenerator.Terrain.GetAdjacentValues(tile))
			{
				neighboringTerrainTypes.Add(neighbor);
			}
		}

		List<Settlement.CityTrait> traits = new List<CityTrait>();
		foreach (TerrainInfo groundInfo in neighboringTerrainTypes)
		{
			if(groundInfo.traits.Contains(TerrainInfo.GroundTraits.Rocky))
				traits.Add(CityTrait.Mountains);
			if (groundInfo.traits.Contains(TerrainInfo.GroundTraits.Forest))
				traits.Add(CityTrait.Forest);
			if (groundInfo.traits.Contains(TerrainInfo.GroundTraits.Fertile))
				traits.Add(CityTrait.Fertile);
			if (groundInfo.traits.Contains(TerrainInfo.GroundTraits.Water))
				traits.Add(CityTrait.Port);
			if (neighboringTerrainTypes.Contains(MapGenerator.RealmModel.River))
				traits.Add(CityTrait.River);
		}

		if(!traits.Contains(CityTrait.Port) && !traits.Contains(CityTrait.River))
			traits.Add(CityTrait.Landlocked);

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
				if(adj == MapGenerator.RealmModel.Ocean || adj == MapGenerator.RealmModel.River || adj.HasTrait(TerrainInfo.GroundTraits.Road))
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
				defensibility += adj.GetDefensibility();
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
