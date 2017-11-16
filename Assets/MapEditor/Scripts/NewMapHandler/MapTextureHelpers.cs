using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTextureHelpers
{
	public static Texture2D GetTerrainTexture(MapModel Map, Int2 startingPoint, int size)
	{
		List<Color> pixels = new List<Color>();
		var block = Map.Map.FlipMap().GetMapBlock(new Int2(startingPoint.Y * size, startingPoint.X * size), size, size);
		foreach (MapTileModel tile in block.GetMapValues())
			pixels.Add(tile.Terrain().LookupColor);

		int sizeX = Math.Min(size, Map.Map.Width - startingPoint.X);
		int sizeY = Math.Min(size, Map.Map.Height - startingPoint.Y);

		Texture2D terrainMapImage = new Texture2D(sizeX, sizeY, TextureFormat.ARGB32, true, true);
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