using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapGenerator
{
	Map GenerateMaps(int width, int height);
}

public class Map
{
	public Map2D<float> heights;
	public Map2D<GroundTypes.Type> terrain;

	public Map(Map2D<float> h, Map2D<GroundTypes.Type> ter)
	{
		heights = h;
		terrain = ter;
	}
}