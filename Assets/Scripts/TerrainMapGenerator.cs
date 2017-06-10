using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMapGenerator
{
	TerrainTile[][] map;
	Texture2D terrainMapImage;

	public float mountainHeight = 0.3f;

	public TerrainMapGenerator(float[][] heightMap, float landHeight)
	{
		map = new TerrainTile[heightMap.Length][];
		for(int i = 0; i < heightMap.Length; i++)
		{
			map[i] = new TerrainTile[heightMap[0].Length];
			for(int j = 0; j < heightMap[0].Length; j++)
			{
				if(heightMap[i][j] < landHeight)
					map[i][j] = new TerrainTile(TerrainTile.TileType.Ocean);
				else if (heightMap[i][j] >= mountainHeight)
					map[i][j] = new TerrainTile(TerrainTile.TileType.Mountain);
				else map[i][j] = new TerrainTile(TerrainTile.TileType.Grass);
			}
		}

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
