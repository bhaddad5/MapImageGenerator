using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator
{
	Map2D<float> map;
	private Texture2D heightMapImage;

	public HeightMapGenerator(int width, int height)
	{
		map = new Map2D<float>(width, height);

		GenerateMountainRanges(Random.Range(5, 15));
		RandomizeCoastline();
		BlendHeightMap();

		List<Color> pixels = new List<Color>();
		foreach (float h in map.GetMapValues())
		{
			pixels.Add(new Color(h, h, h));
		}

		heightMapImage = new Texture2D(width, height);
		heightMapImage.filterMode = FilterMode.Point;
		heightMapImage.anisoLevel = 0;
		heightMapImage.SetPixels(pixels.ToArray());
	}

	private void GenerateMountainRanges(int numOfRanges)
	{
		for (int i = 0; i < numOfRanges; i++)
		{
			GenerateMountainRange();
		}
	}

	private void GenerateMountainRange()
	{
		Int2 startingPixel = new Int2(Random.Range(0, map.Width - 1), Random.Range(0, map.Height - 1));
		float startingStrength = Random.Range(.5f, .9f);
		Int2 mountainDirection = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int mountainsLength = Random.Range(4, 50);
		int distToCoast = Random.Range(2, 20);

		Int2 currPixel = startingPixel;
		for (int k = 0; k < mountainsLength; k++)
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

		map.SetPoint(pixel, height);
		TrySpreadLandArea(pixel, distanceToCoast);
		return true;
	}

	private bool pixelInBounds(Int2 pixel)
	{
		return !(pixel.X < 0 || pixel.X >= map.Width || pixel.Y < 0 || pixel.Y >= map.Height);
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
				foreach (Int2 neighbor in map.GetAdjacentPoints(pixelsToSpreadTo.TopValue()))
				{
					if(map.GetValueAt(neighbor) == 0)
					{
						map.SetPoint(neighbor, Globals.MinGroundHeight);
						pixelsToSpreadTo.Insert(pixelsToSpreadTo.TopKey() - 1, neighbor);
					}
				}
			}
			pixelsToSpreadTo.Pop();
		}
	}

	private void RandomizeCoastline()
	{
		int numPasses = 2;
		for (int i = 0; i < numPasses; i++)
		{
			RandomizeCoastlinePass();
		}
	}

	private void RandomizeCoastlinePass()
	{
		foreach(Int2 point in map.GetMapPoints())
		{
			TryRandomizeCoastTile(point);
		}
	}

	private void TryRandomizeCoastTile(Int2 tile)
	{
		float probOfWaterfy = 0.4f;
		if (IsCoastline(tile) && !BordersMountain(tile))
		{
			if (Random.Range(0f, 1f) <= probOfWaterfy)
				map.SetPoint(tile, 0f);
		}
	}

	private bool IsCoastline(Int2 tile)
	{
		foreach (Int2 neighbor in map.GetAdjacentPoints(tile))
		{
			if (HeightAt(neighbor).Equals(0))
				return true;
		}
		return false;
	}

	private bool BordersMountain(Int2 tile)
	{
		foreach (float neighbor in map.GetAllNeighboringValues(tile))
		{
			if (neighbor > Globals.MinGroundHeight)
				return true;
		}
		return false;
	}

	private void BlendHeightMap()
	{
		int passes = 2;
		for (int i = 0; i < passes; i++)
		{
			BlendHeightMapPass();
		}
	}

	private void BlendHeightMapPass()
	{
		foreach (Int2 pixle in map.GetMapPoints())
		{
			float avg = NeighborAverageHeight(pixle);
			if (map.GetValueAt(pixle) > 0 && (map.GetValueAt(pixle) != Globals.MinGroundHeight || avg >= Globals.MinGroundHeight))
				map.SetPoint(pixle, (map.GetValueAt(pixle) + avg) / 2);
		}
	}

	private float NeighborAverageHeight(Int2 pixle)
	{
		float average = 0f;
		var points = map.GetAdjacentValues(pixle);
		foreach (var pt in points)
		{
			average += pt;
		}
		return average / points.Count;
	}

	private void TryAddPoint(List<float> points, int x, int y)
	{
		if (x >= 0 && x < map.Width - 1 && y >= 0 && y < map.Height - 1)
			points.Add(map.GetValueAt(new Int2(x, y)));
	}

	private float HeightAt(Int2 px)
	{
		return map.GetValueAt(px);
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}

	public Map2D<float> GetHeightMap()
	{
		return map;
	}
}