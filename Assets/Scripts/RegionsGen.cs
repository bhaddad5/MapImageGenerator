using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class RegionsGen
{
	public static Map2D<RegionTile> Map;
	public static List<Kingdom> Kingdoms = new List<Kingdom>();
	public int Width { get { return Map.Width; } }
	public int Height { get { return Map.Height; } }

	public RegionsGen(List<MapBuilder.CulturePrevelance> cultures)
	{
		StartFillMap();

		foreach(var culture in cultures)
		{
			var settlementLocations = GetSettlementLocations(culture.culture, culture.numSettlements);

			for (int i = settlementLocations.Count - 1; i >= 0; i--)
			{
				MapGenerator.Terrain.Set(settlementLocations.ValueAt(i), GroundTypes.Type.City);
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
			kingdom.SetNamesAndHeraldry();
		}
	}

	private void PrintStrengths()
	{
		float humanTotal = 0;
		float orcTotal = 0;
		float DwarfTotal = 0;
		foreach (var Kingdom in Kingdoms)
		{
			if (Kingdom.culture == CultureDefinitions.Anglo)
				humanTotal += Kingdom.Strength();
			if (Kingdom.culture == CultureDefinitions.Dwarf)
				DwarfTotal += Kingdom.Strength();
			if (Kingdom.culture == CultureDefinitions.Orc)
				orcTotal += Kingdom.Strength();
		}
		Debug.Log(humanTotal + ", " + DwarfTotal + " , " + orcTotal);
	}

	private void StartFillMap()
	{
		Kingdom noMansLand = new Kingdom(CultureDefinitions.Anglo, new Int2(0, 0));
		Map = new Map2D<RegionTile>(MapGenerator.Terrain.Width, MapGenerator.Terrain.Height);
		foreach(var pixel in Map.GetMapPoints())
		{
			Map.Set(pixel, new RegionTile(noMansLand.settlements[0]));
		}
	}

	private void EndFillMap()
	{
		Kingdom Ocean = new Kingdom(CultureDefinitions.Anglo, new Int2(0, 0));
		foreach (var pixel in Map.GetMapPoints())
		{
			if(MapGenerator.Terrain.Get(pixel) == GroundTypes.Type.Ocean ||
			   MapGenerator.Terrain.Get(pixel) == GroundTypes.Type.River)
				Map.Set(pixel, new RegionTile(Ocean.settlements[0]));
		}
	}

	private SortedDupList<Int2> GetSettlementLocations(Culture culture, int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();

		for(int i = 0; i < numberOfSettlements * 200; i++)
		{
			Int2 testPos = new Int2(UnityEngine.Random.Range(0, MapGenerator.Terrain.Width), UnityEngine.Random.Range(0, MapGenerator.Terrain.Height));
			if (GroundTypes.ViableCityTerrain(MapGenerator.Terrain.Get(testPos)) &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(MapGenerator.Terrain.Width, MapGenerator.Terrain.Height)))
			{
				regions.Insert(testPos, culture.TileAreaValue(testPos));
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

			if (MapGenerator.Terrain.Get(regions.ValueAt(regionsIndex)) == GroundTypes.Type.City ||
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
		foreach (var border in MapGenerator.Terrain.GetAdjacentPoints(tile))
		{
			if (MapGenerator.Terrain.Get(border) == GroundTypes.Type.City)
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
		TileAt(pos).TrySetRegion(settlement, startingValue - settlement.kingdom.culture.GetTileDifficulty(pos));

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(pos, TileAt(pos).holdingStrength);
		while(frontierTiles.Count > 0)
		{
			foreach(var neighbor in GetPossibleNeighborTiles(frontierTiles.TopValue(), settlement))
			{
				float strength = frontierTiles.TopKey() - settlement.kingdom.culture.GetTileDifficulty(neighbor);
				if(MapGenerator.Terrain.Get(neighbor) == GroundTypes.Type.Ocean &&
					MapGenerator.Terrain.Get(frontierTiles.TopValue()) != GroundTypes.Type.Ocean)
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
		foreach (Int2 adjacent in MapGenerator.Terrain.GetAdjacentPoints(pos))
		{
			if (IsPossibleNeighbor(adjacent, settlement))
				goodNeighbors.Add(adjacent);
		}
		return goodNeighbors;
	}

	private bool IsPossibleNeighbor(Int2 neighbor, Settlement settlement)
	{
		return MapGenerator.Terrain.PosInBounds(neighbor) && TileAt(neighbor).settlement != settlement;
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
			foreach(var tile in MapGenerator.Terrain.GetAdjacentPoints(currTile))
			{
				if (distMap.Get(tile) != 0 ||
				    MapGenerator.Terrain.Get(tile) == GroundTypes.Type.Ocean)
					continue;

				if(MapGenerator.Terrain.Get(tile) == GroundTypes.Type.City)
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

				float difficulty = settlement.kingdom.culture.GetTileDifficulty(tile);
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
		if (MapGenerator.Terrain.Get(tile) == GroundTypes.Type.Road)
			return;
		if (MapGenerator.Terrain.Get(tile) != GroundTypes.Type.City)
			MapGenerator.Terrain.Set(tile, GroundTypes.Type.Road);
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
			foreach (var tile in MapGenerator.Terrain.GetAdjacentPoints(currTile))
			{
				if (distMap.Get(tile) != 0)
					continue;

				if (MapGenerator.Terrain.Get(tile) == GroundTypes.Type.City)
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
					var type = MapGenerator.Terrain.Get(tile);
					var currType = MapGenerator.Terrain.Get(currTile);
					if ((type == GroundTypes.Type.Ocean && (currType == GroundTypes.Type.Ocean || currType == GroundTypes.Type.City)) ||
					(type == GroundTypes.Type.River && (currType == GroundTypes.Type.River || currType == GroundTypes.Type.City)) ||
					(type == GroundTypes.Type.Road && (currType == GroundTypes.Type.Road || currType == GroundTypes.Type.City)))
					{
						float difficulty = settlement.kingdom.culture.GetTileDifficulty(tile);
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
				float attackPower = kingdom.Strength() * Random.Range(.75f, 1.25f) * kingdom.culture.AttackMultiplier;
				float defenderPower = (closestSett.kingdom.Strength() + closestSett.GetSettlementDefensibility()) * Random.Range(.75f, 1.25f) * closestSett.kingdom.culture.DefenseMultiplier;

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
	}
}
