﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

class StoredRegionsMap
{
	List<List<RegionTile>> mapTiles = new List<List<RegionTile>>();
	public int Width { get { return mapTiles.Count; } }
	public int Height { get { return mapTiles[0].Count; } }

	public StoredRegionsMap(StoredTerrainMap terrainMap, int numberOfSettlements)
	{
		var settlementLocations = GetSettlementLocations(terrainMap, numberOfSettlements);

		Region oceanRegion = new Region("Ocean", 0f);

		for (int i = 0; i < terrainMap.Width; i++)
		{
			mapTiles.Insert(i, new List<RegionTile>());
			for (int j = 0; j < terrainMap.Height; j++)
			{
				mapTiles[i].Insert(j, new RegionTile(oceanRegion));
			}
		}

		foreach(Int2 loc in settlementLocations)
		{
			mapTiles[loc.X][loc.Y].SetIsSettlement(true);
		}
	}

	private List<Int2> GetSettlementLocations(StoredTerrainMap terrainMap, int numberOfSettlements)
	{
		SortedDupList<Int2> regions = new SortedDupList<Int2>();
		Random r = new Random();

		for(int i = 0; i < numberOfSettlements * 10; i++)
		{
			Int2 testPos = new Int2(r.Next(0, terrainMap.Width), r.Next(terrainMap.Height));
			if (!terrainMap.TileIsOceanOrRiver(testPos) && !TooCloseToExistingSettlement(testPos, regions))
			{
				regions.Insert(terrainMap.TileAreaValue(testPos), testPos);
			}
		}

		List<Int2> usedSettlements = new List<Int2>();
		int regionsIndex = 0;
		bool past50Percent = false;
		bool past75Percent = false;
		for (int i = 0; i < numberOfSettlements; i++)
		{
			if(regionsIndex >= regions.Count)
			{
				Console.WriteLine("We ran out of regions!");
				break;
			}

			usedSettlements.Add(regions.ValueAt(regionsIndex));
			if (!past50Percent && i > numberOfSettlements * .5f)
			{
				regionsIndex = (int) (regions.Count * .5f);
				past50Percent = true;
			}
			else if(!past75Percent && i > numberOfSettlements * .75)
			{
				regionsIndex = (int)(regions.Count * .25f);
				past75Percent = true;
			}

			regionsIndex++;
		}
		return usedSettlements;
	}

	private bool TooCloseToExistingSettlement(Int2 pos, SortedDupList<Int2> existingSettlements)
	{
		int minDist = 3;
		for(int i = 0; i < existingSettlements.Count; i++)
		{
			Int2 existing = existingSettlements.ValueAt(i);
			if (pos.X > existing.X - minDist && pos.X < existing.X + minDist &&
				pos.Y > existing.Y - minDist && pos.Y < existing.Y + minDist)
				return true;
		}
		return false;
	}

	public Color GetTileColor(Int2 pos)
	{
		return mapTiles[pos.X][pos.Y].GetColor();
	}
}
