using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMeshBuilder
{
	private MapModel Map;
	private Map2D<float> vertHeights;
	private const int vertsPerTileAcross = 5;

	public List<Mesh> BuildMapMeshes(MapModel map)
	{
		Map = map;
		populateVertHeights();
		RandomizeVertHeights();
		ZeroOutWaterBorders();

		return MeshConstructor.BuildMeshes(vertHeights, vertsPerTileAcross);
	}

	private void populateVertHeights()
	{
		vertHeights = new Map2D<float>(Map.Map.Width * vertsPerTileAcross + 1, Map.Map.Height * vertsPerTileAcross + 1);

		foreach (var pixle in Map.Map.GetMapPoints())
		{
			FillHeightsForTile(pixle, Map.Map.Get(pixle).Terrain().Height);
		}
	}

	private void FillHeightsForTile(Int2 pixle, float tileHeight)
	{
		int baseI = pixle.X * (vertsPerTileAcross);
		int baseJ = pixle.Y * (vertsPerTileAcross);
		for (int x = 0; x < vertsPerTileAcross; x++)
		{
			for (int y = 0; y < vertsPerTileAcross; y++)
			{
				vertHeights.Set(new Int2(baseI + x, baseJ + y), tileHeight);
			}
		}
	}

	private void RandomizeVertHeights()
	{
		int numPasses = 2;
		for (int i = 0; i < numPasses; i++)
		{
			RandomizeVertHeightsPass();
		}
	}

	private void RandomizeVertHeightsPass()
	{
		foreach (Int2 pos in vertHeights.GetMapPoints())
		{
			float newHeight = Mathf.Min(vertHeights.Get(pos), (vertHeights.Get(pos) + NeighborAverageHeight(pos)) / 2 * Random.Range(.7f, 1f));
			vertHeights.Set(pos, newHeight);
		}
	}

	private float NeighborAverageHeight(Int2 pos)
	{
		var points = vertHeights.GetAdjacentValues(pos);

		float average = 0f;
		foreach (var pt in points)
		{
			average += pt;
		}
		return average / points.Count;
	}

	private void ZeroOutWaterBorders()
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if(Map.Map.Get(point).Terrain().HasTrait(TerrainModel.GroundTraits.Ocean))
				continue;
			List<Int2> AdjacentOceanTiles = new List<Int2>();
			foreach (Int2 adjacentPoint in Map.Map.GetAdjacentPoints(point))
			{
				if (Map.Map.Get(adjacentPoint).Terrain().HasTrait(TerrainModel.GroundTraits.Ocean))
				{
					AdjacentOceanTiles.Add(adjacentPoint);
				}
			}
			foreach (Int2 adjacentOceanTile in AdjacentOceanTiles)
			{
				for (int i = 0; i < vertsPerTileAcross; i++)
				{
					var diff = point - adjacentOceanTile;
					float newHeight = Map.Map.Get(adjacentOceanTile).Terrain().Height;
					if (diff.Equals(new Int2(-1, 0)))
					{
						vertHeights.Set(new Int2(point.X * vertsPerTileAcross + vertsPerTileAcross, point.Y * vertsPerTileAcross + i), newHeight);
					}
					if (diff.Equals(new Int2(1, 0)))
					{
						vertHeights.Set(new Int2(point.X * vertsPerTileAcross, point.Y * vertsPerTileAcross + i), newHeight);
					}
					if (diff.Equals(new Int2(0, -1)))
					{
						vertHeights.Set(new Int2(point.X * vertsPerTileAcross + i, point.Y * vertsPerTileAcross + vertsPerTileAcross), newHeight);
					}
					if (diff.Equals(new Int2(0, 1)))
					{
						vertHeights.Set(new Int2(point.X * vertsPerTileAcross + i, point.Y * vertsPerTileAcross), newHeight);
					}
				}
			}
		}
	}

	private void SphereAtVertexHeight(Int2 vertex)
	{
		Helpers.DEBUGSphereAtPoint(new Vector3(vertex.X/(float)vertsPerTileAcross - Map.Map.Width / 2, 0, vertex.Y / (float)vertsPerTileAcross - Map.Map.Height / 2), 0.1f);
	}
}
