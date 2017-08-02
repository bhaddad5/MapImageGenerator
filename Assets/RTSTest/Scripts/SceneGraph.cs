using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SceneGraph
{
	public static Map2D<bool> PassableGraph;
	public static Map2D<float> HeightGraph;
	public static Map2D<List<TroopController>> ForceMap;

	public static int ForceMapPartitionSize = 5;

	public static void Setup(int width, int height, List<RtsModelPlacement> modelPlacers)
	{
		PassableGraph = new Map2D<bool>(width, height);
		HeightGraph = new Map2D<float>(width, height);

		ForceMap = new Map2D<List<TroopController>>(width/ ForceMapPartitionSize, height/ ForceMapPartitionSize);

		ForceMap.FillMap(new List<TroopController>());

		foreach (Int2 point in PassableGraph.GetMapPoints())
		{
			PassableGraph.Set(point, true);
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
		Vector3 pos = new Vector3(Random.Range(0f, HeightGraph.Width), 0f, Random.Range(0, HeightGraph.Height));
		pos = HeightAdjustedPos(pos);
		GameObject spawned = GameObject.Instantiate(obj);
		spawned.transform.position = pos;

		foreach (BoxCollider collider in spawned.GetComponentsInChildren<BoxCollider>())
		{
			BlockSceneNodes(collider);
		}
	}

	private static void BlockSceneNodes(BoxCollider coll)
	{
		Vector3 min = coll.transform.TransformPoint(coll.center - coll.size);
		Vector3 max = coll.transform.TransformPoint(coll.center + coll.size);

		Int2 minTile = new Int2((int)Mathf.Min(min.x, max.x), (int)Mathf.Min(min.z, max.z));
		Int2 maxTile = new Int2((int)Mathf.Max(min.x, max.x), (int)Mathf.Max(min.z, max.z));

		for (int i = minTile.X; i < maxTile.X; i++)
		{
			for (int j = minTile.Y; j < maxTile.Y; j++)
			{
				if(PassableGraph.PosInBounds(new Int2(i, j)))
					PassableGraph.Set(new Int2(i, j), false);
			}
		}
	}

	public static Vector3 HeightAdjustedPos(Vector3 pos)
	{
		float fractionX = pos.x % 1f;
		int integerX = (int) pos.x;
		float fractionZ = pos.z % 1f;
		int integerZ = (int)pos.z;
		float interpolatedValue = (1 - fractionX) *
		                    ((1 - fractionZ) * HeightGraph.GetOrDefault(new Int2(integerX, integerZ), 0) +
		                     fractionZ * HeightGraph.GetOrDefault(new Int2(integerX, integerZ + 1), 0)) +
		                    fractionX *
		                    ((1 - fractionZ) * HeightGraph.GetOrDefault(new Int2(integerX + 1, integerZ), 0) +
		                     fractionZ * HeightGraph.GetOrDefault(new Int2(integerX + 1, integerZ + 1), 0));

		pos.y = interpolatedValue;
		return pos;
	}

	public static bool PosIsPassable(Vector3 pos)
	{
		return PassableGraph.GetOrDefault(new Int2((int) pos.x, (int) pos.z), false);
	}

	public static Vector3 ClosestPassable(Vector3 pos)
	{
		Map2D<bool> visited = new Map2D<bool>(HeightGraph.Width, HeightGraph.Height);
		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		Int2 startPos = new Int2((int) pos.x, (int) pos.z);

		startPos.X = Math.Min(Math.Max(startPos.X, 0), HeightGraph.Width-1);
		startPos.Y = Math.Min(Math.Max(startPos.Y, 0), HeightGraph.Height - 1);

		frontierTiles.Insert(startPos, 0);
		while (frontierTiles.Count > 0)
		{
			Int2 currPos = frontierTiles.Pop();
			visited.Set(currPos, true);
			if (PassableGraph.Get(currPos))
			{
				return new Vector3(currPos.X + (pos.x%1), pos.y, currPos.Y + (pos.z%1));
			}
			foreach (Int2 adjacentPoint in HeightGraph.GetAdjacentPoints(currPos))
			{
				if(HeightGraph.PosInBounds(adjacentPoint) && !visited.Get(adjacentPoint))
					frontierTiles.Insert(adjacentPoint, 0);
			}
		}
		return pos;
	}
}

public class RtsModelPlacement
{
	public float placementsPer400Square;
	public GameObject objToPlace;

	public RtsModelPlacement(float num, GameObject o)
	{
		placementsPer400Square = num;
		objToPlace = o;
	}
}