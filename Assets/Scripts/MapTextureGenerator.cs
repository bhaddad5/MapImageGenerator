﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MapTextureGenerator
{
	private Texture2D mapTexture;
	public MapTextureGenerator(StoredTerrainMap map, MapTextureLookup lookup)
	{
		int textureToInputScale = 50;
		setUpMapTexture(textureToInputScale, map.Width, map.Height);

		for(int i = 0; i < map.Width; i++)
		{
			for(int j = 0; j < map.Height; j++)
			{
				AddMapChunkForTile(i, j, textureToInputScale, map.TileAt(new Int2(i, j)), lookup);
			}
		}

		Debug.Log(mapTexture.GetPixel(0, 0));
		Debug.Log(mapTexture.GetPixel(879, 276));
	}

	private void setUpMapTexture(int scale, int width, int height)
	{
		mapTexture = new Texture2D(width * scale, height * scale);
		Color[] colors = mapTexture.GetPixels();
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i] = new Color(0, 0, 0, 0);
		}
		mapTexture.SetPixels(colors);
	}

	private void AddMapChunkForTile(int i, int j, int scale, TerrainTile tile, MapTextureLookup lookup)
	{
		float percentOutToFade = 0.15f;
		int distanceOutToFade = (int)(scale * percentOutToFade);
		Int2 textureCoord = new Int2(Random.Range(0, 400), Random.Range(0, 400));
		Texture2D tileTexture = lookup.GetTileTypeTexture(tile.tileType);
		for(int x = i*scale - distanceOutToFade; x < i*scale + scale + distanceOutToFade; x++)
		{
			for (int y = j*scale - distanceOutToFade; y < j*scale + scale + distanceOutToFade; y++)
			{
				float strength = .5f;
				TrySetPixel(x, y, tileTexture.GetPixel(textureCoord.X + x, textureCoord.Y + y), strength);
			}
		}
	}

	private void TrySetPixel(int x, int y, Color c, float strength)
	{
		if (x < 0 || x >= mapTexture.width || y < 0 || y > mapTexture.height)
			return;

		if (mapTexture.GetPixel(x, y).a == 0)
		{
			mapTexture.SetPixel(x, y, c);
		}
		else {
			Color blended = mapTexture.GetPixel(x, y)*strength + c*(1 - strength);
			mapTexture.SetPixel(x, y, blended);
		}
	}

	public Texture2D GetMapTexture()
	{
		mapTexture.Apply();
		return mapTexture;
	}
}
