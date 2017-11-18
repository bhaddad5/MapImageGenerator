using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class MeshConstructor
{
	public static Mesh BuildMeshes(Map2D<float> vertHeights, float scale)
	{
		List<Vector3> vertices = SetVerticesFromHeights(vertHeights, scale);
		List<int> indices = SetTriangles(vertHeights.Width, vertHeights.Height);
		List<Vector2> uvCoords = SetUVs(vertHeights.Width, vertHeights.Height);

		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.uv = uvCoords.ToArray();

		mesh.RecalculateNormals();

		return mesh;
	}

	private static List<Vector3> SetVerticesFromHeights(Map2D<float> vertHeights, float scale)
	{
		List<Vector3> vertices = new List<Vector3>();
		foreach (Int2 pos in vertHeights.GetMapPointsFlipped())
		{
			vertices.Add(new Vector3(pos.X/scale, vertHeights.Get(pos), pos.Y / scale));
		}
		return vertices;
	}

	//FROM: http://answers.unity3d.com/questions/667029/convert-an-array-of-points-into-a-mesh-generate-tr.html
	private static List<int> SetTriangles(int lrLengthx, int lrLengthz)
	{
		List<int> indices = new int[lrLengthx * lrLengthz * 6].ToList();
		int index = 0;
		for (int z = 0; z < lrLengthz - 1; z++)
		{
			for (int x = 0; x < lrLengthx - 1; x++)
			{
				indices[index + 2] = x + z * lrLengthx;
				indices[index + 1] = x + 1 + z * lrLengthx;
				indices[index + 0] = x + z * lrLengthx + lrLengthx;

				indices[index + 3] = x + z * lrLengthx + lrLengthx;
				indices[index + 4] = x + 1 + z * lrLengthx + lrLengthx;
				indices[index + 5] = x + 1 + z * lrLengthx;
				index += 6;
			}
		}

		return indices;
	}

	private static List<Vector2> SetUVs(int lrLengthx, int lrLengthz)
	{
		List<Vector2> uvCoords = new Vector2[lrLengthx * lrLengthz].ToList();

		for (int z = 0; z <= lrLengthz - 1; z++)
		{
			for (int x = 0; x <= lrLengthx - 1; x++)
			{
				uvCoords[x + z * lrLengthx] = new Vector2(x / (lrLengthx - 1.0f), z / (lrLengthz - 1.0f));
			}
		}

		return uvCoords;
	}
}
