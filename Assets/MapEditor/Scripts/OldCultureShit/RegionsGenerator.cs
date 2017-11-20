using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RegionsGenerator
{
	public MapModel Map;

	public void GenerateRegions(MapModel map, RealmModel realm)
	{
		Map = map;
		
		/*if (realm.Cultures.Count == 0)
			return;
			
		foreach (StoredCulturePrevelance culture in realm.Cultures)
		{
			float portionAtSeaLevel = (float)MapTextureHelpers.SeaLevelPixelCount(Map) / (float)(Map.Map.Width * Map.Map.Height);
			var settlementLocations = GetSettlementLocations(culture.Culture, (int)(culture.avgSettlementsPer80Square * portionAtSeaLevel));

			for (int i = settlementLocations.Count - 1; i >= 0; i--)
			{
				KingdomModel kingdom = new KingdomModel();
				kingdom.Id = settlementLocations.ValueAt(i).ToString();
				Map.Kingdoms[kingdom.Id] = kingdom;
				ExpandRegionFromSettlement(5, kingdom.settlements[0], settlementLocations.ValueAt(i));
			}
		}

		CalculateRegionValues();

		ExpandSettlements();

		BuildRoads();

		FightWars();

		foreach(var kingdom in Map.Kingdoms)
		{
			kingdom.SetNamesAndHeraldry();
		}*/
	}
	/*
	private SortedDupList<Int2> GetSettlementLocations(CultureModel culture, int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();

		for(int i = 0; i < numberOfSettlements * 200; i++)
		{
			Int2 testPos = new Int2(Random.Range(0, Map.Map.Width), Random.Range(0, Map.Map.Height));
			if (!Map.Map.Get(testPos).Terrain.HasTrait(TerrainModel.GroundTraits.Impassable) &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(Map.Map.Width, Map.Map.Height)))
			{
				regions.Insert(testPos, culture.TileAreaValue(testPos, Map));
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

			if (Map.Map.Get(regions.ValueAt(regionsIndex)).Terrain.HasTrait(TerrainInfo.GroundTraits.City) ||
				BordersCity(regions.ValueAt(regionsIndex)))
			{
				regionsIndex++;
				continue;
			}

			usedSettlements.Insert(regions.ValueAt(regionsIndex), regions.KeyAt(regionsIndex));
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
		foreach (var border in MapTextureHelpers.Terrain.GetAdjacentPoints(tile))
		{
			if (MapTextureHelpers.Terrain.Get(border).HasTrait(TerrainInfo.GroundTraits.City))
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
		TileAt(pos).TrySetRegion(settlement, startingValue - MapTextureHelpers.Terrain.Get(pos).difficulty);

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(pos, TileAt(pos).holdingStrength);
		while(frontierTiles.Count > 0)
		{
			foreach(var neighbor in GetPossibleNeighborTiles(frontierTiles.TopValue(), settlement))
			{
				float strength = frontierTiles.TopKey() - MapTextureHelpers.Terrain.Get(neighbor).difficulty;
				if(MapTextureHelpers.Terrain.Get(neighbor) == MapTextureHelpers.RealmModel.Ocean &&
					MapTextureHelpers.Terrain.Get(frontierTiles.TopValue()) != MapTextureHelpers.RealmModel.Ocean)
				{
					float startOceanDifficulty = 0.35f;
					strength = strength - startOceanDifficulty;
				}

				if (TileAt(neighbor).TrySetRegion(settlement, strength))
				{
					frontierTiles.Insert(neighbor, strength);
				}
			}
			frontierTiles.Pop();
		}
	}

	private List<Int2> GetPossibleNeighborTiles(Int2 pos, Settlement settlement)
	{
		List<Int2> goodNeighbors = new List<Int2>();
		foreach (Int2 adjacent in MapTextureHelpers.Terrain.GetAdjacentPoints(pos))
		{
			if (IsPossibleNeighbor(adjacent, settlement))
				goodNeighbors.Add(adjacent);
		}
		return goodNeighbors;
	}

	private bool IsPossibleNeighbor(Int2 neighbor, Settlement settlement)
	{
		return MapTextureHelpers.Terrain.PosInBounds(neighbor) && TileAt(neighbor).settlement != settlement;
	}

	private void CalculateRegionValues()
	{
		foreach(Int2 tile in Map.GetMapPoints())
			Map.Get(tile).settlement.kingdom.value += Map.Get(tile).settlement.kingdom.culture.GetTileValue(tile);
	}

	private void ExpandSettlements()
	{
		foreach(var reg in Kingdoms)
		{
			foreach(var sett in reg.settlements)
				sett.ExpandSettlement(reg.value);
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
		frontierTiles.Insert(startTile, 3f);

		Map2D<float> distMap = new Map2D<float>(Map.Width, Map.Height);

		while(frontierTiles.Count > 0)
		{
			distMap.Set(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach(var tile in MapTextureHelpers.Terrain.GetAdjacentPoints(currTile))
			{
				if (distMap.Get(tile) != 0 ||
				    MapTextureHelpers.Terrain.Get(tile) == MapTextureHelpers.RealmModel.Ocean)
					continue;

				if(MapTextureHelpers.Terrain.Get(tile).HasTrait(TerrainInfo.GroundTraits.City))
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

				float difficulty = MapTextureHelpers.Terrain.Get(tile).difficulty;
				if(currDifficulty - difficulty > 0)
				{
					frontierTiles.Insert(tile, currDifficulty - difficulty);
					distMap.Set(tile, currDifficulty - difficulty);
				}
			}
		}
	}

	private void BuildRoadBackFromTile(Int2 tile, Map2D<float> distMap)
	{
		if (MapTextureHelpers.Terrain.Get(tile).HasTrait(TerrainInfo.GroundTraits.Road))
			return;
		if (!MapTextureHelpers.Terrain.Get(tile).HasTrait(TerrainInfo.GroundTraits.City))
			MapTextureHelpers.Terrain.Set(tile, MapTextureHelpers.RealmModel.Road);
		Int2 maxTile = tile;
		foreach(var t in distMap.GetAdjacentPoints(tile))
		{
			if (distMap.Get(t) > distMap.Get(maxTile))
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
		frontierTiles.Insert(startTile, startDifficulty);

		Map2D<float> distMap = new Map2D<float>(Map.Width, Map.Height);

		while (frontierTiles.Count > 0)
		{
			distMap.Set(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach (var tile in MapTextureHelpers.Terrain.GetAdjacentPoints(currTile))
			{
				if (distMap.Get(tile) != 0)
					continue;

				if (MapTextureHelpers.Terrain.Get(tile).HasTrait(TerrainInfo.GroundTraits.City))
				{
					var sett = GetSettlementFromTile(tile);
					if (!hitSettlements.ContainsValue(sett))
					{
						hitSettlements.Insert(GetSettlementFromTile(tile), startDifficulty - currDifficulty);
					}
					else if (sett == settlement)
					{
						frontierTiles.Insert(tile, currDifficulty);
						distMap.Set(tile, currDifficulty);
					}
				}
				else {
					var type = MapTextureHelpers.Terrain.Get(tile);
					var currType = MapTextureHelpers.Terrain.Get(currTile);
					if ((type == MapTextureHelpers.RealmModel.Ocean && (currType == MapTextureHelpers.RealmModel.Ocean || currType.HasTrait(TerrainInfo.GroundTraits.City))) ||
					(type == MapTextureHelpers.RealmModel.River && (currType == MapTextureHelpers.RealmModel.River || currType.HasTrait(TerrainInfo.GroundTraits.City))) ||
					(type.HasTrait(TerrainInfo.GroundTraits.Road) && (currType.HasTrait(TerrainInfo.GroundTraits.Road) || currType.HasTrait(TerrainInfo.GroundTraits.City))))
					{
						float difficulty = MapTextureHelpers.Terrain.Get(tile).difficulty;
						if (currDifficulty - difficulty > 0)
						{
							frontierTiles.Insert(tile, currDifficulty - difficulty);
							distMap.Set(tile, currDifficulty - difficulty);
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
				float attackPower = kingdom.Strength() * Random.Range(.75f, 1.25f);
				float defenderPower = (closestSett.kingdom.Strength() + closestSett.GetSettlementDefensibility()) * Random.Range(.75f, 1.25f);

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
		return Map.Get(pos);
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
		return Map;
	}*/
}
