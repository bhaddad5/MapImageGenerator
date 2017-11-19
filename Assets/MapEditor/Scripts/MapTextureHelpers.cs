using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTextureHelpers
{
	public static Texture2D GetTerrainTexture(Map2D<MapTileModel> Map)
	{
		List<Color> pixels = new List<Color>();
		foreach (MapTileModel tile in Map.GetMapValuesFlipped())
			pixels.Add(tile.Terrain().LookupColor);
		Texture2D terrainMapImage = new Texture2D(Map.Width, Map.Height, TextureFormat.ARGB32, true, true);
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
			if (!tile.HasTrait(MapTileModel.TileTraits.Water) && !tile.HasTrait(MapTileModel.TileTraits.Mountain))
				numTiles++;
		}
		return numTiles;
	}

	public static Texture2D ColorMapToMaterial(Map2D<Color> colors)
	{
		Texture2D tex = new Texture2D(colors.Width, colors.Height);
		tex.SetPixels(colors.GetMapValuesFlipped().ToArray());
		tex.Apply();
		return tex;
	}
}