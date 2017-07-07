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
	public Map2D<GroundInfo> terrain;

	public Map(Map2D<float> h, Map2D<GroundInfo> ter)
	{
		heights = h;
		terrain = ter;
	}
}