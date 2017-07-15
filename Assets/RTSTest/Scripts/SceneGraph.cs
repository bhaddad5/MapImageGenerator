using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGraph
{
	public static Map2D<SceneNode> Graph;
	public SceneGraph(int width, int height, List<BoxCollider> obstacles)
	{
		Graph = new Map2D<SceneNode>(width, height);
		foreach (Int2 point in Graph.GetMapPoints())
		{
			Graph.Set(point, new SceneNode(true, 0f));
		}

		for (int i = 0; i < 10; i++)
		{
			Graph.Get(Int2.Random(0, width)).height = 5;
		}

		BlendUp(30);


		foreach (BoxCollider collider in obstacles)
		{
			
		}
	}



	public void BlendUp(int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Graph.GetMapPoints())
			{
				float avg = NeighborAverageHeightAbove(point);
				if (Graph.Get(point).height > 0 && (Graph.Get(point).height > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				{
					if (avg > Graph.Get(point).height)
						Graph.Get(point).height = (Graph.Get(point).height + avg) / 2;
				}
			}
		}
	}

	private float NeighborAverageHeightAbove(Int2 pixle)
	{
		float sum = 0f;
		var points = Graph.GetAdjacentValues(pixle);
		int count = 0;
		foreach (var pt in points)
		{
			if (pt.height > Graph.Get(pixle).height)
			{
				sum += pt.height;
				count++;
			}
		}
		return sum / count;
	}
}

public class SceneNode
{
	public bool passable;
	public float height;
	public SceneNode left;
	public SceneNode right;

	public SceneNode(bool pass, float h)
	{
		passable = pass;
		height = h;
	}
}