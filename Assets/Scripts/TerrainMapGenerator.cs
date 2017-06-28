using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMapGenerator
{
	Map2D<TerrainTile> map;
	Texture2D terrainMapImage;

	public TerrainMapGenerator(Map2D<float> heightMap)
	{
		map = new Map2D<TerrainTile>(heightMap.Width, heightMap.Height);

		foreach(var point in map.GetMapPoints())
		{
			if (heightMap.GetValueAt(point) < Globals.MinGroundHeight)
			{
				int numLandBorders = NumLandBorders(point, heightMap);
				if(numLandBorders >= 3)
					map.SetPoint(point, new TerrainTile(TerrainTile.TileType.River));
				else map.SetPoint(point, new TerrainTile(TerrainTile.TileType.Ocean));
			}
			else if (heightMap.GetValueAt(point) >= Globals.MountainHeight)
				map.SetPoint(point, new TerrainTile(TerrainTile.TileType.Mountain));
			else map.SetPoint(point, new TerrainTile(TerrainTile.TileType.Grass));
		}

		FillInLandTextures();
	}

	private int NumLandBorders(Int2 point, Map2D<float> heightMap)
	{
		int landBorders = 0;
		foreach(var tile in heightMap.GetAllNeighboringValues(point))
		{
			if (tile >= Globals.MinGroundHeight)
				landBorders++;
		}
		return landBorders;
	}

	public void RebuildTerrainTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (var tile in map.GetMapValuesFlipped())
		{
			pixels.Add(tile.GetTileColor());
		}
		terrainMapImage = new Texture2D(map.Width, map.Height);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
		terrainMapImage.Apply();
	}

	public void FillInLandTextures()
	{
		int numPasses = 4;
		for(int i = 0; i < numPasses; i++)
		{
			FillInLandTexturesPass();
		}
	}

	private void FillInLandTexturesPass()
	{
		foreach(var tile in map.GetMapPoints())
		{
			TryFillInTile(tile);
		}
	}

	private void TryFillInTile(Int2 tile)
	{
		float oddsOfForest = 0.01f +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) + 
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Forest);
		float oddsOfFertile = 0.01f +
			0.07f * NextToNumOfType(tile, TerrainTile.TileType.Ocean)+
			0.15f * NextToNumOfType(tile, TerrainTile.TileType.River) -
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) +
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Fertile);
		float oddsOfSwamp = 0.001f +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Ocean) +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.River) -
			1f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) +
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Swamp);

		if (GetTile(tile).tileType == TerrainTile.TileType.Grass)
		{
			if (Helpers.Odds(oddsOfFertile))
				map.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Fertile));
			else if(Helpers.Odds(oddsOfForest))
				map.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Forest));
			else if (Helpers.Odds(oddsOfSwamp))
				map.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Swamp));
		}
	}

	private int NextToNumOfType(Int2 tile, TerrainTile.TileType type)
	{
		int numNextTo = 0;
		foreach(TerrainTile t in GetNeighborTiles(tile))
		{
			if (t.tileType == type)
				numNextTo++;
		}
		return numNextTo;
	}

	private List<TerrainTile> GetNeighborTiles(Int2 pos)
	{
		List<TerrainTile> neighbors = new List<TerrainTile>();
		TryAddTile(pos + new Int2(1, 0), neighbors);
		TryAddTile(pos + new Int2(-1, 0), neighbors);
		TryAddTile(pos + new Int2(0, 1), neighbors);
		TryAddTile(pos + new Int2(0, -1), neighbors);
		return neighbors;
	}

	public void TryAddTile(Int2 pos, List<TerrainTile> neighbors)
	{
		if (pixelInBounds(pos))
			neighbors.Add(GetTile(pos));
	}

	private bool pixelInBounds(Int2 pixel)
	{
		return !(pixel.X < 0 || pixel.X >= map.Width || pixel.Y < 0 || pixel.Y >= map.Height);
	}

	public TerrainTile GetTile(Int2 pos)
	{
		return map.GetValueAt(pos);
	}

	public Map2D<TerrainTile> GetTerrainMap()
	{
		return map;
	}

	public Texture2D GetTerrainTexture()
	{
		RebuildTerrainTexture();
		return terrainMapImage;
	}

	public float TileAreaValue(Int2 pos, bool includeDiag = false)
	{
		float value = TileAt(pos).GetValue() * 2;

		float oneWaterBorderValue = 3f;
		float someWaterValue = 2f;
		float allWaterValue = -1f;

		int numWaterBorders = 0;
		foreach (TerrainTile t in map.GetAdjacentValues(pos))
		{
			if (t.tileType == TerrainTile.TileType.Ocean || t.tileType == TerrainTile.TileType.River)
				numWaterBorders++;
			value += t.GetValue();
		}

		if (includeDiag)
		{
			foreach (TerrainTile t in map.GetDiagonalValues(pos))
				value += t.GetValue() * .8f;
		}

		if (numWaterBorders == 1)
			value += oneWaterBorderValue;
		else if (numWaterBorders > 1 && numWaterBorders < 4)
			value += someWaterValue;
		else if (numWaterBorders == 4)
			value += allWaterValue;

		return value;
	}

	public bool TileInBounds(Int2 pos)
	{
		return pos.X >= 0 && pos.X < map.Width && pos.Y >= 0 && pos.Y < map.Height;
	}

	public bool TileIsType(Int2 pos, TerrainTile.TileType t)
	{
		return TileInBounds(pos) && TileAt(pos).tileType == t;
	}

	public float TileDifficulty(Int2 pos)
	{
		return TileAt(pos).GetDifficulty();
	}

	public TerrainTile TileAt(Int2 pos)
	{
		return map.GetValueAt(pos);
	}

	public int LandPixelCount()
	{
		int numTiles = 0;
		foreach (var tile in map.GetMapValues())
		{
			if (tile.tileType != TerrainTile.TileType.Ocean &&
				tile.tileType != TerrainTile.TileType.River)
				numTiles++;
		}
		return numTiles;
	}

	public List<Int2> MapPixels()
	{
		return map.GetMapPoints();
	}

	public void SetValue(Int2 pos, TerrainTile value)
	{
		map.SetPoint(pos, value);
	}
}
