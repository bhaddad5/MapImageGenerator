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
			if(point.X == 70 && point.Y == 70)
				Debug.Log("HIT!");

			Int2 mapPoint = point / tileSize;
			Vector2 mapFraction = new Vector2(((point.X - mapPoint.X * tileSize))/(float) tileSize, (point.Y - mapPoint.Y * tileSize) / (float)tileSize);

			Color interp = (1 - mapFraction.x) *
			               ((1 - mapFraction.y) * GetColor(point, mapPoint, Map)) +
			               mapFraction.y * GetColor(point, mapPoint + new Int2(0, 1), Map) +
						   mapFraction.x *
						   ((1 - mapFraction.y) * GetColor(point, mapPoint + new Int2(1, 0), Map)) +
						   mapFraction.y * GetColor(point, mapPoint + new Int2(1, 1), Map);

			interp = interp / interp.a;

			/*interpolatedValue = (1 - fractionX) *
			                    ((1 - fractionY) * data[integerX, integerY] +
			                     fractionY * data[integerX, integerY + 1]) +
			                    fractionX *
			                    ((1 - fractionY) * data[integerX + 1, integerY] +
			                     fractionY * data[integerX + 1, integerY + 1]);*/

			mapTex.Set(point, interp);

			//Map2D<Color> colors = Map.Get(mapPoint).Terrain().GetTerrainTexture().Colors;
			//mapTex.Set(point, colors.Get(new Int2(point.X, point.Y), true));
		}
		return mapTex;
	}

	private static Color GetColor(Int2 texPos, Int2 mapPos, Map2D<MapTileModel> Map)
	{
		if (!Map.PosInBounds(mapPos))
			return new Color(1, .4f, .7f, 1);
		Color res = Map.Get(mapPos).Terrain().GetTerrainTexture().Colors.Get(texPos, true);
		return res;
	}

	/*private static Color GetAdjustedColor(Int2 myPos, Int2 centerPointToTestAgainst, int tileSize, Map2D<MapTileModel> Map)
	{
		if(!Map.PosInBounds(centerPointToTestAgainst))
			return new Color(0, 0, 0, 0);

		float dist = new Vector2(myPos.X, myPos.Y).FromTo(new Vector2(centerPointToTestAgainst.X * tileSize, centerPointToTestAgainst.Y * tileSize)).magnitude;
		float colorStrength = Mathf.Max(0, (tileSize*2f - dist)/tileSize);
		Color res = Map.Get(centerPointToTestAgainst).Terrain().GetTerrainTexture().Colors.Get(myPos, true) * colorStrength;
		if (res.r.Equals(0) && res.b.Equals(0) && res.g.Equals(0))
			res.a = 0;
		return res;
	}*/

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