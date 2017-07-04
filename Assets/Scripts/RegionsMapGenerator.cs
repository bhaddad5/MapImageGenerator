using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RegionsMapGenerator
{
	public List<Kingdom> Kingdoms = new List<Kingdom>();
	Map2D<RegionTile> map;
	TerrainMapGenerator terrainMap;
	public int Width { get { return map.Width; } }
	public int Height { get { return map.Height; } }

	public RegionsMapGenerator(TerrainMapGenerator tm, List<MapBuilder.CulturePrevelance> cultures)
	{
		terrainMap = tm;

		StartFillMap();

		foreach(var culture in cultures)
		{
			var settlementLocations = GetSettlementLocations(culture.culture, culture.numSettlements);

			for (int i = settlementLocations.Count - 1; i >= 0; i--)
			{
				terrainMap.SetValue(settlementLocations.ValueAt(i), new TerrainTile(TerrainTile.TileType.City));
				Kingdom r = new Kingdom(culture.culture, settlementLocations.ValueAt(i));
				Kingdoms.Add(r);
				ExpandRegionFromSettlement(5, r.settlements[0], settlementLocations.ValueAt(i));
			}
		}

		EndFillMap();

		CalculateRegionValues();

		ExpandSettlements();

		BuildRoads();

		FightWars();

		foreach(var kingdom in Kingdoms)
		{
			kingdom.SetNamesAndHeraldry(terrainMap.GetTerrainMap());
		}

		/*foreach (var kingdom in Kingdoms)
		{
			kingdom.PrintKingdomInfo(terrainMap.GetTerrainMap());
		}*/
	}

	private void StartFillMap()
	{
		Kingdom noMansLand = new Kingdom(CultureDefinitions.Anglo, new Int2(0, 0));
		map = new Map2D<RegionTile>(terrainMap.GetTerrainMap().Width, terrainMap.GetTerrainMap().Height);
		foreach(var pixel in map.GetMapPoints())
		{
			map.SetPoint(pixel, new RegionTile(noMansLand.settlements[0]));
		}
	}

	private void EndFillMap()
	{
		Kingdom Ocean = new Kingdom(CultureDefinitions.Anglo, new Int2(0, 0));
		foreach (var pixel in map.GetMapPoints())
		{
			if(terrainMap.TileIsType(pixel, TerrainTile.TileType.Ocean) ||
				terrainMap.TileIsType(pixel, TerrainTile.TileType.River))
				map.SetPoint(pixel, new RegionTile(Ocean.settlements[0]));
		}
	}

	private SortedDupList<Int2> GetSettlementLocations(Culture culture, int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();

		for(int i = 0; i < numberOfSettlements * 200; i++)
		{
			Int2 testPos = new Int2(UnityEngine.Random.Range(0, terrainMap.GetTerrainMap().Width), UnityEngine.Random.Range(0, terrainMap.GetTerrainMap().Height));
			if (!terrainMap.TileIsType(testPos, TerrainTile.TileType.Ocean) &&
				!terrainMap.TileIsType(testPos, TerrainTile.TileType.River) &&
				!terrainMap.TileIsType(testPos, TerrainTile.TileType.Mountain) &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(terrainMap.GetTerrainMap().Width, terrainMap.GetTerrainMap().Height)))
			{
				regions.Insert(terrainMap.TileAreaValue(culture, testPos), testPos);
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
				Debug.Log("We ran out of regions!");
				break;
			}

			if (terrainMap.GetTerrainMap().GetValueAt(regions.ValueAt(regionsIndex)).tileType == TerrainTile.TileType.City ||
				BordersCity(regions.ValueAt(regionsIndex)))
			{
				regionsIndex++;
				continue;
			}

			usedSettlements.Insert(regions.KeyAt(regionsIndex), regions.ValueAt(regionsIndex));
			if (i == (int)(numOfSettlements * .75f))
				regionsIndex = (int)(regions.Count * .6f);
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

	private bool BordersCity(Int2 tile)
	{
		bool bordersCity = false;
		foreach (var border in terrainMap.GetTerrainMap().GetAdjacentPoints(tile))
		{
			if (terrainMap.GetTerrainMap().GetValueAt(border).tileType == TerrainTile.TileType.City)
			{
				bordersCity = true;
			}
		}
		return bordersCity;
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

	private void ExpandRegionFromSettlement(float startingValue, Settlement settlement, Int2 pos)
	{
		TileAt(pos).TrySetRegion(settlement, startingValue - settlement.kingdom.culture.GetTileDifficulty(pos, terrainMap.GetTerrainMap()));

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(TileAt(pos).holdingStrength, pos);
		while(frontierTiles.Count > 0)
		{
			foreach(var neighbor in GetPossibleNeighborTiles(frontierTiles.TopValue(), settlement))
			{
				float strength = frontierTiles.TopKey() - settlement.kingdom.culture.GetTileDifficulty(neighbor, terrainMap.GetTerrainMap());
				if(terrainMap.TileIsType(neighbor, TerrainTile.TileType.Ocean) &&
					!terrainMap.TileIsType(frontierTiles.TopValue(), TerrainTile.TileType.Ocean))
				{
					strength = strength - TerrainTile.startOceanDifficulty;
				}

				if (TileAt(neighbor).TrySetRegion(settlement, strength))
				{
					frontierTiles.Insert(strength, neighbor);
				}
			}
			frontierTiles.Pop();
		}
	}

	private List<Int2> GetPossibleNeighborTiles(Int2 pos, Settlement settlement)
	{
		List<Int2> goodNeighbors = new List<Int2>();
		foreach (Int2 adjacent in terrainMap.GetTerrainMap().GetAdjacentPoints(pos))
		{
			if (IsPossibleNeighbor(adjacent, settlement))
				goodNeighbors.Add(adjacent);
		}
		return goodNeighbors;
	}

	private bool IsPossibleNeighbor(Int2 neighbor, Settlement settlement)
	{
		return terrainMap.TileInBounds(neighbor) && TileAt(neighbor).settlement != settlement;
	}

	private void CalculateRegionValues()
	{
		foreach(Int2 tile in map.GetMapPoints())
			map.GetValueAt(tile).settlement.kingdom.value += map.GetValueAt(tile).settlement.kingdom.culture.GetTileValue(tile, terrainMap.GetTerrainMap());
	}

	private void ExpandSettlements()
	{
		foreach(var reg in Kingdoms)
		{
			foreach(var sett in reg.settlements)
				sett.ExpandSettlement(reg.value, terrainMap, map, sett);
		}
	}

	private void BuildRoads()
	{
		foreach (var kingdom in Kingdoms)
		{
			foreach (var sett in kingdom.settlements)
			{
				HashSet<Settlement> settlementsHit = new HashSet<Settlement>();
				settlementsHit.Add(sett);
				BuildRoadsFromSettlement(sett, settlementsHit);
			}
		}
		foreach (var kingdom in Kingdoms)
		{
			foreach (var sett in kingdom.settlements)
			{
				sett.adjacentSettlements = GetAdjacentSettlements(sett);
			}
		}
	}

	private void BuildRoadsFromSettlement(Settlement settlement, HashSet<Settlement> settlementsHit)
	{
		Int2 startTile = settlement.cityTiles[0];

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(3f, startTile);

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

				float difficulty = settlement.kingdom.culture.GetTileDifficulty(tile, terrainMap.GetTerrainMap());
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

	private SortedDupList<Settlement> GetAdjacentSettlements(Settlement settlement)
	{
		SortedDupList<Settlement> hitSettlements = new SortedDupList<Settlement>();
		Int2 startTile = settlement.cityTiles[0];

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		float startDifficulty = 3f;
		frontierTiles.Insert(startDifficulty, startTile);

		Map2D<float> distMap = new Map2D<float>(map.Width, map.Height);

		while (frontierTiles.Count > 0)
		{
			distMap.SetPoint(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach (var tile in terrainMap.GetTerrainMap().GetAdjacentPoints(currTile))
			{
				if (distMap.GetValueAt(tile) != 0)
					continue;

				if (terrainMap.GetTerrainMap().GetValueAt(tile).tileType == TerrainTile.TileType.City)
				{
					var sett = GetSettlementFromTile(tile);
					if (!hitSettlements.ContainsValue(sett))
					{
						hitSettlements.Insert(startDifficulty - currDifficulty, GetSettlementFromTile(tile));
					}
					else if (sett == settlement)
					{
						frontierTiles.Insert(currDifficulty, tile);
						distMap.SetPoint(tile, currDifficulty);
					}
				}
				else {
					var type = terrainMap.GetTerrainMap().GetValueAt(tile).tileType;
					var currType = terrainMap.GetTerrainMap().GetValueAt(currTile).tileType;
					if ((type == TerrainTile.TileType.Ocean && (currType == TerrainTile.TileType.Ocean || currType == TerrainTile.TileType.City)) ||
					(type == TerrainTile.TileType.River && (currType == TerrainTile.TileType.River || currType == TerrainTile.TileType.City)) ||
					(type == TerrainTile.TileType.Road && (currType == TerrainTile.TileType.Road || currType == TerrainTile.TileType.City)))
					{
						float difficulty = settlement.kingdom.culture.GetTileDifficulty(tile, terrainMap.GetTerrainMap());
						if (currDifficulty - difficulty > 0)
						{
							frontierTiles.Insert(currDifficulty - difficulty, tile);
							distMap.SetPoint(tile, currDifficulty - difficulty);
						}
					}
				}
			}
		}
		return hitSettlements;
	}

	private Settlement GetSettlementFromTile(Int2 tile)
	{
		foreach(var region in Kingdoms)
		{
			foreach(var sett in region.settlements)
			{
				foreach (var cityTile in sett.cityTiles)
				{
					if (tile.Equals(cityTile))
						return sett;
				}
			}
		}
		return null;
	}

	private void FightWars()
	{
		int numWars = 5;
		for(int i = 0; i < numWars; i++)
		{
			FightWar();
		}
	}

	private void FightWar()
	{
		var ShuffledKingdoms = Kingdoms.OrderBy(a => Random.Range(0, 1f)).ToList();
		foreach (var kingdom in ShuffledKingdoms)
		{
			Settlement closestSett = kingdom.ClosestEnemySettlement();
			if (closestSett != null)
			{
				float attackPower = kingdom.Strength(terrainMap.GetTerrainMap()) * Random.Range(.75f, 1.25f) * kingdom.culture.AttackMultiplier;
				float defenderPower = (closestSett.kingdom.Strength(terrainMap.GetTerrainMap()) + closestSett.GetSettlementDefensibility(terrainMap.GetTerrainMap())) * Random.Range(.75f, 1.25f) * closestSett.kingdom.culture.DefenseMultiplier;

				if (attackPower > defenderPower)
				{
					kingdom.AddSettlement(closestSett);
					closestSett.kingdom.settlements.Remove(closestSett);
					closestSett.kingdom = kingdom;
				}
			}
		}

		for(int i = 0; i < Kingdoms.Count; i++)
		{
			if(Kingdoms[i].settlements.Count == 0)
			{
				Kingdoms.RemoveAt(i);
			}
		}
	}

	private RegionTile TileAt(Int2 pos)
	{
		return map.GetValueAt(pos);
	}

	public Color GetTileColor(Int2 pos)
	{
		return TileAt(pos).GetColor();
	}

	public List<Kingdom> GetKingdoms()
	{
		return Kingdoms;
	}

	public Map2D<RegionTile> GetRegionsMap()
	{
		return map;
	}
}
