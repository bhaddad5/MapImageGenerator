using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator
{
	public static Map2D<float> HeightMap;
	private Texture2D heightMapImage;

	public HeightMapGenerator(int width, int height)
	{
		HeightMap = new Map2D<float>(width, height);

		int pixelsPerRange = 700;
		int avgNumOfRanges = (width * height) / pixelsPerRange;

		int pixelsPerHill = 25;
		int avgNumHills = (width * height) / pixelsPerHill;

		GenerateMountainRanges(Random.Range(avgNumOfRanges/2, avgNumOfRanges + avgNumOfRanges/2));
		GenerateHills(Random.Range(avgNumHills / 2, avgNumHills + avgNumHills / 2));
		RandomizeCoastline();
		BlendHeightMap();
		CreateRivers();

		List<Color> pixels = new List<Color>();
		foreach (float h in HeightMap.GetMapValues())
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
		Int2 startingPixel = new Int2(Random.Range(0, HeightMap.Width - 1), Random.Range(0, HeightMap.Height - 1));
		float startingStrength = Random.Range(.6f, .75f);
		Int2 mountainDirection = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int mountainsLength = Random.Range(4, 50);
		int distToCoast = Random.Range(5, 30);

		Int2 currPixel = startingPixel;
		for (int k = 0; k < mountainsLength; k++)
		{
			float strength = startingStrength + Random.Range(-.4f, .4f);
			TryMountainCenterPixel(currPixel, strength, distToCoast);
			currPixel = TryGetNextMountainPixel(currPixel, mountainDirection); ;
		}
	}

	private Int2 TryGetNextMountainPixel(Int2 currPoint, Int2 direction)
	{
		Int2[] potentials = new Int2[10];
		potentials[0] = currPoint + nextDirection(direction, 0) * 3;
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
		if (!HeightMap.PosInBounds(pixel))
			return false;

		//Random mountain passes
		if (Helpers.Odds(0.2f))
			return true;

		HeightMap.SetPoint(pixel, height);

		TrySpreadLandArea(pixel, distanceToCoast);
		foreach (Int2 point in HeightMap.GetAllNeighboringPoints(pixel))
		{
			if(HeightMap.PosInBounds(point) && Helpers.Odds(0.7f))
				HeightMap.SetPoint(point, height * .6f);
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
				foreach (Int2 neighbor in HeightMap.GetAdjacentPoints(pixelsToSpreadTo.TopValue()))
				{
					if(HeightMap.GetValueAt(neighbor) == 0)
					{
						HeightMap.SetPoint(neighbor, Globals.MinGroundHeight);
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
		foreach(Int2 point in HeightMap.GetMapPoints())
		{
			TryRandomizeCoastTile(point);
		}
	}

	private void TryRandomizeCoastTile(Int2 tile)
	{
		float probOfWaterfy = 0.4f;
		if (IsCoastline(tile) && !BordersMountain(tile))
		{
			if (Helpers.Odds(probOfWaterfy))
				HeightMap.SetPoint(tile, 0f);
		}
	}

	private bool IsCoastline(Int2 tile)
	{
		foreach (Int2 neighbor in HeightMap.GetAdjacentPoints(tile))
		{
			if (HeightAt(neighbor).Equals(0))
				return true;
		}
		return false;
	}

	private bool BordersMountain(Int2 tile)
	{
		foreach (float neighbor in HeightMap.GetAllNeighboringValues(tile))
		{
			if (neighbor > Globals.MinGroundHeight)
				return true;
		}
		return false;
	}

	private void GenerateHills(int numOfHills)
	{
		List<Int2> possibleHillSites = new List<Int2>();
		for(int i = 0; i < numOfHills * 100; i++)
		{
			Int2 pos = new Int2(Random.Range(0, HeightMap.Width - 1), Random.Range(0, HeightMap.Height - 1));
			if (HeightMap.GetValueAt(pos) == Globals.MinGroundHeight && !BordersWater(pos))
				possibleHillSites.Add(pos);
		}

		for(int i = 0; i < numOfHills && i < possibleHillSites.Count; i++)
		{
			float hillHeight = Random.Range(.25f, .3f);
			HeightMap.SetPoint(possibleHillSites[i], hillHeight);

			foreach (Int2 point in HeightMap.GetAllNeighboringPoints(possibleHillSites[i]))
			{
				if (HeightMap.PosInBounds(point) && HeightMap.GetValueAt(point) == Globals.MinGroundHeight && Helpers.Odds(0.8f))
					HeightMap.SetPoint(point, hillHeight * .85f);
			}

		}
	}

	private bool BordersWater(Int2 tile)
	{
		foreach (float neighbor in HeightMap.GetAllNeighboringValues(tile))
		{
			if (neighbor < Globals.MinGroundHeight)
				return true;
		}
		return false;
	}

	private void BlendHeightMap()
	{
		int passes = 1;
		for (int i = 0; i < passes; i++)
		{
			BlendHeightMapPass();
		}
	}

	private void BlendHeightMapPass()
	{
		foreach (Int2 pixle in HeightMap.GetMapPoints())
		{
			float avg = NeighborAverageHeight(pixle);
			if (HeightMap.GetValueAt(pixle) > 0 && (HeightMap.GetValueAt(pixle) > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				HeightMap.SetPoint(pixle, (HeightMap.GetValueAt(pixle) + avg) / 2);
		}
	}

	private float NeighborAverageHeight(Int2 pixle)
	{
		float sum = 0f;
		var points = HeightMap.GetAdjacentValues(pixle);
		foreach (var pt in points)
		{
			sum += pt;
		}
		return sum / points.Count;
	}

	private void CreateRivers()
	{
		int mapPixelsPerRiver = 500;
		int numOfRivers = (HeightMap.Width * HeightMap.Height) / mapPixelsPerRiver;

		List<Int2> possibleRiverStarts = new List<Int2>();
		for(int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, HeightMap.Width), Random.Range(0, HeightMap.Height));

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
						HeightMap.SetPoint(px, Globals.MinGroundHeight - 0.05f);
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
		foreach(float h in HeightMap.GetAdjacentValues(startPos))
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
		Map2D<int> checkedTiles = new Map2D<int>(HeightMap.Width, HeightMap.Height);
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
				if (HeightMap.GetValueAt(neighbor) < Globals.MinGroundHeight)
				{
					endTile = shortestTile;
					break;
				}
				else if(HeightMap.GetValueAt(neighbor) <= HeightMap.GetValueAt(shortestTile) + 0.03f)
				{
					checkedTiles.SetPoint(neighbor, checkedTiles.GetValueAt(shortestTile)- 1);
					nextRiverTiles.Insert(checkedTiles.GetValueAt(shortestTile) - 1, neighbor);
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

	private List<Int2> GetAdjacentRiverExpansions(Int2 pos, Map2D<int> checkedTiles)
	{
		List<Int2> expansionTiles = new List<Int2>();
		foreach (Int2 neighbor in HeightMap.GetAdjacentPoints(pos))
		{
			if (checkedTiles.GetValueAt(neighbor) == 0 && HeightMap.GetValueAt(neighbor) < Globals.MountainHeight)
				expansionTiles.Add(neighbor);
		}
		return expansionTiles;
	}

	private void BuildRiverBack(Map2D<int> riverDistField, List<Int2> riverPath)
	{
		Int2 maxNeighbor = riverPath[riverPath.Count - 1];
		foreach (Int2 tile in riverDistField.GetAdjacentPoints(maxNeighbor).RandomEnumerate())
		{
			if (!riverPath.Contains(tile) && riverDistField.GetValueAt(tile) > riverDistField.GetValueAt(maxNeighbor))
			{
				if (maxNeighbor == riverPath[riverPath.Count - 1])
					maxNeighbor = tile;
				else if (Helpers.Odds(.5f))
					maxNeighbor = tile;
			}
				
		}

		if (maxNeighbor == riverPath[riverPath.Count - 1])
			return;
		else
		{
			riverPath.Add(maxNeighbor);
			BuildRiverBack(riverDistField, riverPath);
		}
	}

	private float HeightAt(Int2 px)
	{
		return HeightMap.GetValueAt(px);
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}

	public Map2D<float> GetHeightMap()
	{
		return HeightMap;
	}
}