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
		Map2D<bool> done = new Map2D<bool>(HeightGraph.Width, HeightGraph.Height);
		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(point, HeightGraph.Get(point));
		done.Set(point, true);

		float heightDiff = 0.1f;

		while (frontierTiles.Count > 0)
		{
			var currPoint = frontierTiles.Pop();
			float currHeight = HeightGraph.Get(currPoint);
			if (currHeight < heightDiff)
				continue;
			foreach (Int2 adjacentPoint in HeightGraph.GetAdjacentPoints(currPoint))
			{
				if (!done.Get(adjacentPoint) && HeightGraph.Get(adjacentPoint) < currHeight)
				{
					HeightGraph.Set(adjacentPoint, currHeight - heightDiff);
					frontierTiles.Insert(adjacentPoint, currHeight - heightDiff);
					done.Set(adjacentPoint, true);
				}
			}
		}
	}

	public static Vector3 HeightAdjustedPos(Vector3 pos)
	{
		pos.y = HeightGraph.Get(new Int2((int)pos.x, (int)pos.z));
		return pos;
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