using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialMapGenerator
{
	protected Map2D<float> Heights;
	protected Map2D<GroundInfo> Terrain;


	//HEIGHTS
	public void HeightsDefaultFill(float height)
	{
		Heights.FillMap(height);
	}

	public void HeightRandomlyPlace(float height, float occurancesPer80Square)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
			Heights.Set(randPos, height);
		}
	}

	public void HeightRandomlyPlaceNotInWater(float height, float occurancesPer80Square)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
			if(Heights.Get(randPos) > 0)
				Heights.Set(randPos, height);
		}
	}

	public void HeightRandomlyPlaceAlongLine(float height, float occurancesPer80Square, float minLength, float maxLength, float spacing)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
			PlaceHeightAlongVector(randPos, height, new Int2(Random.Range(-1, 1), Random.Range(-1, 1)), Random.Range(minLength, maxLength), spacing);
		}
	}

	private void PlaceHeightAlongVector(Int2 startPos, float height, Int2 direction, float length, float spacing)
	{
		Int2 currPixel = startPos;
		for (int k = 0; k < (int)length; k++)
		{
			Heights.Set(currPixel, height);
			currPixel = GetNextPixelInDirection(currPixel, direction, (int)spacing);
			if (!Heights.PosInBounds(currPixel))
				return;
		}
	}

	protected Int2 GetNextPixelInDirection(Int2 currPoint, Int2 direction, int distBetweenPixles)
	{
		Int2[] potentials = new Int2[10];
		potentials[0] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[1] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[2] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[3] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[4] = currPoint + nextDirection(direction, 1) * distBetweenPixles;
		potentials[5] = currPoint + nextDirection(direction, 1) * distBetweenPixles;
		potentials[6] = currPoint + nextDirection(direction, 2) * distBetweenPixles;
		potentials[7] = currPoint + nextDirection(direction, -1) * distBetweenPixles;
		potentials[8] = currPoint + nextDirection(direction, -1) * distBetweenPixles;
		potentials[9] = currPoint + nextDirection(direction, -2) * distBetweenPixles;

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

	public void HeightRandomlyExpandLevelFromItselfOrLevel(float height, float adjacentExpansionHeight, float minExpansion, float maxExpansion)
	{
		Map2D<float> newHeights = new Map2D<float>(Heights);
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (Heights.Get(point).Equals(height) || (HeightBordersLevel(point, adjacentExpansionHeight) && !Heights.Get(point).Equals(adjacentExpansionHeight)))
				HeightExpandFromPoint(point, height, Random.Range(minExpansion, maxExpansion), newHeights, adjacentExpansionHeight);
		}

		Heights = newHeights;
	}

	public void HeightRandomlyExpandLevel(float height, float avgExpansion)
	{
		Map2D<float> newHeights = new Map2D<float>(Heights);
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (Heights.Get(point).Equals(height))
				HeightExpandFromPoint(point, height, Helpers.Randomize(avgExpansion), newHeights);
		}

		Heights = newHeights;
	}

	private void HeightExpandFromPoint(Int2 point, float height, float numExpansionsLevels, Map2D<float> newHeights, float ignoreLevel = -1)
	{
		SortedDupList<Int2> HeightFrontier = new SortedDupList<Int2>();
		HeightFrontier.Insert(point, numExpansionsLevels);
		while (HeightFrontier.Count > 0)
		{
			Int2 currPos = HeightFrontier.TopValue();
			float currStrength = HeightFrontier.TopKey();
			HeightFrontier.Pop();
			newHeights.Set(currPos, height);
			foreach (Int2 pos in Heights.GetAdjacentPoints(currPos))
			{
				if (!HeightFrontier.ContainsValue(pos) && !Heights.Get(pos).Equals(ignoreLevel) && currStrength > 0)
				{
					HeightFrontier.Insert(pos, currStrength - 1);
				}
			}
		}
	}

	public void HeightRandomizeLevelEdges(float height, int numPasses)
	{
		Map2D<float> newHeights = new Map2D<float>(Heights);
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Heights.GetMapPoints())
			{
				if (HeightBordersLevel(point, height))
				{
					if (Helpers.Odds(0.4f))
						newHeights.Set(point, height);
				}
			}
		}
		Heights = newHeights;
	}

	private bool HeightBordersLevel(Int2 tile, float height)
	{
		foreach (Int2 neighbor in Heights.GetAdjacentPoints(tile))
		{
			if (Heights.Get(neighbor).Equals(height))
				return true;
		}
		return false;
	}

	public void HeightBlendUp(int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Heights.GetMapPoints())
			{
				float avg = NeighborAverageHeightAbove(point);
				if (Heights.Get(point) > 0 && (Heights.Get(point) > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				{
					if(avg > Heights.Get(point))
						Heights.Set(point, (Heights.Get(point) + avg) / 2);
				}
			}
		}
	}

	private float NeighborAverageHeightAbove(Int2 pixle)
	{
		float sum = 0f;
		var points = Heights.GetAdjacentValues(pixle);
		int count = 0;
		foreach (var pt in points)
		{
			if (pt > Heights.Get(pixle))
			{
				sum += pt;
				count++;
			}
		}
		return sum / count;
	}

	public void HeightSetEdges(float edgeHeight)
	{
		for (int i = 0; i < Heights.Width; i++)
		{
			Heights.Set(new Int2(i, 0), edgeHeight);
			Heights.Set(new Int2(i, 1), edgeHeight);
			Heights.Set(new Int2(i, Heights.Height - 2), edgeHeight);
			Heights.Set(new Int2(i, Heights.Height - 1), edgeHeight);
		}
		for (int i = 0; i < Heights.Height; i++)
		{
			Heights.Set(new Int2(0, i), edgeHeight);
			Heights.Set(new Int2(1, i), edgeHeight);
			Heights.Set(new Int2(Heights.Width - 2, i), edgeHeight);
			Heights.Set(new Int2(Heights.Width - 1, i), edgeHeight);
		}
	}




	//TERRAIN
	public void TerrainDefaultFill(string defaultTerrain)
	{
		Terrain.FillMap(MapGenerator.Environment.groundTypes[defaultTerrain]);
	}

	public void TerrainFillInOceans(string ocean)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (Heights.Get(point) < Globals.MinGroundHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes[ocean]);
		}
	}

	public void TerrainFillInSeaLevel(string seaLevel)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[seaLevel]);
		}
	}

	public void TerrainFillInMountains(string mountain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (Heights.Get(point) >= Globals.MountainHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes[mountain]);
		}
	}

	public void TerrainEncourageStartAlongMountains(string terrain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if(IsSeaLevel(point) && Helpers.Odds(0.1f) && BordersMountain(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void TerrainEncourageStartAlongOcean(string terrain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(0.1f) && BordersOcean(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void TerrainRandomlyStart(string terrain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(0.01f))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void TerrainExpandSimmilarTypes(int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Terrain.GetMapPoints())
			{
				if (IsSeaLevel(point))
				{
					var adjacentVals = GetAdjacentSeaLevelTypes(point);
					if (adjacentVals.Count > 0 && Helpers.Odds(0.7f))
						Terrain.Set(point, adjacentVals[Random.Range(0, adjacentVals.Count - 1)]);
				}
				
			}
		}
	}




	private bool IsSeaLevel(Int2 point)
	{
		return Heights.Get(point) < Globals.MountainHeight && Heights.Get(point) >= Globals.MinGroundHeight;
	}

	protected bool BordersMountain(Int2 tile)
	{
		foreach (float neighbor in Heights.GetAllNeighboringValues(tile))
		{
			if (neighbor >= Globals.MountainHeight)
				return true;
		}
		return false;
	}

	protected bool BordersOcean(Int2 tile)
	{
		foreach (Int2 neighbor in Heights.GetAdjacentPoints(tile))
		{
			if (Heights.Get(neighbor).Equals(0))
				return true;
		}
		return false;
	}

	private List<GroundInfo> GetAdjacentSeaLevelTypes(Int2 point)
	{
		var res = new List<GroundInfo>();
		foreach (Int2 adjacent in Terrain.GetAdjacentPoints(point))
		{
			if(IsSeaLevel(adjacent))
				res.Add(Terrain.Get(adjacent));
		}
		return res;
	}

	//END NEW WORK
































	protected void RandomizeCoastline(int numPasses, bool randomizeMountains)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Heights.GetMapPoints())
			{
				TryRandomizeCoastTile(point, randomizeMountains);
			}
		}
	}

	private void TryRandomizeCoastTile(Int2 tile, bool randomizeMountains)
	{
		if (BordersOcean(tile) && (randomizeMountains || !BordersMountain(tile)))
		{
			if (Helpers.Odds(0.4f))
				Heights.Set(tile, 0f);
		}
	}

	
}
