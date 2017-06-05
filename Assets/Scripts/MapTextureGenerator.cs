using System.Collections;
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
		if(i == 55 && j == 55)
		{
			Debug.Log("hit");
		}
		Int2 textureCoord = new Int2(Random.Range(0, 400), Random.Range(0, 400));
		Texture2D tileTexture = lookup.GetTileTypeTexture(tile.tileType);
		for(int x = i*scale - scale/4; x < i*scale + scale + scale/4; x++)
		{
			for (int y = j*scale - scale / 4; y < j*scale + scale + scale / 4; y++)
			{
				TrySetPixel(x, y, tileTexture.GetPixel(textureCoord.X + x, textureCoord.Y + y));
			}
		}
	}

	private void TrySetPixel(int x, int y, Color c)
	{
		if (x < 0 || x >= mapTexture.width || y < 0 || y > mapTexture.height)
			return;

		if (mapTexture.GetPixel(x, y).a == 0)
		{
			mapTexture.SetPixel(x, y, c);
		}
		else {
			Color blended = (mapTexture.GetPixel(x, y) + c) / 2;
			mapTexture.SetPixel(x, y, blended);
		}
	}

	public Texture2D GetMapTexture()
	{
		mapTexture.Apply();
		return mapTexture;
	}
}
