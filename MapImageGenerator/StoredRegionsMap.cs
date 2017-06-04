using System;
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
			if (!terrainMap.TileIsOceanOrRiver(testPos) &&
				!TooCloseToExistingSettlement(testPos, regions) &&
				!TooCloseToBorder(testPos, new Int2(terrainMap.Width, terrainMap.Height)))
			{
				regions.Insert(terrainMap.TileAreaValue(testPos), testPos);
			}
		}

		return PickUsedSettlementsFromSortedList(numberOfSettlements, regions);
	}

	private List<Int2> PickUsedSettlementsFromSortedList(int numOfSettlements, SortedDupList<Int2> regions)
	{
		List<Int2> usedSettlements = new List<Int2>();
		int regionsIndex = 0;
		for (int i = 0; i < numOfSettlements; i++)
		{
			if (regionsIndex >= regions.Count)
			{
				Console.WriteLine("We ran out of regions!");
				break;
			}

			usedSettlements.Add(regions.ValueAt(regionsIndex));
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

	public Color GetTileColor(Int2 pos)
	{
		return mapTiles[pos.X][pos.Y].GetColor();
	}
}
