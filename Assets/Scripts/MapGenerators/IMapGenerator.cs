using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapGenerator
{
	Map GenerateMaps(int width, int height, MapEnvironment env);
}

public class Map
{
	public Map2D<float> heights;
	public Map2D<GroundDisplayInfo> terrain;

	public Map(Map2D<float> h, Map2D<GroundDisplayInfo> ter)
	{
		heights = h;
		terrain = ter;
	}
}