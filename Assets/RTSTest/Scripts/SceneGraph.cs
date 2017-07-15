using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneGraph
{
	public static Map2D<SceneNode> Graph;
	public static Map2D<float> HeightGraph;

	public static void Setup(int width, int height, List<RtsModelPlacement> modelPlacers)
	{
		Graph = new Map2D<SceneNode>(width, height);
		HeightGraph = new Map2D<float>(width, height);

		foreach (Int2 point in Graph.GetMapPoints())
		{
			Graph.Set(point, new SceneNode(true));
		}

		for (int i = 0; i < 10; i++)
		{
			Int2 point = Int2.Random(0, width);
			HeightGraph.Set(point, Random.Range(5f, 7f));
			BlendDownFromPoint(point);
		}

		foreach (RtsModelPlacement modelPlacer in modelPlacers)
		{
			PlaceModels(modelPlacer);
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

	private static void PlaceModels(RtsModelPlacement placer)
	{
		int numToPlace = (int) Helpers.Randomize(placer.placementsPer400Square);
		for (int i = 0; i < numToPlace; i++)
		{
			PlaceModel(placer.objToPlace);
		}
	}

	private static void PlaceModel(GameObject obj)
	{
		Vector3 pos = new Vector3(Random.Range(0, HeightGraph.Width), 0, Random.Range(0, HeightGraph.Height));
		pos = HeightAdjustedPos(pos);
	}

	public static Vector3 HeightAdjustedPos(Vector3 pos)
	{
		float fractionX = pos.x % 1f;
		int integerX = (int) pos.x;
		float fractionZ = pos.z % 1f;
		int integerZ = (int)pos.z;
		float interpolatedValue = (1 - fractionX) *
		                    ((1 - fractionZ) * HeightGraph.Get(new Int2(integerX, integerZ)) +
		                     fractionZ * HeightGraph.Get(new Int2(integerX, integerZ + 1))) +
		                    fractionX *
		                    ((1 - fractionZ) * HeightGraph.Get(new Int2(integerX + 1, integerZ)) +
		                     fractionZ * HeightGraph.Get(new Int2(integerX + 1, integerZ + 1)));

		pos.y = interpolatedValue;
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

public class RtsModelPlacement
{
	public float placementsPer400Square;
	public GameObject objToPlace;
}