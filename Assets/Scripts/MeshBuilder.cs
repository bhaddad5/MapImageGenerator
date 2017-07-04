using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class MeshBuilder
{
	private List<UnityEngine.Mesh> builtMeshes = new List<UnityEngine.Mesh>();
	public MeshBuilder()
	{
		BuildMeshes();
	}

	private Map2D<float> vertHeights;

	private List<Vector3> vertices;
	private List<Vector2> uvCoords;
	private List<int> indices;

	private const int vertsPerTileAcross = 5;

	private void BuildMeshes()
	{
		float heightScaler = 1f;
		List<Vector3> vertices = new List<Vector3>();
		populateVertHeights();
		RandomizeVertHeights();
		ScaleVertHeights(heightScaler);
		SetVerticesFromHeights(vertices);		
		SetUVsAndTriangles(vertHeights.Width, vertHeights.Height);

		builtMeshes = MeshSplitter.Split(vertices, uvCoords, indices, 64000, 64000);

		foreach(Mesh m in builtMeshes)
			m.RecalculateNormals();
	}

	private void populateVertHeights()
	{
		vertHeights = new Map2D<float>(TerrainMapGenerator.TerrainMap.Width * vertsPerTileAcross + 1, TerrainMapGenerator.TerrainMap.Height * vertsPerTileAcross + 1);

		foreach(var pixle in TerrainMapGenerator.TerrainMap.GetMapPoints())
		{
			fillHeightsForTile(pixle, HeightMapGenerator.HeightMap.GetValueAt(pixle), TerrainMapGenerator.TerrainMap.Width, TerrainMapGenerator.TerrainMap.Height);
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
				vertHeights.SetPoint(new Int2(baseI + x, baseJ + y), tileHeight);
			}
		}

		if (pixle.X == mapWidth - 1)
		{
			for(int y = 0; y < vertsPerTileAcross; y++)
				vertHeights.SetPoint(new Int2(baseI + vertsPerTileAcross, baseJ + y), tileHeight);
		}
		if (pixle.Y == mapHeight - 1)
		{
			for (int x = 0; x < vertsPerTileAcross; x++)
				vertHeights.SetPoint(new Int2(baseI + x, baseJ + vertsPerTileAcross), tileHeight);
		}
		if (pixle.X == mapWidth - 1 && pixle.Y == mapHeight - 1)
		{
			vertHeights.SetPoint(new Int2(baseI + vertsPerTileAcross, baseJ + vertsPerTileAcross), tileHeight);
		}
	}

	private void RandomizeVertHeights()
	{
		RandomizeCoastHeights();
		
		int numPasses = 3;
		for(int i = 0; i < numPasses; i++)
		{
			//ZeroOutEdges();
			RandomizeVertHeightsPass();
		}

		RandomizeCoastBumps();
		//ZeroOutEdges();
	}

	private void ZeroOutEdges()
	{
		for (int i = 0; i < vertHeights.Width; i++)
		{
			vertHeights.SetPoint(new Int2(i, 0), 0);
			vertHeights.SetPoint(new Int2(i, vertHeights.Height - 1), 0);
		}
		for (int i = 0; i < vertHeights.Height; i++)
		{
			vertHeights.SetPoint(new Int2(0, i), 0);
			vertHeights.SetPoint(new Int2(vertHeights.Width - 1, i), 0);
		}
	}

	private void RandomizeVertHeightsPass()
	{
		foreach(Int2 pos in vertHeights.GetMapPoints())
		{
			if (vertHeights.GetValueAt(pos) >= Globals.MinGroundHeight)
			{
				float newHeight = Mathf.Max(Globals.MinGroundHeight, (vertHeights.GetValueAt(pos) + NeighborAverageHeight(pos)) / 2 * Random.Range(1f, 1.1f));
				vertHeights.SetPoint(pos, newHeight);
			}
			else vertHeights.SetPoint(pos, Globals.MinGroundHeight - 0.05f);
		}
	}

	private void RandomizeCoastHeights()
	{
		Map2D<float> newHeights = new Map2D<float>(vertHeights.Width, vertHeights.Height);
		foreach(Int2 point in vertHeights.GetMapPoints())
		{
			newHeights.SetPoint(point, vertHeights.GetValueAt(point));
			if (IsOceanCoastVert(point) && Helpers.Odds(0.9f))
				newHeights.SetPoint(point, Globals.MinGroundHeight);
		}
		vertHeights = newHeights;
	}

	private void RandomizeCoastBumps()
	{
		foreach (Int2 point in vertHeights.GetMapPoints())
		{
			if (IsOceanCoastVert(point))
				vertHeights.SetPoint(point, Random.Range(0, Globals.MinGroundHeight-0.02f));
		}
	}

	private bool IsOceanCoastVert(Int2 pos)
	{
		if (vertHeights.GetValueAt(pos) >= Globals.MinGroundHeight)
			return false;

		foreach(var neighbor in vertHeights.GetAdjacentValues(pos))
		{
			if (neighbor >= Globals.MinGroundHeight)
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
		return average/points.Count;
	}

	private void ScaleVertHeights(float scale)
	{
		foreach(Int2 point in vertHeights.GetMapPoints())
		{
			vertHeights.SetPoint(point, vertHeights.GetValueAt(point) * scale);
		}
	}

	private void SetVerticesFromHeights(List<Vector3> vertices)
	{
		foreach(Int2 pos in vertHeights.GetMapPointsFlipped())
		{
			vertices.Add(new Vector3(pos.X / ((float)vertsPerTileAcross), vertHeights.GetValueAt(pos) * 2f, pos.Y / ((float)vertsPerTileAcross)));
		}
	}

	//FROM: http://answers.unity3d.com/questions/667029/convert-an-array-of-points-into-a-mesh-generate-tr.html
	private void SetUVsAndTriangles(int lrLengthx, int lrLengthz)
	{
		int[] triangles = new int[lrLengthx * lrLengthz * 6];
		Vector2[] uvs = new Vector2[lrLengthx * lrLengthz];
		int index = 0;
		for (int z = 0; z < lrLengthz - 1; z++)
		{
			for (int x = 0; x < lrLengthx - 1; x++)
			{
				triangles[index + 2] = x + z * lrLengthx;
				triangles[index + 1] = x + 1 + z * lrLengthx;
				triangles[index + 0] = x + z * lrLengthx + lrLengthx;

				triangles[index + 3] = x + z * lrLengthx + lrLengthx;
				triangles[index + 4] = x + 1 + z * lrLengthx + lrLengthx;
				triangles[index + 5] = x + 1 + z * lrLengthx;
				index += 6;
			}
		}

		for (int z = 0; z <= lrLengthz - 1; z++)
		{
			for (int x = 0; x <= lrLengthx - 1; x++)
			{
				uvs[x + z * lrLengthx] = new Vector2(x / (lrLengthx - 1.0f), z / (lrLengthz - 1.0f));
			}
		}

		indices = triangles.ToList();
		uvCoords = uvs.ToList();
	} 

	public List<UnityEngine.Mesh> GetBuiltMeshes()
	{
		return builtMeshes;
	}
}
