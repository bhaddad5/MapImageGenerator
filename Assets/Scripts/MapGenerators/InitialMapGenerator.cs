using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialMapGenerator
{
	protected Map2D<float> Heights;
	protected Map2D<GroundTypes.Type> Terrain;

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
}
