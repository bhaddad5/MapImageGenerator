using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidlandGenerator : InitialMapGenerator, IMapGenerator
{
	public Map GenerateMaps(int width, int height)
	{
		Heights = new Map2D<float>(width, height);

		int pixelsPerRange = 700;
		int avgNumOfRanges = (width * height) / pixelsPerRange;

		int pixelsPerHill = 25;
		int avgNumHills = (width * height) / pixelsPerHill;

		GenerateMountainRanges(Random.Range(avgNumOfRanges / 2, avgNumOfRanges + avgNumOfRanges / 2));
		GenerateHills(Random.Range(avgNumHills / 2, avgNumHills + avgNumHills / 2));
		ZeroOutEdges();
		RandomizeCoastline();
		BlendHeightMap();
		CreateRivers();



		Terrain = new Map2D<GroundTypes.Type>(width, height);

		foreach (var point in Terrain.GetMapPoints())
		{
			if (Heights.GetValueAt(point) < Globals.MinGroundHeight)
			{
				int numLandBorders = NumLandBorders(point);
				if (numLandBorders >= 6)
					Terrain.SetPoint(point, GroundTypes.Type.River);
				else Terrain.SetPoint(point, GroundTypes.Type.Ocean);
			}
			else if (Heights.GetValueAt(point) >= Globals.MountainHeight)
				Terrain.SetPoint(point, GroundTypes.Type.Mountain);
			else Terrain.SetPoint(point, GroundTypes.Type.Grass);
		}

		FillInLandTextures();

		return new Map(Heights, Terrain);
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
		Int2 startingPixel = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
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
		if (!Heights.PosInBounds(pixel))
			return false;

		//Random mountain passes
		if (Helpers.Odds(0.2f))
			return true;

		Heights.SetPoint(pixel, height);

		TrySpreadLandArea(pixel, distanceToCoast);
		foreach (Int2 point in Heights.GetAllNeighboringPoints(pixel))
		{
			if (Heights.PosInBounds(point) && Helpers.Odds(0.7f))
				Heights.SetPoint(point, height * .6f);
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
				foreach (Int2 neighbor in Heights.GetAdjacentPoints(pixelsToSpreadTo.TopValue()))
				{
					if (Heights.GetValueAt(neighbor) == 0)
					{
						Heights.SetPoint(neighbor, Globals.MinGroundHeight);
						pixelsToSpreadTo.Insert(pixelsToSpreadTo.TopKey() - 1, neighbor);
					}
				}
			}
			pixelsToSpreadTo.Pop();
		}
	}

	private void RandomizeCoastline()
	{
		int numPasses = 3;
		for (int i = 0; i < numPasses; i++)
		{
			RandomizeCoastlinePass();
		}
	}

	private void ZeroOutEdges()
	{
		for (int i = 0; i < Heights.Width; i++)
		{
			Heights.SetPoint(new Int2(i, 0), 0);
			Heights.SetPoint(new Int2(i, 1), 0);
			Heights.SetPoint(new Int2(i, Heights.Height - 2), 0);
			Heights.SetPoint(new Int2(i, Heights.Height - 1), 0);
		}
		for (int i = 0; i < Heights.Height; i++)
		{
			Heights.SetPoint(new Int2(0, i), 0);
			Heights.SetPoint(new Int2(1, i), 0);
			Heights.SetPoint(new Int2(Heights.Width - 2, i), 0);
			Heights.SetPoint(new Int2(Heights.Width - 1, i), 0);
		}
	}

	private void RandomizeCoastlinePass()
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			TryRandomizeCoastTile(point);
		}
	}

	private void TryRandomizeCoastTile(Int2 tile)
	{
		if (IsCoastline(tile) && !BordersMountain(tile))
		{
			if (Helpers.Odds(0.4f))
				Heights.SetPoint(tile, 0f);
		}
	}

	private bool IsCoastline(Int2 tile)
	{
		foreach (Int2 neighbor in Heights.GetAdjacentPoints(tile))
		{
			if (Heights.GetValueAt(neighbor).Equals(0))
				return true;
		}
		return false;
	}

	private bool BordersMountain(Int2 tile)
	{
		foreach (float neighbor in Heights.GetAllNeighboringValues(tile))
		{
			if (neighbor >= Globals.MountainHeight)
				return true;
		}
		return false;
	}

	private void GenerateHills(int numOfHills)
	{
		List<Int2> possibleHillSites = new List<Int2>();
		for (int i = 0; i < numOfHills * 100; i++)
		{
			Int2 pos = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
			if (Heights.GetValueAt(pos) == Globals.MinGroundHeight && !BordersWater(pos))
				possibleHillSites.Add(pos);
		}

		for (int i = 0; i < numOfHills && i < possibleHillSites.Count; i++)
		{
			float hillHeight = Random.Range(.25f, .3f);
			Heights.SetPoint(possibleHillSites[i], hillHeight);

			foreach (Int2 point in Heights.GetAllNeighboringPoints(possibleHillSites[i]))
			{
				if (Heights.PosInBounds(point) && Heights.GetValueAt(point) == Globals.MinGroundHeight && Helpers.Odds(0.8f))
					Heights.SetPoint(point, hillHeight * .85f);
			}

		}
	}

	private bool BordersWater(Int2 tile)
	{
		foreach (float neighbor in Heights.GetAllNeighboringValues(tile))
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
		foreach (Int2 pixle in Heights.GetMapPoints())
		{
			float avg = NeighborAverageHeight(pixle);
			if (Heights.GetValueAt(pixle) > 0 && (Heights.GetValueAt(pixle) > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				Heights.SetPoint(pixle, (Heights.GetValueAt(pixle) + avg) / 2);
		}
	}

	private float NeighborAverageHeight(Int2 pixle)
	{
		float sum = 0f;
		var points = Heights.GetAdjacentValues(pixle);
		foreach (var pt in points)
		{
			sum += pt;
		}
		return sum / points.Count;
	}

	private void CreateRivers()
	{
		int mapPixelsPerRiver = 500;
		int numOfRivers = (Heights.Width * Heights.Height) / mapPixelsPerRiver;

		List<Int2> possibleRiverStarts = new List<Int2>();
		for (int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width), Random.Range(0, Heights.Height));

			if (Heights.GetValueAt(randPos) < Globals.MountainHeight && Heights.GetValueAt(randPos) >= Globals.MinGroundHeight)
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
						Heights.SetPoint(px, Globals.MinGroundHeight - 0.05f);
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
		foreach (float h in Heights.GetAdjacentValues(startPos))
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
		Map2D<int> checkedTiles = new Map2D<int>(Heights.Width, Heights.Height);
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
				if (Heights.GetValueAt(neighbor) < Globals.MinGroundHeight)
				{
					endTile = shortestTile;
					break;
				}
				else if (Heights.GetValueAt(neighbor) <= Heights.GetValueAt(shortestTile) + 0.03f)
				{
					checkedTiles.SetPoint(neighbor, checkedTiles.GetValueAt(shortestTile) - 1);
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
		foreach (Int2 neighbor in Heights.GetAdjacentPoints(pos))
		{
			if (checkedTiles.GetValueAt(neighbor) == 0 && Heights.GetValueAt(neighbor) < Globals.MountainHeight)
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

	private int NumLandBorders(Int2 point)
	{
		int landBorders = 0;
		foreach (var tile in Heights.GetAllNeighboringValues(point))
		{
			if (tile >= Globals.MinGroundHeight)
				landBorders++;
		}
		return landBorders;
	}

	public void FillInLandTextures()
	{
		int numPasses = 4;
		for (int i = 0; i < numPasses; i++)
		{
			FillInLandTexturesPass();
		}
	}

	private void FillInLandTexturesPass()
	{
		foreach (var tile in Terrain.GetMapPoints())
		{
			TryFillInTile(tile);
		}
	}

	private void TryFillInTile(Int2 tile)
	{
		float oddsOfForest = 0.01f +
		                     0.01f * NextToNumOfType(tile, GroundTypes.Type.Mountain) +
		                     0.3f * NextToNumOfType(tile, GroundTypes.Type.Forest);
		float oddsOfFertile = 0.01f +
		                      0.07f * NextToNumOfType(tile, GroundTypes.Type.Ocean) +
		                      0.15f * NextToNumOfType(tile, GroundTypes.Type.River) -
		                      0.01f * NextToNumOfType(tile, GroundTypes.Type.Mountain) +
		                      0.3f * NextToNumOfType(tile, GroundTypes.Type.Fertile);
		float oddsOfSwamp = 0.001f +
		                    0.01f * NextToNumOfType(tile, GroundTypes.Type.Ocean) +
		                    0.01f * NextToNumOfType(tile, GroundTypes.Type.River) -
		                    1f * NextToNumOfType(tile, GroundTypes.Type.Mountain) +
		                    0.3f * NextToNumOfType(tile, GroundTypes.Type.Swamp);

		if (Terrain.GetValueAt(tile) == GroundTypes.Type.Grass)
		{
			if (Helpers.Odds(oddsOfFertile))
				Terrain.SetPoint(tile, GroundTypes.Type.Fertile);
			else if (Helpers.Odds(oddsOfForest))
				Terrain.SetPoint(tile, GroundTypes.Type.Forest);
			else if (Helpers.Odds(oddsOfSwamp))
				Terrain.SetPoint(tile, GroundTypes.Type.Swamp);
		}
	}

	private int NextToNumOfType(Int2 tile, GroundTypes.Type type)
	{
		int numNextTo = 0;
		foreach (GroundTypes.Type t in GetNeighborTiles(tile))
		{
			if (t == type)
				numNextTo++;
		}
		return numNextTo;
	}

	private List<GroundTypes.Type> GetNeighborTiles(Int2 pos)
	{
		List<GroundTypes.Type> neighbors = new List<GroundTypes.Type>();
		TryAddTile(pos + new Int2(1, 0), neighbors);
		TryAddTile(pos + new Int2(-1, 0), neighbors);
		TryAddTile(pos + new Int2(0, 1), neighbors);
		TryAddTile(pos + new Int2(0, -1), neighbors);
		return neighbors;
	}

	public void TryAddTile(Int2 pos, List<GroundTypes.Type> neighbors)
	{
		if (Terrain.PosInBounds(pos))
			neighbors.Add(Terrain.GetValueAt(pos));
	}
}
