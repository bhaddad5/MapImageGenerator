using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundGenerator : InitialMapGenerator, IMapGenerator
{
	public Map GenerateMaps(int width, int height, MapEnvironment env)
	{
		MapGenerator.Environment = env;
		Heights = new Map2D<float>(width, height);
		MakeHeights();

		Terrain = new Map2D<GroundInfo>(width, height);
		MakeTerrain();

		return new Map(Heights, Terrain);
	}

	private void MakeHeights()
	{
		Heights.FillMap(1f);
		MakeUndergroundLakes();
		RandomizeCoastline(3, true);
		MakeConnectingCaves();
		ExpandCaves(4);
	}

	private void MakeUndergroundLakes()
	{
		int pixelsPerLake = 300;
		int avgNumOfLakes = (Heights.Width * Heights.Height) / pixelsPerLake;
		int numLakes = Random.Range(avgNumOfLakes / 2, avgNumOfLakes + avgNumOfLakes / 2);
		for (int i = 0; i < numLakes; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width-1), Random.Range(0, Heights.Height-1));
			ExpandLakeFromPos(randPos);
		}
	}

	private void ExpandLakeFromPos(Int2 pos)
	{
		int averageLakeRadius = 2;
		int myLakeRadius = Random.Range(averageLakeRadius / 2, averageLakeRadius + averageLakeRadius / 2);
		SortedDupList<Int2> LakeFrontier = new SortedDupList<Int2>();
		LakeFrontier.Insert(pos, myLakeRadius);
		while (LakeFrontier.Count > 0)
		{
			Int2 currPos = LakeFrontier.TopValue();
			float currStrength = LakeFrontier.TopKey();
			LakeFrontier.Pop();
			Heights.Set(currPos, 0);
			foreach (Int2 point in Heights.GetAdjacentPoints(currPos))
			{
				if (!LakeFrontier.ContainsValue(point) && Heights.Get(point) > 0 && currStrength > 0)
				{
					LakeFrontier.Insert(point, currStrength - 1);
				}
			}
			
		}
	}

	private void ExpandCaves(int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			Map2D<float> newHeights = new Map2D<float>(Heights);
			foreach (Int2 point in Heights.GetMapPoints())
				TryFlattenLakeBorder(point, newHeights);
			Heights = newHeights;
		}
	}

	private Map2D<float> TryFlattenLakeBorder(Int2 point, Map2D<float> newHeights)
	{
		if (Heights.Get(point) < Globals.MinGroundHeight)
			return newHeights;
		bool bordersNonSolidRock = false;
		bool borderNonOcean = false;
		foreach (float neighborHeight in Heights.GetAdjacentValues(point))
		{
			if (neighborHeight >= Globals.MinGroundHeight)
				borderNonOcean = true;
			if(neighborHeight < Globals.MountainHeight)
				bordersNonSolidRock = true;
		}
		if(bordersNonSolidRock && borderNonOcean && Helpers.Odds(0.7f))
			newHeights.Set(point, Globals.MinGroundHeight);
		return newHeights;
	}

	private void MakeConnectingCaves()
	{
		int pixeldPerCave = 200;
		int averageNumCaves = (Heights.Width * Heights.Height) / pixeldPerCave;
		int actualNumCaves = Random.Range(averageNumCaves / 2, averageNumCaves + averageNumCaves / 2);
		for (int i = 0; i < actualNumCaves; i++)
		{
			MakeCave();
		}
	}

	private void MakeCave()
	{
		Int2 randDir = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int averageCaveLength = 8;
		int actualCaveLength = Random.Range(averageCaveLength / 2, averageCaveLength + averageCaveLength / 2);
		Int2 startingPixel = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));

		Int2 currPixel = startingPixel;
		for (int k = 0; k < actualCaveLength; k++)
		{
			Heights.Set(currPixel, Globals.MinGroundHeight);
			currPixel = GetNextPixelInDirection(currPixel, randDir, 3);
			if (!Heights.PosInBounds(currPixel))
				return;
		}
	}

	private void MakeTerrain()
	{
		Terrain.FillMap(MapGenerator.Environment.groundTypes["CaveWall"]);

		foreach (Int2 point in Heights.GetMapPoints())
		{
			float height = Heights.Get(point);
			if (height < Globals.MinGroundHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes["Ocean"]);
			if(height >= Globals.MinGroundHeight && height < Globals.MountainHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes["CaveFloor"]);
		}
	}
}
