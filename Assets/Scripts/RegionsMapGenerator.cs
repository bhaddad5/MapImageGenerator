﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RegionsMapGenerator
{
	public static Map2D<RegionTile> RegionsMap;
	public static List<Kingdom> Kingdoms = new List<Kingdom>();
	public int Width { get { return RegionsMap.Width; } }
	public int Height { get { return RegionsMap.Height; } }

	public RegionsMapGenerator(List<MapBuilder.CulturePrevelance> cultures)
	{
		StartFillMap();

		foreach(var culture in cultures)
		{
			var settlementLocations = GetSettlementLocations(culture.culture, culture.numSettlements);

			for (int i = settlementLocations.Count - 1; i >= 0; i--)
			{
				TerrainMapGenerator.TerrainMap.SetPoint(settlementLocations.ValueAt(i), new TerrainTile(TerrainTile.TileType.City));
				Kingdom r = new Kingdom(culture.culture, settlementLocations.ValueAt(i));
				Kingdoms.Add(r);
				ExpandRegionFromSettlement(5, r.settlements[0], settlementLocations.ValueAt(i));
			}
		}

		float humanTotal = 0;
		float orcTotal = 0;
		float DwarfTotal = 0;
		foreach(var Kingdom in Kingdoms)
		{
			if (Kingdom.culture == CultureDefinitions.Anglo)
				humanTotal += Kingdom.Strength(TerrainMapGenerator.TerrainMap);
			if (Kingdom.culture == CultureDefinitions.Dwarf)
				DwarfTotal += Kingdom.Strength(TerrainMapGenerator.TerrainMap);
			if (Kingdom.culture == CultureDefinitions.Orc)
				orcTotal += Kingdom.Strength(TerrainMapGenerator.TerrainMap);
		}

		Debug.Log(humanTotal + ", " + DwarfTotal + " , " + orcTotal);

		EndFillMap();

		CalculateRegionValues();

		ExpandSettlements();

		BuildRoads();

		FightWars();

		foreach(var kingdom in Kingdoms)
		{
			kingdom.SetNamesAndHeraldry(TerrainMapGenerator.TerrainMap);
		}

		/*foreach (var kingdom in Kingdoms)
		{
			kingdom.PrintKingdomInfo(TerrainMap.GetTerrainMap());
		}*/

		humanTotal = 0;
		orcTotal = 0;
		DwarfTotal = 0;
		foreach (var Kingdom in Kingdoms)
		{
			if (Kingdom.culture == CultureDefinitions.Anglo)
				humanTotal += Kingdom.Strength(TerrainMapGenerator.TerrainMap);
			if (Kingdom.culture == CultureDefinitions.Dwarf)
				DwarfTotal += Kingdom.Strength(TerrainMapGenerator.TerrainMap);
			if (Kingdom.culture == CultureDefinitions.Orc)
				orcTotal += Kingdom.Strength(TerrainMapGenerator.TerrainMap);
		}

		Debug.Log(humanTotal + ", " + DwarfTotal + " , " + orcTotal);
	}

	private void StartFillMap()
	{
		Kingdom noMansLand = new Kingdom(CultureDefinitions.Anglo, new Int2(0, 0));
		RegionsMap = new Map2D<RegionTile>(TerrainMapGenerator.TerrainMap.Width, TerrainMapGenerator.TerrainMap.Height);
		foreach(var pixel in RegionsMap.GetMapPoints())
		{
			RegionsMap.SetPoint(pixel, new RegionTile(noMansLand.settlements[0]));
		}
	}

	private void EndFillMap()
	{
		Kingdom Ocean = new Kingdom(CultureDefinitions.Anglo, new Int2(0, 0));
		foreach (var pixel in RegionsMap.GetMapPoints())
		{
			if(TerrainMapGenerator.TerrainMap.GetValueAt(pixel).tileType == TerrainTile.TileType.Ocean ||
			   TerrainMapGenerator.TerrainMap.GetValueAt(pixel).tileType == TerrainTile.TileType.River)
				RegionsMap.SetPoint(pixel, new RegionTile(Ocean.settlements[0]));
		}
	}

	private SortedDupList<Int2> GetSettlementLocations(Culture culture, int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();

		for(int i = 0; i < numberOfSettlements * 200; i++)
		{
			Int2 testPos = new Int2(UnityEngine.Random.Range(0, TerrainMapGenerator.TerrainMap.Width), UnityEngine.Random.Range(0, TerrainMapGenerator.TerrainMap.Height));
			if (TerrainMapGenerator.TerrainMap.GetValueAt(testPos).tileType != TerrainTile.TileType.Ocean &&
				TerrainMapGenerator.TerrainMap.GetValueAt(testPos).tileType != TerrainTile.TileType.River &&
				TerrainMapGenerator.TerrainMap.GetValueAt(testPos).tileType != TerrainTile.TileType.Mountain &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(TerrainMapGenerator.TerrainMap.Width, TerrainMapGenerator.TerrainMap.Height)))
			{
				regions.Insert(TerrainMapGenerator.TileAreaValue(culture, testPos), testPos);
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

			if (TerrainMapGenerator.TerrainMap.GetValueAt(regions.ValueAt(regionsIndex)).tileType == TerrainTile.TileType.City ||
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
		foreach (var border in TerrainMapGenerator.TerrainMap.GetAdjacentPoints(tile))
		{
			if (TerrainMapGenerator.TerrainMap.GetValueAt(border).tileType == TerrainTile.TileType.City)
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
		TileAt(pos).TrySetRegion(settlement, startingValue - settlement.kingdom.culture.GetTileDifficulty(pos, TerrainMapGenerator.TerrainMap));

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(TileAt(pos).holdingStrength, pos);
		while(frontierTiles.Count > 0)
		{
			foreach(var neighbor in GetPossibleNeighborTiles(frontierTiles.TopValue(), settlement))
			{
				float strength = frontierTiles.TopKey() - settlement.kingdom.culture.GetTileDifficulty(neighbor, TerrainMapGenerator.TerrainMap);
				if(TerrainMapGenerator.TerrainMap.GetValueAt(neighbor).tileType == TerrainTile.TileType.Ocean &&
					TerrainMapGenerator.TerrainMap.GetValueAt(frontierTiles.TopValue()).tileType != TerrainTile.TileType.Ocean)
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
		foreach (Int2 adjacent in TerrainMapGenerator.TerrainMap.GetAdjacentPoints(pos))
		{
			if (IsPossibleNeighbor(adjacent, settlement))
				goodNeighbors.Add(adjacent);
		}
		return goodNeighbors;
	}

	private bool IsPossibleNeighbor(Int2 neighbor, Settlement settlement)
	{
		return TerrainMapGenerator.TerrainMap.PosInBounds(neighbor) && TileAt(neighbor).settlement != settlement;
	}

	private void CalculateRegionValues()
	{
		foreach(Int2 tile in RegionsMap.GetMapPoints())
			RegionsMap.GetValueAt(tile).settlement.kingdom.value += RegionsMap.GetValueAt(tile).settlement.kingdom.culture.GetTileValue(tile, TerrainMapGenerator.TerrainMap);
	}

	private void ExpandSettlements()
	{
		foreach(var reg in Kingdoms)
		{
			foreach(var sett in reg.settlements)
				sett.ExpandSettlement(reg.value, RegionsMap);
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

		Map2D<float> distMap = new Map2D<float>(RegionsMap.Width, RegionsMap.Height);

		while(frontierTiles.Count > 0)
		{
			distMap.SetPoint(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach(var tile in TerrainMapGenerator.TerrainMap.GetAdjacentPoints(currTile))
			{
				if (distMap.GetValueAt(tile) != 0 ||
				    TerrainMapGenerator.TerrainMap.GetValueAt(tile).tileType == TerrainTile.TileType.Ocean)
					continue;

				if(TerrainMapGenerator.TerrainMap.GetValueAt(tile).tileType == TerrainTile.TileType.City)
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

				float difficulty = settlement.kingdom.culture.GetTileDifficulty(tile, TerrainMapGenerator.TerrainMap);
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
		if (TerrainMapGenerator.TerrainMap.GetValueAt(tile).tileType == TerrainTile.TileType.Road)
			return;
		if (TerrainMapGenerator.TerrainMap.GetValueAt(tile).tileType != TerrainTile.TileType.City)
			TerrainMapGenerator.TerrainMap.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Road));
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

		Map2D<float> distMap = new Map2D<float>(RegionsMap.Width, RegionsMap.Height);

		while (frontierTiles.Count > 0)
		{
			distMap.SetPoint(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach (var tile in TerrainMapGenerator.TerrainMap.GetAdjacentPoints(currTile))
			{
				if (distMap.GetValueAt(tile) != 0)
					continue;

				if (TerrainMapGenerator.TerrainMap.GetValueAt(tile).tileType == TerrainTile.TileType.City)
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
					var type = TerrainMapGenerator.TerrainMap.GetValueAt(tile).tileType;
					var currType = TerrainMapGenerator.TerrainMap.GetValueAt(currTile).tileType;
					if ((type == TerrainTile.TileType.Ocean && (currType == TerrainTile.TileType.Ocean || currType == TerrainTile.TileType.City)) ||
					(type == TerrainTile.TileType.River && (currType == TerrainTile.TileType.River || currType == TerrainTile.TileType.City)) ||
					(type == TerrainTile.TileType.Road && (currType == TerrainTile.TileType.Road || currType == TerrainTile.TileType.City)))
					{
						float difficulty = settlement.kingdom.culture.GetTileDifficulty(tile, TerrainMapGenerator.TerrainMap);
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
		foreach (var kingdom in Kingdoms)
		{
			Settlement closestSett = kingdom.ClosestEnemySettlement();
			if (closestSett != null)
			{
				float attackPower = kingdom.Strength(TerrainMapGenerator.TerrainMap) * Random.Range(.75f, 1.25f) * kingdom.culture.AttackMultiplier;
				float defenderPower = (closestSett.kingdom.Strength(TerrainMapGenerator.TerrainMap) + closestSett.GetSettlementDefensibility(TerrainMapGenerator.TerrainMap)) * Random.Range(.75f, 1.25f) * closestSett.kingdom.culture.DefenseMultiplier;

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
		return RegionsMap.GetValueAt(pos);
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
		return RegionsMap;
	}
}
