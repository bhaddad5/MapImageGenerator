using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTextureHelpers
{
	public static Map2D<Color> GetTerrainTexture(Map2D<MapTileModel> Map, int tileSize)
	{
		Map2D<Color> mapTex = new Map2D<Color>(Map.Width * tileSize, Map.Height * tileSize);
		foreach (Int2 point in mapTex.GetMapPoints())
		{
			Int2 mapPoint = point / tileSize;

			Color c1 = GetAdjustedColor(point, mapPoint + new Int2(-1, 1), tileSize, Map);
			Color c2 = GetAdjustedColor(point, mapPoint + new Int2(0, 1), tileSize, Map);
			Color c3 = GetAdjustedColor(point, mapPoint + new Int2(1, 1), tileSize, Map);
			Color c4 = GetAdjustedColor(point, mapPoint + new Int2(-1, 0), tileSize, Map);
			Color c5 = GetAdjustedColor(point, mapPoint + new Int2(0, 0), tileSize, Map);
			Color c6 = GetAdjustedColor(point, mapPoint + new Int2(1, 0), tileSize, Map);
			Color c7 = GetAdjustedColor(point, mapPoint + new Int2(-1, -1), tileSize, Map);
			Color c8 = GetAdjustedColor(point, mapPoint + new Int2(0, -1), tileSize, Map);
			Color c9 = GetAdjustedColor(point, mapPoint + new Int2(1, -1), tileSize, Map);

			float combinedAlpha = c1.a + c2.a + c3.a + c4.a + c5.a + c6.a + c7.a + c8.a + c9.a;

			Color c = c1 / combinedAlpha + c2 / combinedAlpha + c3 / combinedAlpha + 
				c4 / combinedAlpha + c5 / combinedAlpha + c6 / combinedAlpha + 
				c7 / combinedAlpha + c8 / combinedAlpha + c9 / combinedAlpha;

			mapTex.Set(point, c);

			//Map2D<Color> colors = Map.Get(mapPoint).Terrain().GetTerrainTexture().Colors;
			//mapTex.Set(point, colors.Get(new Int2(point.X, point.Y), true));
		}
		return mapTex;
	}

	private static Color GetAdjustedColor(Int2 myPos, Int2 centerPointToTestAgainst, int tileSize, Map2D<MapTileModel> Map)
	{
		if(!Map.PosInBounds(centerPointToTestAgainst))
			return new Color(0, 0, 0, 0);

		float dist = new Vector2(myPos.X, myPos.Y).FromTo(new Vector2(centerPointToTestAgainst.X * tileSize, centerPointToTestAgainst.Y * tileSize)).magnitude;
		float colorStrength = Mathf.Max(0, (tileSize*2f - dist)/tileSize);
		Color res = Map.Get(centerPointToTestAgainst).Terrain().GetTerrainTexture().Colors.Get(myPos, true) * colorStrength;
		if (res.r.Equals(0) && res.b.Equals(0) && res.g.Equals(0))
			res.a = 0;
		return res;
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

}