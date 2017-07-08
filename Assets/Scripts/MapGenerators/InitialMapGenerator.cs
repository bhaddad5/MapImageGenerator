using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialMapGenerator
{
	protected Map2D<float> Heights;
	protected Map2D<GroundInfo> Terrain;

	public void DefaultFill(string defaultTerrain)
	{
		Terrain.FillMap(MapGenerator.Environment.groundTypes[defaultTerrain]);
	}

	public void FillInOceans(string ocean)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (Heights.Get(point) < Globals.MinGroundHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes[ocean]);
		}
	}

	public void FillInSeaLevel(string seaLevel)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[seaLevel]);
		}
	}

	public void FillInMountains(string mountain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (Heights.Get(point) >= Globals.MountainHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes[mountain]);
		}
	}

	public void EncourageStartTerrainAlongMountains(string terrain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if(IsSeaLevel(point) && Helpers.Odds(0.1f) && BordersMountain(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void EncourageStartTerrainAlongOcean(string terrain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(0.1f) && BordersOcean(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void RandomlyStartTerrain(string terrain)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(0.01f))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void ExpandSimmilarTerrainTypes(int numPasses)
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
