﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MapTextureHelpers
{
	public static IEnumerator GetTerrainTexture(Map2D<MapTileModel> Map, int tileSize, Map2D<Color> mapTex, TextMeshProUGUI displayText)
	{
		int count = 0;
		int displayedCount = 0;
		foreach (Int2 point in mapTex.GetMapPoints())
		{
			Int2 mapPoint = (point - new Int2(tileSize/2, tileSize/2)) / tileSize;
			Vector2 mapFraction = new Vector2(((point.X - mapPoint.X * tileSize - tileSize / 2))/(float) tileSize, (point.Y - mapPoint.Y * tileSize - tileSize / 2) / (float)tileSize);
			Color interp = (1 - mapFraction.x) *
			               ((1 - mapFraction.y) * GetColor(point, mapPoint, Map) +
			               mapFraction.y * GetColor(point, mapPoint + new Int2(0, 1), Map)) +
						   mapFraction.x *
						   ((1 - mapFraction.y) * GetColor(point, mapPoint + new Int2(1, 0), Map) +
						   mapFraction.y * GetColor(point, mapPoint + new Int2(1, 1), Map));
			interp = interp / interp.a;

			mapTex.Set(point, interp);

			if (count >= (mapTex.Size / 10) * displayedCount)
			{
				displayedCount++;
				displayText.text = "Displaying Lands " + (displayedCount * 10) + "%";
				yield return new WaitForEndOfFrame();
			}
			count++;
		}
	}

	private static Color GetColor(Int2 texPos, Int2 mapPos, Map2D<MapTileModel> Map)
	{
		if (mapPos.X > Map.Width - 1)
			mapPos.X = Map.Width - 1;
		if (mapPos.Y > Map.Height - 1)
			mapPos.Y = Map.Height - 1;

		if (!Map.PosInBounds(mapPos))
			return new Color(1, .4f, .7f, 1);
		Color res = Map.Get(mapPos).Terrain().Texture.GetTexture().Get(texPos, true);
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
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.Apply();
		return tex;
	}
}