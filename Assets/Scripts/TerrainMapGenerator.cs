using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMapGenerator
{
	TerrainTile[][] map;
	Texture2D terrainMapImage;

	public float mountainHeight = 0.3f;

	public TerrainMapGenerator(float[][] heightMap)
	{
		map = new TerrainTile[heightMap.Length][];
		for(int i = 0; i < heightMap.Length; i++)
		{
			map[i] = new TerrainTile[heightMap[0].Length];
			for(int j = 0; j < heightMap[0].Length; j++)
			{
				if(heightMap[i][j] < Globals.MinGroundHeight)
					map[i][j] = new TerrainTile(TerrainTile.TileType.Ocean);
				else if (heightMap[i][j] >= mountainHeight)
					map[i][j] = new TerrainTile(TerrainTile.TileType.Mountain);
				else map[i][j] = new TerrainTile(TerrainTile.TileType.Grass);
			}
		}

		FillInLandTextures();

		List<Color> pixels = new List<Color>();
		foreach (TerrainTile[] column in map)
		{
			foreach (TerrainTile t in column)
			{
				pixels.Add(t.GetTileColor());
			}
		}
		terrainMapImage = new Texture2D(map.Length, map[0].Length);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
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
		for(int i = 0; i < map.Length; i++)
		{
			for(int j = 0; j < map[0].Length; j++)
			{
				TryFillInTile(new Int2(i, j));
			}
		}
	}

	private void TryFillInTile(Int2 tile)
	{
		float oddsOfForest = 0.01f +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) + 
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Forest);
		float oddsOfFertile = 0.01f +
			0.1f * NextToNumOfType(tile, TerrainTile.TileType.Ocean) -
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) +
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Fertile);
		float oddsOfSwamp = 0.001f +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Ocean) -
			1f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) +
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Swamp);

		if (GetTile(tile).tileType == TerrainTile.TileType.Grass)
		{
			float f = Random.Range(0, 1f);
			if (f <= oddsOfFertile)
				map[tile.X][tile.Y] = new TerrainTile(TerrainTile.TileType.Fertile);
			else if(f - oddsOfFertile <= oddsOfForest)
				map[tile.X][tile.Y] = new TerrainTile(TerrainTile.TileType.Forest);
			else if (f - (oddsOfFertile + oddsOfForest) <= oddsOfSwamp)
				map[tile.X][tile.Y] = new TerrainTile(TerrainTile.TileType.Swamp);
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
		return !(pixel.X < 0 || pixel.X >= map.Length || pixel.Y < 0 || pixel.Y >= map[0].Length);
	}

	public TerrainTile GetTile(Int2 pos)
	{
		return map[pos.X][pos.Y];
	}

	public TerrainTile[][] GetTerrainMap()
	{
		return map;
	}

	public Texture2D GetTerrainTexture()
	{
		terrainMapImage.Apply();
		return terrainMapImage;
	}
}
