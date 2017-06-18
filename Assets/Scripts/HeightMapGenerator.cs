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

		int pixelsPerRange = 1000;
		int avgNumOfRanges = (width * height) / pixelsPerRange;

		GenerateMountainRanges(Random.Range(avgNumOfRanges/2, avgNumOfRanges + avgNumOfRanges/2));
		RandomizeCoastline();
		BlendHeightMap();
		CreateRivers();

		List<Color> pixels = new List<Color>();
		foreach (float h in map.GetMapValues())
		{
			if (h.Equals(-1))
				pixels.Add(Color.red);
			else pixels.Add(new Color(h, h, h));
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
		float startingStrength = Random.Range(.7f, 1f);
		Int2 mountainDirection = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int mountainsLength = Random.Range(4, 50);
		int distToCoast = Random.Range(5, 40);

		Int2 currPixel = startingPixel;
		for (int k = 0; k < mountainsLength; k++)
		{
			float strength = startingStrength + Random.Range(-.2f, .2f);
			TryMountainCenterPixel(currPixel, strength, distToCoast);
			currPixel = TryGetNextMountainPixel(currPixel, mountainDirection); ;
		}
	}

	private Int2 TryGetNextMountainPixel(Int2 currPoint, Int2 direction)
	{
		Int2[] potentials = new Int2[10];
		potentials[0] = currPoint + nextDirection(direction, 0)*3;
		potentials[1] = currPoint + nextDirection(direction, 0) * 3;
		potentials[2] = currPoint + nextDirection(direction, 0) * 3;
		potentials[3] = currPoint + nextDirection(direction, 0) * 3;
		potentials[4] = currPoint + nextDirection(direction, 1) * 3;
		potentials[5] = currPoint + nextDirection(direction, 1) * 3;
		potentials[6] = currPoint + nextDirection(direction, 2) * 3;
		potentials[7] = currPoint + nextDirection(direction, -1) * 3;
		potentials[8] = currPoint + nextDirection(direction, -1) * 3;
		potentials[9] = currPoint + nextDirection(direction, -2) * 3;

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

	private bool TryMountainCenterPixel(Int2 pixel, float height, int distanceToCoast)
	{
		if (!map.PosInBounds(pixel))
		{
			return false;
		}
		else map.SetPoint(pixel, height);

		TrySpreadLandArea(pixel, distanceToCoast);
		foreach (Int2 point in map.GetAllNeighboringPoints(pixel))
		{
			if(map.PosInBounds(point))
			map.SetPoint(point, height * .75f);
		}

		return true;
	}

	private void TrySpreadLandArea(Int2 startPixel, int distanceToCoast)
	{
		SortedDupList<Int2> pixelsToSpreadTo = new SortedDupList<Int2>();
		pixelsToSpreadTo.Insert(distanceToCoast, startPixel);

		while (pixelsToSpreadTo.Count > 0)
		{
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
			if (map.GetValueAt(pixle) > 0 && (map.GetValueAt(pixle) > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				map.SetPoint(pixle, (map.GetValueAt(pixle) + avg) / 2);
		}
	}

	private float NeighborAverageHeight(Int2 pixle)
	{
		float sum = 0f;
		var points = map.GetAdjacentValues(pixle);
		foreach (var pt in points)
		{
			sum += pt;
		}
		return sum / points.Count;
	}

	private void CreateRivers()
	{
		int mapPixelsPerRiver = 500;
		int numOfRivers = (map.Width * map.Height) / mapPixelsPerRiver;

		List<Int2> possibleRiverStarts = new List<Int2>();
		for(int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, map.Width), Random.Range(0, map.Height));

			if(HeightAt(randPos) < Globals.MountainHeight && HeightAt(randPos) >= Globals.MinGroundHeight)
			{
				possibleRiverStarts.Add(randPos);
			}
		}

		int k = 0;
		while (numOfRivers > 0 && k < possibleRiverStarts.Count)
		{
			if (possibleRiverStarts.Count > k && 
				RiverStartValue(possibleRiverStarts[k]) > 0)
			{
				var river = TryExpandRiver(possibleRiverStarts[k], new List<Int2>());

				if (river != null)
				{
					foreach (var px in river)
						map.SetPoint(px, 0);
					numOfRivers--;
				}
			}
			k++;
		}
	}

	private float RiverStartValue(Int2 startPos)
	{
		int numMountains = 0;
		int numOceans = 0;
		int numOthers = 0;
		foreach(float h in map.GetAdjacentValues(startPos))
		{
			if (h >= Globals.MountainHeight)
				numMountains++;
			else if (h == 0)
				numOceans++;
			else numOthers++;
		}

		if (numOceans == 0 && numMountains > 0 && numMountains < 4)
			return 1f;
		else return 0f;
	}

	private List<Int2> TryExpandRiver(Int2 pos, List<Int2> currPath)
	{
		Map2D<int> checkedTiles = new Map2D<int>(map.Width, map.Height);
		SortedDupList<Int2> nextRiverTiles = new SortedDupList<Int2>();
		int maxRiverLength = int.MaxValue;

		nextRiverTiles.Insert(0, pos);
		checkedTiles.SetPoint(pos, maxRiverLength);
		Int2 endTile = null;

		while (nextRiverTiles.Count > 0 && endTile == null)
		{
			var shortestTile = nextRiverTiles.MinValue();
			nextRiverTiles.PopMin();
			foreach (var neighbor in GetAdjacentRiverExpansions(shortestTile, checkedTiles))
			{
				if (map.GetValueAt(neighbor) < Globals.MinGroundHeight)
				{
					endTile = shortestTile;
					break;
				}
				else
				{
					int distMod = Random.Range(1, 3);
					checkedTiles.SetPoint(neighbor, checkedTiles.GetValueAt(shortestTile)- distMod);
					nextRiverTiles.Insert(checkedTiles.GetValueAt(shortestTile) - distMod, neighbor);
				}
			}
			
		}

		List<Int2> riverPath = new List<Int2>();
		if (endTile != null)
		{
			riverPath.Add(pos);
			riverPath.Add(endTile);
			BuildRiverBack(checkedTiles, riverPath);
		}
		return riverPath;
	}

	private void BuildRiverBack(Map2D<int> riverDistField, List<Int2> riverPath)
	{
		Int2 maxNeighbor = riverPath[riverPath.Count - 1];
		foreach(Int2 tile in riverDistField.GetAdjacentPoints(maxNeighbor).RandomEnumerate())
		{
			if (!riverPath.Contains(tile) && riverDistField.GetValueAt(tile) > riverDistField.GetValueAt(maxNeighbor))
				maxNeighbor = tile;
		}

		if (maxNeighbor == riverPath[riverPath.Count - 1])
			return;
		else
		{
			riverPath.Add(maxNeighbor);
			BuildRiverBack(riverDistField, riverPath);
		}
	}

	private List<Int2> GetAdjacentRiverExpansions(Int2 pos, Map2D<int> checkedTiles)
	{
		List<Int2> expansionTiles = new List<Int2>();
		foreach (Int2 neighbor in map.GetAdjacentPoints(pos))
		{
			if (checkedTiles.GetValueAt(neighbor) == 0 && map.GetValueAt(neighbor) < Globals.MountainHeight)
				expansionTiles.Add(neighbor);
		}
		return expansionTiles;
	}

	private bool bordersInProgressRiver(Int2 pos, List<Int2> currPath)
	{
		float numBorders = 0;
		foreach(Int2 px in map.GetAdjacentPoints(pos))
		{
			if (currPath.Contains(pos))
				numBorders++;
		}

		return numBorders > 1;
	}

	private bool bordersOcean(Int2 pos)
	{
		foreach(float height in map.GetAdjacentValues(pos))
		{
			if (height.Equals(0))
				return true;
		}
		return false;
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