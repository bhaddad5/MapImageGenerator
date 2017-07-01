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

	public string name;
	public Texture2D heraldry;
	public List<Int2> cityTiles = new List<Int2>();
	public Settlement(string n, Int2 cityTile)
	{
		cityTiles.Add(cityTile);
		name = n;
	}

	public void ExpandSettlement(float regionValue, TerrainMapGenerator terrainTiles, Map2D<RegionTile> regionsMap, Kingdom myRegion)
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

	private SortedDupList<Int2> GetPossibleExpnasionTiles(TerrainMapGenerator terrainTiles, Map2D<RegionTile> regionsMap, Kingdom myRegion)
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
					neighborType != TerrainTile.TileType.River &&
					neighborType != TerrainTile.TileType.Mountain &&
					regionsMap.GetValueAt(neighbor).region == myRegion)
				{
					possibleExpansions.Insert(terrainTiles.TileAreaValue(neighbor, true), neighbor);
				}
			}
		}
		return possibleExpansions;
	}

	public List<Settlement.CityTrait> GetCityTraits(Map2D<TerrainTile> regionsMap)
	{
		List<TerrainTile.TileType> neighboringTerrainTypes = new List<TerrainTile.TileType>();
		foreach(Int2 tile in cityTiles)
		{
			foreach(var neighbor in regionsMap.GetAdjacentValues(tile))
			{
				neighboringTerrainTypes.Add(neighbor.tileType);
			}
		}

		List<Settlement.CityTrait> traits = new List<CityTrait>();
		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Mountain))
			traits.Add(CityTrait.Mountains);
		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Forest))
			traits.Add(CityTrait.Forest);
		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Fertile))
			traits.Add(CityTrait.Fertile);


		if (neighboringTerrainTypes.Contains(TerrainTile.TileType.Ocean))
			traits.Add(CityTrait.Port);
		else if (neighboringTerrainTypes.Contains(TerrainTile.TileType.River))
			traits.Add(CityTrait.River);
		else traits.Add(CityTrait.Landlocked);

		if (cityTiles.Count <= 2)
			traits.Add(CityTrait.Small);
		else if (cityTiles.Count <= 4)
			traits.Add(CityTrait.Medium);
		else traits.Add(CityTrait.Large);

		return traits;
	}
}

public class Kingdom
{
	public Settlement settlement;
	public Color mainColor;
	public Color secondaryColor;
	public Color tertiaryColor;
	public float value;

	public Kingdom(string name, Int2 cityTile)
	{
		if(cityTile != null)
			settlement = new Settlement(name, cityTile);
		mainColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

		secondaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
		while(Vector3.Magnitude(new Vector3(mainColor.r, mainColor.g, mainColor.b) - new Vector3(secondaryColor.r, secondaryColor.g, secondaryColor.b)) < 0.1f)
			secondaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

		tertiaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
		while (Vector3.Magnitude(new Vector3(mainColor.r, mainColor.g, mainColor.b) - new Vector3(tertiaryColor.r, tertiaryColor.g, tertiaryColor.b)) < 0.1f &&
			Vector3.Magnitude(new Vector3(secondaryColor.r, secondaryColor.g, secondaryColor.b) - new Vector3(tertiaryColor.r, tertiaryColor.g, tertiaryColor.b)) < 0.1f)
			tertiaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}
}

