﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMeshBuilder
{
	//private static MapModel Map;
	private static Map2D<float> VertHeights;
	private static int VertsPerTile;

	public static Map2D<float> BuildVertHeights(MapModel map, int vertsPerTile)
	{
		VertsPerTile = vertsPerTile;
		VertHeights = new Map2D<float>(map.Map.Width * VertsPerTile + 1, map.Map.Height * VertsPerTile + 1);

		populateVertHeights(map);
		RandomizeVertHeights();
		ZeroOutTerrainTypeBorders(map, MapTileModel.TileTraits.Ocean);
		ZeroOutTerrainTypeBorders(map, MapTileModel.TileTraits.Settled);

		return VertHeights;
	}

	private static void populateVertHeights(MapModel Map)
	{
		foreach (var pixle in Map.Map.GetMapPoints())
		{
			FillHeightsForTile(pixle, Map.Map.Get(pixle).Height, Map);
		}
	}

	private static void FillHeightsForTile(Int2 pixle, float tileHeight, MapModel Map)
	{
		int baseI = pixle.X * (VertsPerTile);
		int baseJ = pixle.Y * (VertsPerTile);
		for (int x = 0; x < VertsPerTile; x++)
		{
			for (int y = 0; y < VertsPerTile; y++)
			{
				float height = tileHeight;
				int combinedHeights = 1;
				if (x == 0)
				{
					height += GetHeight(pixle + new Int2(-1, 0), Map);
					combinedHeights++;
				}
				if (y == 0)
				{
					height += GetHeight(pixle + new Int2(0, -1), Map);
					combinedHeights++;
				}
				if (x == VertsPerTile - 1)
				{
					height += GetHeight(pixle + new Int2(1, 0), Map);
					combinedHeights++;
				}
				if (y == VertsPerTile - 1)
				{
					height += GetHeight(pixle + new Int2(0, 1), Map);
					combinedHeights++;
				}
				height = height / combinedHeights;
				if (Map.Map.Get(pixle).HasTrait(MapTileModel.TileTraits.Water))
					height = tileHeight;
				if (Map.Map.Get(pixle).HasTrait(MapTileModel.TileTraits.Settled))
					height = tileHeight;
				VertHeights.Set(new Int2(baseI + x, baseJ + y), height);
			}
		}
	}

	private static float GetHeight(Int2 point, MapModel Map)
	{
		if (Map.Map.PosInBounds(point))
			return Map.Map.Get(point).Height;
		else return 0;
	}

	private static void RandomizeVertHeights()
	{
		int numPasses = 2;
		for (int i = 0; i < numPasses; i++)
		{
			RandomizeVertHeightsPass();
		}
	}

	private static void RandomizeVertHeightsPass()
	{
		foreach (Int2 pos in VertHeights.GetMapPoints())
		{
			float newHeight = Mathf.Min(VertHeights.Get(pos), (VertHeights.Get(pos) + NeighborAverageHeight(pos)) / 2 * Random.Range(.7f, 1f));
			VertHeights.Set(pos, newHeight);
		}
	}

	private static float NeighborAverageHeight(Int2 pos)
	{
		var points = VertHeights.GetAdjacentValues(pos);

		float average = 0f;
		foreach (var pt in points)
		{
			average += pt;
		}
		return average / points.Count;
	}

	private static void ZeroOutTerrainTypeBorders(MapModel Map, MapTileModel.TileTraits trait)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if(Map.Map.Get(point).HasTrait(trait))
				continue;
			List<Int2> AdjacentTraitTiles = new List<Int2>();
			foreach (Int2 adjacentPoint in Map.Map.GetAdjacentPoints(point))
			{
				if (Map.Map.Get(adjacentPoint).HasTrait(trait))
				{
					AdjacentTraitTiles.Add(adjacentPoint);
				}
			}
			foreach (Int2 adjacentTraitTile in AdjacentTraitTiles)
			{
				for (int i = 0; i < VertsPerTile; i++)
				{
					var diff = point - adjacentTraitTile;
					float newHeight = Map.Map.Get(adjacentTraitTile).Height;
					if (diff.Equals(new Int2(-1, 0)))
					{
						Int2 edgePos = new Int2(point.X * VertsPerTile + VertsPerTile, point.Y * VertsPerTile + i);
						VertHeights.Set(edgePos, newHeight);
						float avgHeight = (newHeight + VertHeights.Get(new Int2(edgePos.X - 2, edgePos.Y))) / 2f;
						VertHeights.Set(new Int2(edgePos.X - 1, edgePos.Y), avgHeight);
					}
					if (diff.Equals(new Int2(1, 0)))
					{
						Int2 edgePos = new Int2(point.X * VertsPerTile, point.Y * VertsPerTile + i);
						VertHeights.Set(edgePos, newHeight);
						float avgHeight = (newHeight + VertHeights.Get(new Int2(edgePos.X + 2, edgePos.Y))) / 2f;
						VertHeights.Set(new Int2(edgePos.X + 1, edgePos.Y), avgHeight);
					}
					if (diff.Equals(new Int2(0, -1)))
					{
						Int2 edgePos = new Int2(point.X * VertsPerTile + i, point.Y * VertsPerTile + VertsPerTile);
						VertHeights.Set(edgePos, newHeight);
						float avgHeight = (newHeight + VertHeights.Get(new Int2(edgePos.X, edgePos.Y - 2))) / 2f;
						VertHeights.Set(new Int2(edgePos.X, edgePos.Y - 1), avgHeight);
					}
					if (diff.Equals(new Int2(0, 1)))
					{
						Int2 edgePos = new Int2(point.X * VertsPerTile + i, point.Y * VertsPerTile);
						VertHeights.Set(edgePos, newHeight);
						float avgHeight = (newHeight + VertHeights.Get(new Int2(edgePos.X, edgePos.Y + 2))) / 2f;
						VertHeights.Set(new Int2(edgePos.X, edgePos.Y + 1), avgHeight);
					}
				}
			}
		}
	}

	private void SphereAtVertexHeight(Int2 vertex, MapModel Map)
	{
		Helpers.DEBUGSphereAtPoint(new Vector3(vertex.X/(float)VertsPerTile - Map.Map.Width / 2, 0, vertex.Y / (float)VertsPerTile - Map.Map.Height / 2), 0.1f);
	}
}
