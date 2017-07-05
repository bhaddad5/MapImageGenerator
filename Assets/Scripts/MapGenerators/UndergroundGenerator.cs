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

	}

	private void MakeTerrain()
	{
		
	}
}
