using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RegionsMapGenerator
{
	List<Kingdom> regions = new List<Kingdom>();
	Map2D<RegionTile> map;
	TerrainMapGenerator terrainMap;
	public int Width { get { return map.Width; } }
	public int Height { get { return map.Height; } }

	public RegionsMapGenerator(TerrainMapGenerator tm, int numberOfSettlements)
	{
		terrainMap = tm;

		StartFillMap();

		var settlementLocations = GetSettlementLocations(numberOfSettlements);

		for(int i = settlementLocations.Count - 1; i >= 0; i--)
		{
			Kingdom r = new Kingdom("Region" + i, settlementLocations.ValueAt(i));
			regions.Add(r);
			ExpandRegionFromSettlement(5, r, settlementLocations.ValueAt(i));
		}

		EndFillMap();

		CalculateRegionValues();

		ExpandSettlements();

		BuildRoads();

		foreach(var region in regions)
		{
			if (region.settlement != null)
			{
				var cityTraits = region.settlement.GetCityTraits(terrainMap.GetTerrainMap());
				region.settlement.name = SettlementNameGenerator.GetSettlementName(CultureDefinitions.Anglo, cityTraits);
				region.settlement.heraldry = HeraldryGenerator.GetHeraldry(CultureDefinitions.Anglo, cityTraits, region);
			}
		}
	}

	private void StartFillMap()
	{
		Kingdom NoMansLand = new Kingdom("NoMansLand", null);
		NoMansLand.mainColor = Color.black;
		regions.Add(NoMansLand);

		map = new Map2D<RegionTile>(terrainMap.GetTerrainMap().Width, terrainMap.GetTerrainMap().Height);
		foreach(var pixel in map.GetMapPoints())
		{
			map.SetPoint(pixel, new RegionTile(NoMansLand));
		}
	}

	private void EndFillMap()
	{
		Kingdom OceanRegion = new Kingdom("Ocean", null);
		OceanRegion.mainColor = Color.blue;
		regions.Add(OceanRegion);

		foreach (var pixel in map.GetMapPoints())
		{
			if(terrainMap.TileIsType(pixel, TerrainTile.TileType.Ocean) ||
				terrainMap.TileIsType(pixel, TerrainTile.TileType.River))
				map.SetPoint(pixel, new RegionTile(OceanRegion));
		}
	}

	private SortedDupList<Int2> GetSettlementLocations(int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();

		for(int i = 0; i < numberOfSettlements * 120; i++)
		{
			Int2 testPos = new Int2(UnityEngine.Random.Range(0, terrainMap.GetTerrainMap().Width), UnityEngine.Random.Range(0, terrainMap.GetTerrainMap().Height));
			if (!terrainMap.TileIsType(testPos, TerrainTile.TileType.Ocean) &&
				!terrainMap.TileIsType(testPos, TerrainTile.TileType.River) &&
				!terrainMap.TileIsType(testPos, TerrainTile.TileType.Mountain) &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(terrainMap.GetTerrainMap().Width, terrainMap.GetTerrainMap().Height)))
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

	private void ExpandRegionFromSettlement(float startingValue, Kingdom region, Int2 pos)
	{
		TileAt(pos).TrySetRegion(region, startingValue - terrainMap.TileDifficulty(pos));

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(TileAt(pos).holdingStrength, pos);
		while(frontierTiles.Count > 0)
		{
			foreach(var neighbor in GetPossibleNeighborTiles(frontierTiles.TopValue(), region))
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

	private List<Int2> GetPossibleNeighborTiles(Int2 pos, Kingdom region)
	{
		List<Int2> goodNeighbors = new List<Int2>();
		foreach (Int2 adjacent in terrainMap.GetTerrainMap().GetAdjacentPoints(pos))
		{
			if (IsPossibleNeighbor(adjacent, region))
				goodNeighbors.Add(adjacent);
		}
		return goodNeighbors;
	}

	private bool IsPossibleNeighbor(Int2 neighbor, Kingdom region)
	{
		return terrainMap.TileInBounds(neighbor) && TileAt(neighbor).region != region;
	}

	private void CalculateRegionValues()
	{
		foreach(Int2 tile in map.GetMapPoints())
			map.GetValueAt(tile).region.value += terrainMap.TileAt(tile).GetValue();
	}

	private void ExpandSettlements()
	{
		foreach(var reg in regions)
		{
			if(reg.settlement != null)
				reg.settlement.ExpandSettlement(reg.value, terrainMap, map, reg);
		}
	}

	private void BuildRoads()
	{
		foreach(var region in regions)
		{
			if(region.settlement != null)
			{
				HashSet<Settlement> settlementsHit = new HashSet<Settlement>();
				settlementsHit.Add(region.settlement);
				BuildRoadsFromSettlement(region.settlement, settlementsHit);
			}
		}
	}

	private void BuildRoadsFromSettlement(Settlement settlement, HashSet<Settlement> settlementsHit)
	{
		Int2 startTile = settlement.cityTiles[0];

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(5f, startTile);

		Map2D<float> distMap = new Map2D<float>(map.Width, map.Height);

		while(frontierTiles.Count > 0)
		{
			distMap.SetPoint(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach(var tile in terrainMap.GetTerrainMap().GetAdjacentPoints(currTile))
			{
				if (distMap.GetValueAt(tile) != 0 ||
					terrainMap.GetTerrainMap().GetValueAt(tile).tileType == TerrainTile.TileType.Ocean)
					continue;

				if(terrainMap.GetTerrainMap().GetValueAt(tile).tileType == TerrainTile.TileType.City)
				{
					var sett = GetSettlementFromTile(tile);
					if (!settlementsHit.Contains(sett))
					{
						settlementsHit.Add(GetSettlementFromTile(tile));
						BuildRoadBackFromTile(currTile, distMap);
						BuildRoadsFromSettlement(settlement, settlementsHit);
						return;
					}
				}

				float difficulty = terrainMap.TileDifficulty(tile);
				if(currDifficulty - difficulty > 0)
				{
					frontierTiles.Insert(currDifficulty - difficulty, tile);
					distMap.SetPoint(tile, currDifficulty - difficulty);
				}
			}
		}
	}

	private void BuildRoadBackFromTile(Int2 tile, Map2D<float> distMap)
	{
		if (terrainMap.GetTerrainMap().GetValueAt(tile).tileType == TerrainTile.TileType.Road)
			return;
		if (terrainMap.GetTerrainMap().GetValueAt(tile).tileType != TerrainTile.TileType.City)
			terrainMap.SetValue(tile, new TerrainTile(TerrainTile.TileType.Road));
		Int2 maxTile = tile;
		foreach(var t in distMap.GetAdjacentPoints(tile))
		{
			if (distMap.GetValueAt(t) > distMap.GetValueAt(maxTile))
				maxTile = t;
		}

		if (maxTile != tile)
			BuildRoadBackFromTile(maxTile, distMap);
	}

	private Settlement GetSettlementFromTile(Int2 tile)
	{
		foreach(var region in regions)
		{
			if (region.settlement == null)
				continue;
			foreach(var cityTile in region.settlement.cityTiles)
			{
				if (tile.Equals(cityTile))
					return region.settlement;
			}
		}
		return null;
	}

	private RegionTile TileAt(Int2 pos)
	{
		return map.GetValueAt(pos);
	}

	public Color GetTileColor(Int2 pos)
	{
		return TileAt(pos).GetColor();
	}

	public List<Kingdom> GetRegions()
	{
		return regions;
	}
}
