using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshBuilder
{
	private Mesh builtMesh = new Mesh();
	public MeshBuilder(StoredTerrainMap map, float[][] heights)
	{
		BuildMesh(map, heights);
	}

	private void BuildMesh(StoredTerrainMap map, float[][] pixelHeights)
	{
		float heightScaler = 2f;
		int vertsPerTileAcross = 3;
		List<Vector3> vertices = new List<Vector3>();
		float[][] vertHeights = populateVertHeights(map, vertsPerTileAcross, pixelHeights);
		vertHeights = RandomizeVertHeights(vertHeights);
		ScaleVertHeights(vertHeights, heightScaler);
		SetVerticesFromHeights(vertices, vertHeights, vertsPerTileAcross);		
		builtMesh.vertices = vertices.ToArray();
		SetUVsAndTriangles(builtMesh, vertHeights.Length, vertHeights[0].Length);
		builtMesh.name = "MapMesh";
	}

	private float[][] populateVertHeights(StoredTerrainMap map, int vertsPerTileAcross, float[][] pixelHeights)
	{
		float[][] vertHeights = new float[map.Width * (vertsPerTileAcross-1) + 1][];
		for (int i = 0; i < vertHeights.Length; i++)
		{
			vertHeights[i] = new float[map.Height * (vertsPerTileAcross - 1) + 1];
		}

		for (int i = 0; i < map.Width; i++)
		{
			for (int j = 0; j < map.Height; j++)
			{
				var tile = map.TileAt(new Int2(i, j));
				vertHeights = fillHeightsForTile(vertHeights, i, j, vertsPerTileAcross, pixelHeights[i][j], map.Width, map.Height);			
			}
		}

		return vertHeights;
	}

	private float[][] fillHeightsForTile(float[][] heights, int i, int j, int vertsPerTileAcross, float tileHeight, int mapWidth, int mapHeight)
	{
		int baseI = i * (vertsPerTileAcross - 1);
		int baseJ = j * (vertsPerTileAcross - 1);
		for (int x = 0; x < vertsPerTileAcross-1; x++)
		{
			for (int y = 0; y < vertsPerTileAcross-1; y++)
			{
				heights[baseI + x][baseJ + y] = tileHeight;
			}
		}

		if (i == mapWidth - 1)
		{
			for(int x = 0; x < vertsPerTileAcross - 1; x++)
				heights[baseI + vertsPerTileAcross - 1][baseJ + x] = tileHeight;
		}
		if (j == mapHeight - 1)
		{
			for (int x = 0; x < vertsPerTileAcross - 1; x++)
				heights[baseI + x][baseJ + vertsPerTileAcross - 1] = tileHeight;
		}
		if (i == mapWidth - 1 && j == mapHeight - 1)
		{
			heights[baseI + vertsPerTileAcross - 1][baseJ + vertsPerTileAcross - 1] = tileHeight;
		}
		return heights;
	}

	private float[][] RandomizeVertHeights(float[][] heights)
	{
		for(int i = 0; i < heights.Length; i++)
		{
			for(int j = 0; j < heights[0].Length; j++)
			{
				if (heights[i][j] > 0)
					heights[i][j] = Mathf.Max(Globals.MinGroundHeight, (heights[i][j] + neighborAverageHeight(i, j, heights)) / 2 * Random.Range(1f, 1.3f));
				else heights[i][j] = Globals.MinGroundHeight - 0.05f;
			}
		}
		return heights;
	}

	private float neighborAverageHeight(int x, int y, float[][] heights)
	{
		List<float> points = new List<float>();
		TryAddPoint(points, x, y - 1, heights.Length - 1, heights[0].Length - 1, heights);
		TryAddPoint(points, x - 1, y, heights.Length - 1, heights[0].Length - 1, heights);
		TryAddPoint(points, x + 1, y, heights.Length - 1, heights[0].Length - 1, heights);
		TryAddPoint(points, x, y + 1, heights.Length - 1, heights[0].Length - 1, heights);

		float average = 0f;
		foreach (var pt in points)
		{
			average += pt;
		}
		return average/points.Count;
	}

	private void TryAddPoint(List<float> points, int x, int y, int maxX, int maxY, float[][] heights)
	{
		if (x >= 0 && x < maxX && y >= 0 && y < maxY)
			points.Add(heights[x][y]);
	}

	private void ScaleVertHeights(float[][] heights, float scale)
	{
		for(int i = 0; i < heights.Length; i++)
		{
			for(int j = 0; j < heights[0].Length; j++)
			{
				heights[i][j] = heights[i][j] * scale;
			}
		}
	}

	private void SetVerticesFromHeights(List<Vector3> vertices, float[][] heights, float vertsPerTileAcross)
	{
		for (int i = 0; i < heights.Length; i++)
		{
			for (int j = 0; j < heights[0].Length; j++)
			{
				vertices.Add(new Vector3(j/ vertsPerTileAcross, heights[j][i] * 10f, i/ vertsPerTileAcross));
			}
		}
	}

	//FROM: http://answers.unity3d.com/questions/667029/convert-an-array-of-points-into-a-mesh-generate-tr.html
	private void SetUVsAndTriangles(Mesh m, int lrLengthx, int lrLengthz)
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

		m.triangles = triangles;
		m.uv = uvs;
	} 

	public Mesh GetBuiltMesh()
	{
		return builtMesh;
	}
}
