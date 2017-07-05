using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
	public static Map2D<float> Heights;
	public static Map2D<GroundTypes.Type> Terrain;
	private Texture2D heightMapImage;
	private Texture2D terrainMapImage;

	public MapGenerator(int width, int height, IMapGenerator generator)
	{
		var maps = generator.GenerateMaps(width, height);
		Heights = maps.heights;
		Terrain = maps.terrain;

		List<Color> pixels = new List<Color>();
		foreach (float h in Heights.GetMapValues())
		{
			if (h.Equals(-1))
				pixels.Add(Color.red);
			else pixels.Add(new Color(h, h, h));
		}

		heightMapImage = new Texture2D(width, height);
		heightMapImage.filterMode = FilterMode.Point;
		heightMapImage.anisoLevel = 0;
		heightMapImage.SetPixels(pixels.ToArray());
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}

	public void RebuildTerrainTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (var tile in Terrain.GetMapValuesFlipped())
		{
			pixels.Add(GroundTypes.tileColors[tile]);
		}
		terrainMapImage = new Texture2D(Terrain.Width, Terrain.Height, TextureFormat.ARGB32, true, true);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
		terrainMapImage.Apply();
	}

	public Texture2D GetTerrainTexture()
	{
		RebuildTerrainTexture();
		return terrainMapImage;
	}

	public int LandPixelCount()
	{
		int numTiles = 0;
		foreach (var tile in Terrain.GetMapValues())
		{
			if (tile != GroundTypes.Type.Ocean &&
			    tile != GroundTypes.Type.River)
				numTiles++;
		}
		return numTiles;
	}
}