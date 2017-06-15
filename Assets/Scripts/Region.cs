using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Settlement
{
	public string name;
	public List<Int2> cityTiles = new List<Int2>();
	public Settlement(string n, Int2 cityTile)
	{
		cityTiles.Add(cityTile);
		name = n;
	}

	public void ExpandSettlement(float regionValue, TerrainMapGenerator terrainTiles, Map2D<RegionTile> regionsMap, Region myRegion)
	{
		terrainTiles.SetValue(cityTiles[0], new TerrainTile(TerrainTile.TileType.City));

		float valuePerNewTile = 10;
		while(cityTiles.Count < regionValue / valuePerNewTile)
		{
			var expansionTiles = GetPossibleExpnasionTiles(terrainTiles, regionsMap, myRegion);
			if (expansionTiles.Count == 0)
				break;
			cityTiles.Add(expansionTiles.TopValue());
			terrainTiles.SetValue(expansionTiles.TopValue(), new TerrainTile(TerrainTile.TileType.City));
		}
	}

	private SortedDupList<Int2> GetPossibleExpnasionTiles(TerrainMapGenerator terrainTiles, Map2D<RegionTile> regionsMap, Region myRegion)
	{
		SortedDupList<Int2> possibleExpansions = new SortedDupList<Int2>();
		foreach(Int2 cityTile in cityTiles)
		{
			foreach(Int2 neighbor in terrainTiles.GetTerrainMap().GetAdjacentPoints(cityTile))
			{
				var neighborType = terrainTiles.GetTerrainMap().GetValueAt(neighbor).tileType;
				if (!possibleExpansions.ContainsValue(neighbor) &&
					neighborType != TerrainTile.TileType.City &&
					neighborType != TerrainTile.TileType.Ocean &&
					neighborType != TerrainTile.TileType.Mountain &&
					regionsMap.GetValueAt(neighbor).region == myRegion)
				{
					possibleExpansions.Insert(terrainTiles.TileAreaFullValue(neighbor), neighbor);
				}
			}
		}
		return possibleExpansions;
	}
}

class Region
{
	public Settlement settlement;
	public Color color;
	public float value;

	public Region(string name, Int2 cityTile)
	{
		if(cityTile != null)
			settlement = new Settlement(name, cityTile);
		color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}
}

