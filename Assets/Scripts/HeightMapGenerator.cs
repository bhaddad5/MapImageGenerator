using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator
{
	float[][] map;
	private Texture2D heightMapImage;

	public HeightMapGenerator(int width, int height)
	{
		map = new float[width][];
		for(int i = 0; i < width; i++)
		{
			map[i] = new float[height];
		}

		GenerateMountainRanges();

		List<Color> pixels = new List<Color>();
		foreach (float[] column in map)
		{
			foreach(float h in column)
			{
				pixels.Add(new Color(h, h, h));
			}
		}

		heightMapImage = new Texture2D(width, height);
		heightMapImage.filterMode = FilterMode.Point;
		heightMapImage.anisoLevel = 0;
		heightMapImage.SetPixels(pixels.ToArray());		
	}

	private void GenerateMountainRanges()
	{
		Int2 startingPixel = new Int2(Random.Range(0, map.Length - 1), Random.Range(0, map[0].Length - 1));
		float startingStrength = Random.Range(.5f, .9f);
		Int2 mountainDirection = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int mountainsLength = Random.Range(1, 20);

		for(int k = 0; k < mountainsLength; k++)
		{
			float strength = startingStrength + Random.Range(-.2f, .2f);
			int x = (int)(startingPixel.X + mountainDirection.X*k);
			int y = (int)(startingPixel.Y + mountainDirection.Y*k);
			if (!TrySetPixel(x, y, strength))
				break;
		}
	}

	private bool TrySetPixel(int x, int y, float height)
	{
		if (x < 0 || x > map.Length || y < 0 || y > map[0].Length || map[x][y] > 0)
			return false;

		map[x][y] = height;
		return true;
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}
}
