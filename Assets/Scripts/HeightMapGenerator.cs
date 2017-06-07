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

		GenerateMountainRanges(Random.Range(5, 15));

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

	private void GenerateMountainRanges(int numOfRanges)
	{
		for(int i = 0; i < numOfRanges; i++)
		{
			GenerateMountainRange();
		}
	}

	private void GenerateMountainRange()
	{
		Int2 startingPixel = new Int2(Random.Range(0, map.Length - 1), Random.Range(0, map[0].Length - 1));
		float startingStrength = Random.Range(.5f, .9f);
		Int2 mountainDirection = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int mountainsLength = Random.Range(4, 50);

		Int2 currPixel = startingPixel;
		for(int k = 0; k < mountainsLength; k++)
		{
			float strength = startingStrength + Random.Range(-.2f, .2f);
			TrySetPixel(currPixel, strength);
			currPixel = TryGetNextMountainPixel(currPixel, mountainDirection); ;
		}
	}

	private Int2 TryGetNextMountainPixel(Int2 currPoint, Int2 direction)
	{
		Int2[] potentials = new Int2[10];
		potentials[0] = currPoint + nextDirection(direction, 0);
		potentials[1] = currPoint + nextDirection(direction, 0);
		potentials[2] = currPoint + nextDirection(direction, 0);
		potentials[3] = currPoint + nextDirection(direction, 0);
		potentials[4] = currPoint + nextDirection(direction, 1);
		potentials[5] = currPoint + nextDirection(direction, 1);
		potentials[6] = currPoint + nextDirection(direction, 2);
		potentials[7] = currPoint + nextDirection(direction, -1);
		potentials[8] = currPoint + nextDirection(direction, -1);
		potentials[9] = currPoint + nextDirection(direction, -2);

		return potentials[Random.Range(0, 9)];
	}

	private Int2 nextDirection(Int2 dir, int forwardOrBack)
	{
		List<Int2> dirs = new List<Int2> { new Int2(-1, -1), new Int2(-1, 0), new Int2(-1, 1), new Int2(0, 1), new Int2(1, 1), new Int2(1, 0), new Int2(1, -1), new Int2(0, -1), };
		int index = dirs.IndexOf(dir) + forwardOrBack;
		if (index >= dirs.Count)
			index = index - dirs.Count;
		if (index < 0)
			index = dirs.Count + index;
		return dirs[index];
	}

	private bool TrySetPixel(Int2 pixel, float height)
	{
		if (pixel.X < 0 || pixel.X >= map.Length || pixel.Y < 0 || pixel.Y >= map[0].Length)
			return false;

		map[pixel.X][pixel.Y] = height;
		return true;
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}
}
