using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class MeshConstructor
{
	public static Mesh BuildMesh(Map2D<float> vertHeights, float scale, Int2 startingPoint, int size)
	{
		Mesh builtMesh = new Mesh();

		Map2D<float> blockUsed = vertHeights.FlipMap().GetMapBlock(new Int2(startingPoint.Y, startingPoint.X), size, size);
		List <Vector3> vertices = SetVerticesFromHeights(blockUsed, scale);
		List<int> indices = SetTriangles(blockUsed.Width, blockUsed.Height);
		List<Vector2> uvCoords = SetUVs(blockUsed.Width, blockUsed.Height);

		builtMesh.vertices = vertices.ToArray();
		builtMesh.triangles = indices.ToArray();
		builtMesh.uv = uvCoords.ToArray();
		builtMesh.RecalculateNormals();

		return builtMesh;
	}

	private static List<Vector3> SetVerticesFromHeights(Map2D<float> vertHeights, float scale)
	{
		List<Vector3> vertices = new List<Vector3>();
		foreach (Int2 pos in vertHeights.GetMapPoints())
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
