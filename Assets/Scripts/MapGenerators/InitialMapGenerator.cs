using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialMapGenerator
{
	protected Map2D<float> Heights;
	protected Map2D<GroundDisplayInfo> Terrain;

	protected bool IsCoastline(Int2 tile)
	{
		foreach (Int2 neighbor in Heights.GetAdjacentPoints(tile))
		{
			if (Heights.Get(neighbor).Equals(0))
				return true;
		}
		return false;
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
		if (IsCoastline(tile) && (randomizeMountains || !BordersMountain(tile)))
		{
			if (Helpers.Odds(0.4f))
				Heights.Set(tile, 0f);
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
}
