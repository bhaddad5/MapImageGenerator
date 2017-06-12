﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RegionsMapGenerator
{
	List<Region> regions = new List<Region>();
	Map2D<RegionTile> map;
	public int Width { get { return map.Width; } }
	public int Height { get { return map.Height; } }

	public RegionsMapGenerator(StoredTerrainMap terrainMap, int numberOfSettlements)
	{
		StartFillMap(terrainMap);

		var settlementLocations = GetSettlementLocations(terrainMap, numberOfSettlements);

		for(int i = settlementLocations.Count - 1; i >= 0; i--)
		{
			Region r = new Region("Region" + i, settlementLocations.KeyAt(i));
			ExpandRegionFromSettlement(2, r, settlementLocations.ValueAt(i), terrainMap);
		}

		EndFillMap(terrainMap);
	}

	private void StartFillMap(StoredTerrainMap terrainMap)
	{
		Region NoMansLand = new Region("NoMansLand", 0f);
		NoMansLand.color = Color.black;
		regions.Add(NoMansLand);

		map = new Map2D<RegionTile>(terrainMap.Width, terrainMap.Height);
		foreach(var pixel in map.GetMapPoints())
		{
			map.SetPoint(pixel, new RegionTile(NoMansLand));
		}
	}

	private void EndFillMap(StoredTerrainMap terrainMap)
	{
		Region OceanRegion = new Region("Ocean", 0f);
		OceanRegion.color = Color.blue;
		regions.Add(OceanRegion);

		foreach (var pixel in map.GetMapPoints())
		{
			if(terrainMap.TileIsType(pixel, TerrainTile.TileType.Ocean))
				map.SetPoint(pixel, new RegionTile(OceanRegion));
		}
	}

	private SortedDupList<Int2> GetSettlementLocations(StoredTerrainMap terrainMap, int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();

		for(int i = 0; i < numberOfSettlements * 10; i++)
		{
			Int2 testPos = new Int2(UnityEngine.Random.Range(0, terrainMap.Width), UnityEngine.Random.Range(0, terrainMap.Height));
			if (!terrainMap.TileIsType(testPos, TerrainTile.TileType.Ocean) &&
				!terrainMap.TileIsType(testPos, TerrainTile.TileType.Mountain) &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(terrainMap.Width, terrainMap.Height)))
			{
				regions.Insert(terrainMap.TileAreaValue(testPos), testPos);
			}
		}

		return PickUsedSettlementsFromSortedList(numberOfSettlements, regions);
	}

	private SortedDupList<Int2> PickUsedSettlementsFromSortedList(int numOfSettlements, SortedDupList<Int2> regions)
	{
		SortedDupList<Int2> usedSettlements = new SortedDupList<Int2>();
		int regionsIndex = 0;
		for (int i = 0; i < numOfSettlements; i++)
		{
			if (regionsIndex >= regions.Count)
			{
				Console.WriteLine("We ran out of regions!");
				break;
			}

			usedSettlements.Insert(regions.KeyAt(regionsIndex), regions.ValueAt(regionsIndex));
			if (i == (int)(numOfSettlements * .75f))
				regionsIndex = (int)(regions.Count * .75f);
			if (i == (int)(numOfSettlements * .5f))
				regionsIndex = (int)(regions.Count * .5f);
			else if (i == (int)(numOfSettlements * .25f))
				regionsIndex = (int)(regions.Count * .25f);
			else if (i == (int)(numOfSettlements * .10f))
				regionsIndex = (int)(regions.Count * .15f);

			regionsIndex++;
		}
		return usedSettlements;
	}

	private bool TooCloseToExistingSettlement(Int2 pos, SortedDupList<Int2> existingSettlements)
	{
		int minDist = 4;
		for(int i = 0; i < existingSettlements.Count; i++)
		{
			Int2 existing = existingSettlements.ValueAt(i);
			if (pos.X > existing.X - minDist && pos.X < existing.X + minDist &&
				pos.Y > existing.Y - minDist && pos.Y < existing.Y + minDist)
				return true;
		}
		return false;
	}

	private bool TooCloseToBorder(Int2 pos, Int2 mapDimensions)
	{
		int minDist = 3;
		return pos.X < minDist || pos.X > mapDimensions.X - minDist ||
			pos.Y < minDist || pos.Y > mapDimensions.Y - minDist;
	}

	private void ExpandRegionFromSettlement(float startingValue, Region region, Int2 pos, StoredTerrainMap terrainMap)
	{
		TileAt(pos).SetIsSettlement(true);
		TileAt(pos).TrySetRegion(region, startingValue - terrainMap.TileDifficulty(pos));

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(TileAt(pos).holdingStrength, pos);
		while(frontierTiles.Count > 0)
		{
			foreach(var neighbor in GetPossibleNeighborTiles(frontierTiles.TopValue(), region, terrainMap))
			{
				float strength = frontierTiles.TopKey() - terrainMap.TileDifficulty(neighbor);
				if(terrainMap.TileIsType(neighbor, TerrainTile.TileType.Ocean) &&
					!terrainMap.TileIsType(frontierTiles.TopValue(), TerrainTile.TileType.Ocean))
				{
					strength = strength - TerrainTile.startOceanDifficulty;
				}

				if (TileAt(neighbor).TrySetRegion(region, strength))
				{
					frontierTiles.Insert(strength, neighbor);
				}
			}
			frontierTiles.Pop();
		}
	}

	private List<Int2> GetPossibleNeighborTiles(Int2 pos, Region region, StoredTerrainMap terrainMap)
	{
		List<Int2> neighbors = new List<Int2>();
		neighbors.Add(pos + new Int2(1, 0));
		neighbors.Add(pos + new Int2(0, 1));
		neighbors.Add(pos + new Int2(0, -1));
		neighbors.Add(pos + new Int2(-1, 0));

		for (int i = neighbors.Count - 1; i >= 0; i--)
		{
			if (!IsPossibleNeighbor(neighbors[i], region, terrainMap))
				neighbors.RemoveAt(i);
		}
		return neighbors;
	}

	private bool IsPossibleNeighbor(Int2 neighbor, Region region, StoredTerrainMap terrainMap)
	{
		return terrainMap.TileInBounds(neighbor) && TileAt(neighbor).region != region;
	}

	private RegionTile TileAt(Int2 pos)
	{
		return map.GetValueAt(pos);
	}

	public Color GetTileColor(Int2 pos)
	{
		return TileAt(pos).GetColor();
	}
}