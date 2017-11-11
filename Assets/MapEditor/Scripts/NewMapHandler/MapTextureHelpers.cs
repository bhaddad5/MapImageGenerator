﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTextureHelpers
{
	public static Texture2D GetTerrainTexture(MapModel Map)
	{
		List<Color> pixels = new List<Color>();
		foreach (MapTileModel tile in Map.Map.GetMapValuesFlipped())
			pixels.Add(tile.Terrain().LookupColor);
		Texture2D terrainMapImage = new Texture2D(Map.Map.Width, Map.Map.Height, TextureFormat.ARGB32, true, true);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
		terrainMapImage.Apply();
		return terrainMapImage;
	}

	public static int SeaLevelPixelCount(MapModel Map)
	{
		int numTiles = 0;
		foreach (MapTileModel tile in Map.Map.GetMapValues())
		{
			if (!tile.Terrain().Traits.Contains("Water") && !tile.Terrain().Traits.Contains("Mountain"))
				numTiles++;
		}
		return numTiles;
	}
}