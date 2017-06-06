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
			Vector4 claimStrengths = Vector4.zero;
			for(int i = 0; i < claims.Count; i++)
			{
				claimStrengths[i] = claims[i].strength;
			}
			List<Color> strengthAdjustedColors = new List<Color>();
			for (int i = 0; i < claims.Count; i++)
			{
				strengthAdjustedColors.Add(claims[i].color * claimStrengths.normalized[i]);
			}
			Color finalColor = new Color(0, 0, 0, 1);
			foreach(Color c in strengthAdjustedColors)
			{
				finalColor += c;
			}
			return finalColor;
		}
	}

	private PixelPoint[][] pixels;
	private Texture2D mapTexture;
	public MapTextureGenerator(StoredTerrainMap map, MapTextureLookup lookup)
	{
		int textureToInputScale = 40;
		setUpMapTexture(textureToInputScale, map.Width, map.Height);

		for(int i = 0; i < map.Width; i++)
		{
			for(int j = 0; j < map.Height; j++)
			{
				AddMapChunkForTile(i, j, textureToInputScale, map.TileAt(new Int2(i, j)), lookup);
			}
		}

		List<Color> pixelsToSet = new List<Color>();
		for(int i = 0; i < pixels.Length; i++)
		{
			for(int j = 0; j < pixels[0].Length; j++)
			{
				pixelsToSet.Add(pixels[j][i].GetPixelColor());
			}
		}
		mapTexture.SetPixels(pixelsToSet.ToArray());

		mapTexture.Apply();
		byte[] bytes = mapTexture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/MapTextures/SavedTineTerrain.png", bytes);
	}

	private void setUpMapTexture(int scale, int width, int height)
	{
		mapTexture = new Texture2D(width * scale, height * scale);
		pixels = new PixelPoint[mapTexture.width][];
		for (int i = 0; i < mapTexture.width; i++)
		{
			pixels[i] = new PixelPoint[mapTexture.height];
			for(int j = 0; j < mapTexture.height; j++)
			{
				pixels[i][j] = new PixelPoint();
			}
		}
	}

	private void AddMapChunkForTile(int i, int j, int scale, TerrainTile tile, MapTextureLookup lookup)
	{
		int distanceOutToFade = 10;
		Int2 textureCoord = new Int2(Random.Range(0, 400), Random.Range(0, 400));
		Texture2D tileTexture = lookup.GetTileTypeTexture(tile.tileType);
		for(int x = i*scale - distanceOutToFade; x < i*scale + scale + distanceOutToFade; x++)
		{
			for (int y = j*scale - distanceOutToFade; y < j*scale + scale + distanceOutToFade; y++)
			{
				Vector2 center = new Vector2((i * scale) + (scale / 2), (j * scale) + (scale / 2));
				float distFromCenter = (new Vector2(x, y) - center).magnitude;
				float strength = ((distanceOutToFade * 2) - Mathf.Max(0, (distFromCenter - ((scale/2) - distanceOutToFade)))) / (distanceOutToFade * 2);
				strength = Mathf.Min(Mathf.Max(0, strength), 1);

				TrySetPixel(x, y, tileTexture.GetPixel(textureCoord.X + x, textureCoord.Y + y), strength);
			}
		}
	}

	private void TrySetPixel(int x, int y, Color c, float strength)
	{
		if (x < 0 || x >= mapTexture.width || y < 0 || y >= mapTexture.height)
			return;

		pixels[x][y].claims.Add(new PixelPoint.StrengthClaim(c, strength));
	}

	public Texture2D GetMapTexture()
	{
		mapTexture.Apply();
		return mapTexture;
	}
}
