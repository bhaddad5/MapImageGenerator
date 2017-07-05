using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundGenerator : InitialMapGenerator, IMapGenerator
{
	public Map GenerateMaps(int width, int height)
	{
		Heights = new Map2D<float>(width, height);
		MakeHeights();

		Terrain = new Map2D<GroundTypes.Type>(width, height);
		MakeTerrain();

		return new Map(Heights, Terrain);
	}

	private void MakeHeights()
	{
		Heights.FillMap(1f);
		MakeUndergroundLakes();
		RandomizeCoastline(3, true);
		FlattenLakeBorders(5);
	}

	private void MakeUndergroundLakes()
	{
		int pixelsPerLake = 300;
		int avgNumOfLakes = (Heights.Width * Heights.Height) / pixelsPerLake;
		int numLakes = Random.Range(avgNumOfLakes / 2, avgNumOfLakes + avgNumOfLakes / 2);
		Debug.Log(numLakes);
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

	private void FlattenLakeBorders(int numPasses)
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

	private void MakeTerrain()
	{
		Terrain.FillMap(GroundTypes.Type.SolidRock);

		foreach (Int2 point in Heights.GetMapPoints())
		{
			float height = Heights.Get(point);
			if (height < Globals.MinGroundHeight)
				Terrain.Set(point, GroundTypes.Type.Ocean);
			if(height >= Globals.MinGroundHeight && height < Globals.MountainHeight)
				Terrain.Set(point, GroundTypes.Type.CaveFloor);
		}
	}
}
