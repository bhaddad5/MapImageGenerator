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
		float heightScaler = 1f;
		populateVertHeights();
		RandomizeVertHeights();
		ScaleVertHeights(heightScaler);

		return MeshConstructor.BuildMeshes(vertHeights, vertsPerTileAcross, 2f);
	}

	private void populateVertHeights()
	{
		vertHeights = new Map2D<float>(Map.Map.Width * vertsPerTileAcross + 1, Map.Map.Height * vertsPerTileAcross + 1);

		foreach (var pixle in Map.Map.GetMapPoints())
		{
			fillHeightsForTile(pixle, Map.Map.Get(pixle).Terrain().Height, Map.Map.Width, Map.Map.Height);
		}
	}

	private void fillHeightsForTile(Int2 pixle, float tileHeight, int mapWidth, int mapHeight)
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

		if (pixle.X == mapWidth - 1)
		{
			for (int y = 0; y < vertsPerTileAcross; y++)
				vertHeights.Set(new Int2(baseI + vertsPerTileAcross, baseJ + y), tileHeight);
		}
		if (pixle.Y == mapHeight - 1)
		{
			for (int x = 0; x < vertsPerTileAcross; x++)
				vertHeights.Set(new Int2(baseI + x, baseJ + vertsPerTileAcross), tileHeight);
		}
		if (pixle.X == mapWidth - 1 && pixle.Y == mapHeight - 1)
		{
			vertHeights.Set(new Int2(baseI + vertsPerTileAcross, baseJ + vertsPerTileAcross), tileHeight);
		}
	}

	private void RandomizeVertHeights()
	{
		RandomizeCoastHeights();

		int numPasses = 3;
		for (int i = 0; i < numPasses; i++)
		{
			RandomizeVertHeightsPass();
		}

		RandomizeCoastBumps();
	}

	private void RandomizeVertHeightsPass()
	{
		foreach (Int2 pos in vertHeights.GetMapPoints())
		{
			if (vertHeights.Get(pos) >= TerrainModel.MinGroundHeight())
			{
				float newHeight = Mathf.Max(TerrainModel.MinGroundHeight(), (vertHeights.Get(pos) + NeighborAverageHeight(pos)) / 2 * Random.Range(1f, 1.1f));
				vertHeights.Set(pos, newHeight);
			}
			else vertHeights.Set(pos, TerrainModel.MinGroundHeight() - 0.05f);
		}
	}

	private void RandomizeCoastHeights()
	{
		Map2D<float> newHeights = new Map2D<float>(vertHeights.Width, vertHeights.Height);
		foreach (Int2 point in vertHeights.GetMapPoints())
		{
			newHeights.Set(point, vertHeights.Get(point));
			if (IsOceanCoastVert(point) && Helpers.Odds(0.9f))
				newHeights.Set(point, TerrainModel.MinGroundHeight());
		}
		vertHeights = newHeights;
	}

	private void RandomizeCoastBumps()
	{
		foreach (Int2 point in vertHeights.GetMapPoints())
		{
			if (IsOceanCoastVert(point))
				vertHeights.Set(point, Random.Range(0, TerrainModel.MinGroundHeight() - 0.02f));
		}
	}

	private bool IsOceanCoastVert(Int2 pos)
	{
		if (vertHeights.Get(pos) >= TerrainModel.MinGroundHeight())
			return false;

		foreach (var neighbor in vertHeights.GetAdjacentValues(pos))
		{
			if (neighbor >= TerrainModel.MinGroundHeight())
				return true;
		}

		return false;
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

	private void ScaleVertHeights(float scale)
	{
		foreach (Int2 point in vertHeights.GetMapPoints())
		{
			vertHeights.Set(point, vertHeights.Get(point) * scale);
		}
	}
}
