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
		TerrainMapGenerator.TerrainMap.SetPoint(cityTiles[0], GroundTypes.Type.City);

		float valuePerNewTile = 10;
		while (cityTiles.Count < regionValue / valuePerNewTile)
		{
			var expansionTiles = GetPossibleExpnasionTiles();
			if (expansionTiles.Count == 0)
				break;
			cityTiles.Add(expansionTiles.TopValue());
			TerrainMapGenerator.TerrainMap.SetPoint(expansionTiles.TopValue(), GroundTypes.Type.City);
		}
	}

	private SortedDupList<Int2> GetPossibleExpnasionTiles()
	{
		SortedDupList<Int2> possibleExpansions = new SortedDupList<Int2>();
		foreach (Int2 cityTile in cityTiles)
		{
			foreach (Int2 neighbor in TerrainMapGenerator.TerrainMap.GetAdjacentPoints(cityTile))
			{
				var neighborType = TerrainMapGenerator.TerrainMap.GetValueAt(neighbor);
				if (!possibleExpansions.ContainsValue(neighbor) &&
					neighborType != GroundTypes.Type.City &&
					neighborType != GroundTypes.Type.Ocean &&
					neighborType != GroundTypes.Type.River &&
					neighborType != GroundTypes.Type.Mountain &&
					RegionsMapGenerator.RegionsMap.GetValueAt(neighbor).settlement == this &&
					!BordersUnfriendlyCity(neighbor))
				{
					possibleExpansions.Insert(kingdom.culture.TileAreaValue(neighbor, true), neighbor);
				}
			}
		}
		return possibleExpansions;
	}

	private bool BordersUnfriendlyCity(Int2 tile)
	{
		bool bordersUnfriendlyCity = false;
		foreach(var border in TerrainMapGenerator.TerrainMap.GetAllNeighboringPoints(tile))
		{
			if(TerrainMapGenerator.TerrainMap.GetValueAt(border) == GroundTypes.Type.City)
			{
				if(!cityTiles.Contains(border))
					bordersUnfriendlyCity = true;
			}
		}
		return bordersUnfriendlyCity;
	}

	public List<Settlement.CityTrait> GetCityTraits()
	{
		List<GroundTypes.Type> neighboringTerrainTypes = new List<GroundTypes.Type>();
		foreach (Int2 tile in cityTiles)
		{
			foreach (var neighbor in TerrainMapGenerator.TerrainMap.GetAdjacentValues(tile))
			{
				neighboringTerrainTypes.Add(neighbor);
			}
		}

		List<Settlement.CityTrait> traits = new List<CityTrait>();
		if (neighboringTerrainTypes.Contains(GroundTypes.Type.Mountain))
			traits.Add(CityTrait.Mountains);
		if (neighboringTerrainTypes.Contains(GroundTypes.Type.Forest))
			traits.Add(CityTrait.Forest);
		if (neighboringTerrainTypes.Contains(GroundTypes.Type.Fertile))
			traits.Add(CityTrait.Fertile);


		if (neighboringTerrainTypes.Contains(GroundTypes.Type.Ocean))
			traits.Add(CityTrait.Port);
		else if (neighboringTerrainTypes.Contains(GroundTypes.Type.River))
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
			foreach (var adj in TerrainMapGenerator.TerrainMap.GetAdjacentValues(tile))
			{
				if(adj == GroundTypes.Type.Ocean || adj == GroundTypes.Type.River || adj == GroundTypes.Type.Road)
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
			foreach (var adj in TerrainMapGenerator.TerrainMap.GetAdjacentValues(tile))
			{
				if (adj == GroundTypes.Type.Swamp || adj == GroundTypes.Type.Mountain)
					defensibility += .5f;
				if (adj == GroundTypes.Type.Ocean)
					defensibility += .3f;
				if (adj == GroundTypes.Type.River)
					defensibility += .2f;
				if (adj == GroundTypes.Type.Fertile || adj == GroundTypes.Type.Grass)
					defensibility -= .5f;
				if (adj == GroundTypes.Type.Road || adj == GroundTypes.Type.Forest)
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
