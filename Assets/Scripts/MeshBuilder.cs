using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshBuilder
{
	private Mesh builtMesh = new Mesh();
		public MeshBuilder(StoredTerrainMap map)
	{
		BuildMesh(map);
	}

	private void BuildMesh(StoredTerrainMap map)
	{
		List<Vector3> vertices = new List<Vector3>();
		float[][] vertHeights = populateVertHeights(map);
		vertHeights = RandomizeVertHeights(vertHeights);
		SetVerticesFromHeights(vertices, vertHeights);		
		builtMesh.vertices = vertices.ToArray();
		SetUVsAndTriangles(builtMesh, vertHeights.Length, vertHeights[0].Length);
		builtMesh.name = "MapMesh";
	}

	private float[][] populateVertHeights(StoredTerrainMap map)
	{
		float[][] vertHeights = new float[map.Width * 2 + 1][];
		for (int i = 0; i < vertHeights.Length; i++)
		{
			vertHeights[i] = new float[map.Height * 2 + 1];
		}

		for (int i = 0; i < map.Width; i++)
		{
			for (int j = 0; j < map.Height; j++)
			{
				var tile = map.TileAt(new Int2(i, j));
				vertHeights[i * 2][j * 2] = TerrainTile.tileEdgeHeights[tile.tileType];
				vertHeights[i * 2 + 1][j * 2] = TerrainTile.tileEdgeHeights[tile.tileType];
				vertHeights[i * 2][j * 2 + 1] = TerrainTile.tileEdgeHeights[tile.tileType];
				vertHeights[i * 2 + 1][j * 2 + 1] = TerrainTile.tileHeights[tile.tileType];

				if (i == map.Width - 1)
				{
					vertHeights[i * 2 + 2][j * 2] = TerrainTile.tileEdgeHeights[tile.tileType];
					vertHeights[i * 2 + 2][j * 2 + 1] = TerrainTile.tileEdgeHeights[tile.tileType];
				}
				if (j == map.Height - 1)
				{
					vertHeights[i * 2][j * 2 + 2] = TerrainTile.tileEdgeHeights[tile.tileType];
					vertHeights[i * 2 + 1][j * 2 + 2] = TerrainTile.tileEdgeHeights[tile.tileType];
				}
				if (i == map.Width - 1 && j == map.Height - 1)
				{
					vertHeights[i * 2 + 2][j * 2 + 2] = TerrainTile.tileEdgeHeights[tile.tileType];
				}
			}
		}

		return vertHeights;
	}

	private float[][] RandomizeVertHeights(float[][] heights)
	{
		for(int i = 0; i < heights.Length; i++)
		{
			for(int j = 0; j < heights[0].Length; j++)
			{
				heights[i][j] = heights[i][j] + Random.Range(-.1f, .1f);
			}
		}
		return heights;
	}

	private void SetVerticesFromHeights(List<Vector3> vertices, float[][] heights)
	{
		for (int i = 0; i < heights.Length; i++)
		{
			for (int j = 0; j < heights[0].Length; j++)
			{
				vertices.Add(new Vector3(j, heights[j][i] * 50f, i));
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
				uvs[x + z * lrLengthx] = new Vector2(x / (lrLengthx - 1.0f), z / (lrLengthz - 1.0f));
				triangles[index + 2] = x + z * lrLengthx;
				triangles[index + 1] = x + 1 + z * lrLengthx;
				triangles[index + 0] = x + z * lrLengthx + lrLengthx;

				triangles[index + 3] = x + z * lrLengthx + lrLengthx;
				triangles[index + 4] = x + 1 + z * lrLengthx + lrLengthx;
				triangles[index + 5] = x + 1 + z * lrLengthx;
				index += 6;
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
