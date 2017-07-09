using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	public void CreateRivers(int minNumRivers, int maxNumRivers)
	{
		int numOfRivers = Random.Range(minNumRivers, maxNumRivers);

		List<Int2> possibleRiverStarts = new List<Int2>();
		for (int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width), Random.Range(0, Heights.Height));

			if (Heights.Get(randPos) < Globals.MountainHeight && Heights.Get(randPos) >= Globals.MinGroundHeight)
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
						Heights.Set(px, Globals.MinGroundHeight - 0.05f);
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

		nextRiverTiles.Insert(pos, 0);
		checkedTiles.Set(pos, maxRiverLength);
		Int2 endTile = null;

		while (nextRiverTiles.Count > 0 && endTile == null)
		{
			var shortestTile = nextRiverTiles.MinValue();
			nextRiverTiles.PopMin();
			foreach (var neighbor in GetAdjacentRiverExpansions(shortestTile, checkedTiles))
			{
				if (Heights.Get(neighbor) < Globals.MinGroundHeight)
				{
					endTile = shortestTile;
					break;
				}
				else if (Heights.Get(neighbor) <= Heights.Get(shortestTile) + 0.02f)
				{
					checkedTiles.Set(neighbor, checkedTiles.Get(shortestTile) - 1);
					nextRiverTiles.Insert(neighbor, checkedTiles.Get(shortestTile) - 1);
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
			if (checkedTiles.Get(neighbor) == 0 && Heights.Get(neighbor) < Globals.MountainHeight)
				expansionTiles.Add(neighbor);
		}
		return expansionTiles;
	}

	private void BuildRiverBack(Map2D<int> riverDistField, List<Int2> riverPath)
	{
		Int2 maxNeighbor = riverPath[riverPath.Count - 1];
		foreach (Int2 tile in riverDistField.GetAdjacentPoints(maxNeighbor).RandomEnumerate())
		{
			if (!riverPath.Contains(tile) && riverDistField.Get(tile) > riverDistField.Get(maxNeighbor))
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

	public void TerrainFillInRivers(string river)
	{
		foreach (var point in Terrain.GetMapPoints())
		{
			if (Heights.Get(point) < Globals.MinGroundHeight && NumBordersOfAtLeaseHeight(point, Globals.MinGroundHeight) >= 6)
					Terrain.Set(point, MapGenerator.Environment.groundTypes[river]);
		}
	}

	private int NumBordersOfAtLeaseHeight(Int2 point, float height)
	{
		return Heights.GetAllNeighboringValues(point).Select((h) => h >= height).Count();
	}

	public void TerrainEncourageStartAlongMountains(string terrain, float odds)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if(IsSeaLevel(point) && Helpers.Odds(odds) && BordersMountain(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void TerrainEncourageStartAlongOcean(string terrain, float odds)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(odds) && BordersOcean(point))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void TerrainRandomlyStart(string terrain, float odds)
	{
		foreach (Int2 point in Heights.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(odds))
				Terrain.Set(point, MapGenerator.Environment.groundTypes[terrain]);
		}
	}

	public void TerrainExpandSimmilarTypes(int numPasses, string typeToExpand)
	{
		
		for (int i = 0; i < numPasses; i++)
		{
			Map2D<GroundInfo> NewTerrain = new Map2D<GroundInfo>(Terrain);
			foreach (Int2 point in Terrain.GetMapPoints())
			{
				if (IsSeaLevel(point))
				{
					var adjacentVals = GetAdjacentSeaLevelOfType(point, typeToExpand);
					if (Helpers.Odds(0.25f * adjacentVals.Count))
						NewTerrain.Set(point, MapGenerator.Environment.groundTypes[typeToExpand]);
				}
				
			}
			Terrain = NewTerrain;
		}
	}

	private List<GroundInfo> GetAdjacentSeaLevelOfType(Int2 point, string type)
	{
		var res = new List<GroundInfo>();
		foreach (Int2 adjacent in Terrain.GetAdjacentPoints(point))
		{
			if (IsSeaLevel(adjacent) && Terrain.Get(adjacent).groundType == type)
				res.Add(Terrain.Get(adjacent));
		}
		return res;
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

	
}
