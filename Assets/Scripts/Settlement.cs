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
	public List<Settlement> adjacentSettlements = new List<Settlement>();

	public List<Int2> cityTiles = new List<Int2>();
	public Kingdom kingdom;
	public Settlement(Int2 cityTile, Kingdom k)
	{
		cityTiles.Add(cityTile);
		kingdom = k;
	}

	public void ExpandSettlement(float regionValue, TerrainMapGenerator terrainTiles, Map2D<RegionTile> regionsMap, Kingdom myRegion)
	{
		terrainTiles.SetValue(cityTiles[0], new TerrainTile(TerrainTile.TileType.City));

		float valuePerNewTile = 10;
		while (cityTiles.Count < regionValue / valuePerNewTile)
		{
			var expansionTiles = GetPossibleExpnasionTiles(terrainTiles, regionsMap, myRegion);
			if (expansionTiles.Count == 0)
				break;
			cityTiles.Add(expansionTiles.TopValue());
			terrainTiles.SetValue(expansionTiles.TopValue(), new TerrainTile(TerrainTile.TileType.City));
		}
	}

	private SortedDupList<Int2> GetPossibleExpnasionTiles(TerrainMapGenerator terrainTiles, Map2D<RegionTile> regionsMap, Kingdom myRegion)
	{
		SortedDupList<Int2> possibleExpansions = new SortedDupList<Int2>();
		foreach (Int2 cityTile in cityTiles)
		{
			foreach (Int2 neighbor in terrainTiles.GetTerrainMap().GetAdjacentPoints(cityTile))
			{
				var neighborType = terrainTiles.GetTerrainMap().GetValueAt(neighbor).tileType;
				if (!possibleExpansions.ContainsValue(neighbor) &&
					neighborType != TerrainTile.TileType.City &&
					neighborType != TerrainTile.TileType.Ocean &&
					neighborType != TerrainTile.TileType.River &&
					neighborType != TerrainTile.TileType.Mountain &&
					regionsMap.GetValueAt(neighbor).region == myRegion)
				{
					possibleExpansions.Insert(terrainTiles.TileAreaValue(neighbor, true), neighbor);
				}
			}
		}
		return possibleExpansions;
	}

	public List<Settlement.CityTrait> GetCityTraits(Map2D<TerrainTile> terrainMap)
	{
		List<TerrainTile.TileType> neighboringTerrainTypes = new List<TerrainTile.TileType>();
		foreach (Int2 tile in cityTiles)
		{
			foreach (var neighbor in terrainMap.GetAdjacentValues(tile))
			{
				neighboringTerrainTypes.Add(neighbor.tileType);
			}
		}

		List<Settlement.CityTrait> traits = new List<CityTrait>();
		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Mountain))
			traits.Add(CityTrait.Mountains);
		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Forest))
			traits.Add(CityTrait.Forest);
		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Fertile))
			traits.Add(CityTrait.Fertile);


		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Ocean))
			traits.Add(CityTrait.Port);
		else if (neighboringTerrainTypes.Contains(TerrainTile.TileType.River))
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

	public float GetSettlementValue(Map2D<TerrainTile> terrainMap)
	{
		float val = cityTiles.Count;
		foreach(var tile in cityTiles)
		{
			foreach (var adj in terrainMap.GetAdjacentValues(tile))
			{
				if(adj.tileType == TerrainTile.TileType.Ocean || adj.tileType == TerrainTile.TileType.River || adj.tileType == TerrainTile.TileType.Road)
					val += .5f;
			}
		}

		return val;
	}

	public float GetSettlementDefensibility(Map2D<TerrainTile> terrainMap)
	{
		float defensibility = GetSettlementValue(terrainMap);
		foreach (var tile in cityTiles)
		{
			foreach (var adj in terrainMap.GetAdjacentValues(tile))
			{
				if (adj.tileType == TerrainTile.TileType.Swamp || adj.tileType == TerrainTile.TileType.Mountain)
					defensibility += .5f;
				if (adj.tileType == TerrainTile.TileType.Ocean)
					defensibility += .3f;
				if (adj.tileType == TerrainTile.TileType.River)
					defensibility += .2f;
				if (adj.tileType == TerrainTile.TileType.Fertile || adj.tileType == TerrainTile.TileType.Grass)
					defensibility -= .5f;
				if (adj.tileType == TerrainTile.TileType.Road || adj.tileType == TerrainTile.TileType.Forest)
					defensibility -= 1f;
			}
		}
		return defensibility;
	}
}
