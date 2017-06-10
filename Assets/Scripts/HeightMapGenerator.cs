﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator
{
	float[][] map;
	private Texture2D heightMapImage;
	public const float LandHeight = 0.1f;

	public HeightMapGenerator(int width, int height)
	{
		map = new float[width][];
		for(int i = 0; i < width; i++)
		{
			map[i] = new float[height];
		}

		GenerateMountainRanges(Random.Range(5, 15));
		RandomizeCoastline();
		BlendHeightMap();

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
		int distToCoast = Random.Range(2, 20);

		Int2 currPixel = startingPixel;
		for(int k = 0; k < mountainsLength; k++)
		{
			float strength = startingStrength + Random.Range(-.2f, .2f);
			TrySetPixel(currPixel, strength, distToCoast);
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

	private bool TrySetPixel(Int2 pixel, float height, int distanceToCoast)
	{
		if (!pixelInBounds(pixel))
			return false;

		map[pixel.X][pixel.Y] = height;
		TrySpreadLandArea(pixel, distanceToCoast);
		return true;
	}

	private bool pixelInBounds(Int2 pixel)
	{
		return !(pixel.X < 0 || pixel.X >= map.Length || pixel.Y < 0 || pixel.Y >= map[0].Length);
	}

	private void TrySpreadLandArea(Int2 startPixel, int distanceToCoast)
	{
		SortedDupList<Int2> pixelsToSpreadTo = new SortedDupList<Int2>();
		pixelsToSpreadTo.Insert(distanceToCoast, startPixel);

		while (pixelsToSpreadTo.Count > 0)
		{
			Int2 currPixel = pixelsToSpreadTo.TopValue();

			if (pixelsToSpreadTo.TopKey() > 0)
			{
				List<Int2> neighbors = GetNeighborTiles(pixelsToSpreadTo.TopValue());
				foreach (Int2 neighbor in neighbors)
				{
					map[neighbor.X][neighbor.Y] = LandHeight;
					pixelsToSpreadTo.Insert(pixelsToSpreadTo.TopKey() - 1,  neighbor);
				}
			}
			pixelsToSpreadTo.Pop();
		}
	}

	private List<Int2> GetNeighborTiles(Int2 pos)
	{
		List<Int2> neighbors = new List<Int2>();
		TryAddNeighbor(pos + new Int2(1, 0), neighbors);
		TryAddNeighbor(pos + new Int2(0, 1), neighbors);
		TryAddNeighbor(pos + new Int2(0, -1), neighbors);
		TryAddNeighbor(pos + new Int2(-1, 0), neighbors);

		return neighbors;
	}

	private void TryAddNeighbor(Int2 neighborPos, List<Int2> neighbors)
	{
		if (IsPossibleNeighbor(neighborPos))
			neighbors.Add(neighborPos);
	}

	private bool IsPossibleNeighbor(Int2 neighbor)
	{
		return pixelInBounds(neighbor) && map[neighbor.X][neighbor.Y].Equals(0);
	}

	private void RandomizeCoastline()
	{
		int numPasses = 2;
		for(int i = 0; i < numPasses; i++)
		{
			RandomizeCoastlinePass();
		}
	}

	private void RandomizeCoastlinePass()
	{
		for(int i = 0; i < map.Length; i++)
		{
			for(int j = 0; j <map[0].Length; j++)
			{
				TryRandomizeCoastTile(new Int2(i, j));
			}
		}
	}

	private void TryRandomizeCoastTile(Int2 tile)
	{
		float probOfWaterfy = 0.4f;
		if(IsCoastline(tile) && !BordersMountain(tile))
		{
			if (Random.Range(0f, 1f) <= probOfWaterfy)
				map[tile.X][tile.Y] = 0f;
		}
	}

	private bool IsCoastline(Int2 tile)
	{
		foreach(Int2 neighbor in GetNeighborTiles(tile))
		{
			if (HeightAt(neighbor).Equals(0))
				return true;
		}
		return false;
	}

	private bool BordersMountain(Int2 tile)
	{
		foreach (Int2 neighbor in GetNeighborTiles(tile))
		{
			if (HeightAt(neighbor) > LandHeight)
				return true;
		}
		foreach (Int2 neighbor in GetDiagNeighborTiles(tile))
		{
			if (HeightAt(neighbor) > LandHeight)
				return true;
		}
		return false;
	}

	private List<Int2> GetDiagNeighborTiles(Int2 pos)
	{
		List<Int2> neighbors = new List<Int2>();
		TryAddNeighbor(pos + new Int2(1, 1), neighbors);
		TryAddNeighbor(pos + new Int2(-1, 1), neighbors);
		TryAddNeighbor(pos + new Int2(1, -1), neighbors);
		TryAddNeighbor(pos + new Int2(-1, -1), neighbors);

		return neighbors;
	}

	private void BlendHeightMap()
	{
		int passes = 2;
		for(int i = 0; i < passes; i++)
		{
			BlendHeightMapPass();
		}
	}

	private void BlendHeightMapPass()
	{
		for(int i = 0; i < map.Length; i++)
		{
			for(int j = 0; j < map[0].Length; j++)
			{
				float avg = NeighborAverageHeight(i, j);
				if (map[i][j] > 0 && (map[i][j] != LandHeight || avg >= LandHeight))
					map[i][j] = (map[i][j] + avg) / 2;
			}
		}
	}

	private float NeighborAverageHeight(int x, int y)
	{
		List<float> points = new List<float>();
		TryAddPoint(points, x, y - 1);
		TryAddPoint(points, x - 1, y);
		TryAddPoint(points, x + 1, y);
		TryAddPoint(points, x, y + 1);

		float average = 0f;
		foreach (var pt in points)
		{
			average += pt;
		}
		return average / points.Count;
	}

	private void TryAddPoint(List<float> points, int x, int y)
	{
		if (x >= 0 && x < map.Length - 1 && y >= 0 && y < map[0].Length - 1)
			points.Add(map[x][y]);
	}

	private float HeightAt(Int2 px)
	{
		return map[px.X][px.Y];
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}

	public float[][] GetHeightMap()
	{
		return map;
	}
}
