using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
	public static Map2D<float> Heights;
	public static Map2D<TerrainInfo> Terrain;
	public static RealmCreationInfo RealmCreationInfo;

	public static void SetUpMapGenerator(int width, int height, RealmCreationInfo env)
	{
		RealmCreationInfo = env;
		MapGeneratorApi generatorApi = new MapGeneratorApi();
		var maps = generatorApi.GenerateMaps(width, height, env);
		Heights = maps.heights;
		Terrain = maps.terrain;
	}

	public static Texture2D GetHeightMapTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (float h in Heights.GetMapValues())
		{
			if (h.Equals(-1))
				pixels.Add(Color.red);
			else pixels.Add(new Color(h, h, h));
		}
		Texture2D heightMapImage = new Texture2D(Heights.Width, Heights.Height);
		heightMapImage.filterMode = FilterMode.Point;
		heightMapImage.anisoLevel = 0;
		heightMapImage.SetPixels(pixels.ToArray());
		heightMapImage.Apply();
		return heightMapImage;
	}

	public static Texture2D GetTerrainTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (var tile in Terrain.GetMapValuesFlipped())
		{
			pixels.Add(tile.lookupColor);
		}
		Texture2D terrainMapImage = new Texture2D(Terrain.Width, Terrain.Height, TextureFormat.ARGB32, true, true);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
		terrainMapImage.Apply();
		return terrainMapImage;
	}

	public static int SeaLevelPixelCount()
	{
		int numTiles = 0;
		foreach (var tile in Heights.GetMapValues())
		{
			if (tile >= Globals.MinGroundHeight && tile < Globals.MountainHeight)
				numTiles++;
		}
		return numTiles;
	}
}

public class Map
{
	public Map2D<float> heights;
	public Map2D<TerrainInfo> terrain;

	public Map(Map2D<float> h, Map2D<TerrainInfo> ter)
	{
		heights = h;
		terrain = ter;
	}
}