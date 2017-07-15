using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneGraph
{
	public static Map2D<SceneNode> Graph;
	public static Map2D<float> HeightGraph;

	public static void Setup(int width, int height, List<BoxCollider> obstacles)
	{
		Graph = new Map2D<SceneNode>(width, height);
		HeightGraph = new Map2D<float>(width, height);

		foreach (Int2 point in Graph.GetMapPoints())
		{
			Graph.Set(point, new SceneNode(true));
		}

		for (int i = 0; i < 10; i++)
		{
			Int2 point = Int2.Random(Random.Range(0, width - 1), Random.Range(0, height - 1));
			HeightGraph.Set(point, Random.Range(5f, 7f));
			BlendDownFromPoint(point);
		}
	}

	private static void BlendDownFromPoint(Int2 point)
	{
		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(point, HeightGraph.Get(point));

		float heightDiff = 0.1f;

		while (frontierTiles.Count > 0)
		{
			var currPoint = frontierTiles.Pop();
			float currHeight = HeightGraph.Get(currPoint);
			if (currHeight < heightDiff)
				continue;
			foreach (Int2 adjacentPoint in HeightGraph.GetAdjacentPoints(currPoint))
			{
				if (HeightGraph.Get(adjacentPoint)  <= 0)
				{
					HeightGraph.Set(adjacentPoint, currHeight - heightDiff);
					frontierTiles.Insert(adjacentPoint, currHeight - heightDiff);
				}
			}
		}
	}

	public static void ExpandBlendDown()
	{
		
	}
}

public class SceneNode
{
	public bool passable;
	public SceneNode left;
	public SceneNode right;

	public SceneNode(bool pass)
	{
		passable = pass;
	}
}