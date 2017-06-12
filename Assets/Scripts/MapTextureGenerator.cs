using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

class MapTextureGenerator
{
	private class PixelPoint
	{
		public class StrengthClaim
		{
			public Color color;
			public float strength;

			public StrengthClaim(Color c, float s)
			{
				color = c;
				strength = s;
			}
		}
		public List<StrengthClaim> claims = new List<StrengthClaim>();

		public Color GetPixelColor()
		{
			float claimStrengthsSum = 0;
			for(int i = 0; i < claims.Count; i++)
			{
				claimStrengthsSum += claims[i].strength;
			}
			List<Color> strengthAdjustedColors = new List<Color>();
			for (int i = 0; i < claims.Count; i++)
			{
				strengthAdjustedColors.Add(claims[i].color * claims[i].strength/claimStrengthsSum);
			}
			Color finalColor = new Color(0, 0, 0, 1);
			foreach(Color c in strengthAdjustedColors)
			{
				finalColor += c;
			}
			return finalColor;
		}
	}

	private Map2D<PixelPoint> pixels;
	private Texture2D mapTexture;
	public MapTextureGenerator(StoredTerrainMap map, MapTextureLookup lookup)
	{
		int textureToInputScale = 20;
		setUpMapTexture(textureToInputScale, map.Width, map.Height);

		foreach(Int2 point in map.MapPixels())
		{
			AddMapChunkForTile(point.X, point.Y, textureToInputScale, map.TileAt(point), lookup);
		}

		var pixels = this.pixels.GetMapValuesFlipped();
		Color[] pixelsToSet = new Color[pixels.Count];
		for(int i = 0; i < pixels.Count; i++)
		{
			pixelsToSet[i] = pixels[i].GetPixelColor();
		}
		mapTexture.SetPixels(pixelsToSet);
	}

	private void setUpMapTexture(int scale, int width, int height)
	{
		pixels = new Map2D<PixelPoint>(width * scale, height * scale);
		foreach (var pt in pixels.GetMapPoints())
			pixels.SetPoint(pt, new PixelPoint());
		mapTexture = new Texture2D(pixels.Width, pixels.Height);
	}

	private void AddMapChunkForTile(int i, int j, int scale, TerrainTile tile, MapTextureLookup lookup)
	{
		int distanceOutToFade = scale/2;
		Texture2D tileTexture = lookup.GetTileTypeTexture(tile.tileType);
		for(int x = i*scale - distanceOutToFade; x < i*scale + scale + distanceOutToFade; x++)
		{
			for (int y = j*scale - distanceOutToFade; y < j*scale + scale + distanceOutToFade; y++)
			{
				Vector2 center = new Vector2((i * scale) + (scale / 2), (j * scale) + (scale / 2));
				float distFromCenter = (new Vector2(x, y) - center).magnitude;
				float strength = ((distanceOutToFade * 2) - Mathf.Max(0, (distFromCenter - ((scale/2) - distanceOutToFade)))) / (distanceOutToFade * 2);
				strength = Mathf.Min(Mathf.Max(0, strength), 1);

				TrySetPixel(new Int2(x, y), tileTexture.GetPixel(x, y), strength);
			}
		}
	}

	private void TrySetPixel(Int2 pos, Color c, float strength)
	{
		if(pixels.PosInBounds(pos))
			pixels.GetValueAt(pos).claims.Add(new PixelPoint.StrengthClaim(c, strength));
	}

	public Texture2D GetMapTexture()
	{
		mapTexture.Apply();
		return mapTexture;
	}
}