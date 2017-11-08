using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
	public MapModel Map;

	public MapModel SetUpMapGenerator(int width, int height, RealmModel Realm)
	{
		MapGeneratorApi generatorApi = new MapGeneratorApi();
		return generatorApi.GenerateMaps(width, height, Realm);
	}

	public Texture2D GetHeightMapTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (MapTileModel h in Map.Map.GetMapValues())
		{
			if (h.Height.Equals(-1))
				pixels.Add(Color.red);
			else pixels.Add(new Color(h.Height, h.Height, h.Height));
		}
		Texture2D heightMapImage = new Texture2D(Map.Map.Width, Map.Map.Height);
		heightMapImage.filterMode = FilterMode.Point;
		heightMapImage.anisoLevel = 0;
		heightMapImage.SetPixels(pixels.ToArray());
		heightMapImage.Apply();
		return heightMapImage;
	}

	public Texture2D GetTerrainTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (MapTileModel tile in Map.Map.GetMapValuesFlipped())
		{
			pixels.Add(tile.Terrain.LookupColor);
		}
		Texture2D terrainMapImage = new Texture2D(Map.Map.Width, Map.Map.Height, TextureFormat.ARGB32, true, true);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
		terrainMapImage.Apply();
		return terrainMapImage;
	}

	public int SeaLevelPixelCount()
	{
		int numTiles = 0;
		foreach (MapTileModel tile in Map.Map.GetMapValues())
		{
			if (tile.Height >= Globals.MinGroundHeight && tile.Height < Globals.MountainHeight)
				numTiles++;
		}
		return numTiles;
	}
}