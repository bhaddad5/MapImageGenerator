using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Based on Nick's code in Base
public static class MeshSplitter
{
	public static List<Mesh> Split(List<Vector3> vertices, List<Vector2> uvs, List<int> indices, int maxFaceCount, int maxVertexCount)
	{
		List<Mesh> meshes = new List<Mesh>();

		if (((indices.Count/3) <= maxFaceCount) && (vertices.Count <= maxVertexCount))
		{
			meshes.Add(CreateMesh(vertices, uvs, indices));
			return meshes;
		}

		int faceIndex = 0;

		while (faceIndex < indices.Count/3)
		{
			Mesh newMesh = new Mesh();
			List<Vector3> newVertices = new List<Vector3>();
			List<Vector2> newUvs = new List<Vector2>();
			List<int> newTriangles = new List<int>();
			
			Dictionary<int, int> sourceVertexIndexToDestVertexIndexDictionary = new Dictionary<int, int>();

			while ((faceIndex < indices.Count / 3) && (newTriangles.Count / 3 < maxFaceCount) && (newVertices.Count < (maxVertexCount - 3)))
			{
				Vector3 face = new Vector3(indices[faceIndex*3], indices[faceIndex * 3 + 1], indices[faceIndex * 3 + 2]);
				faceIndex += 1;
				Vector3 newFace = new Vector3();

				newFace.x = GetNewVertexIndex((int)face.x, vertices, uvs, newVertices, newUvs, sourceVertexIndexToDestVertexIndexDictionary);
				newFace.y = GetNewVertexIndex((int)face.y, vertices, uvs, newVertices, newUvs, sourceVertexIndexToDestVertexIndexDictionary);
				newFace.z = GetNewVertexIndex((int)face.z, vertices, uvs, newVertices, newUvs, sourceVertexIndexToDestVertexIndexDictionary);

				newTriangles.Add((int)newFace.x);
				newTriangles.Add((int)newFace.y);
				newTriangles.Add((int)newFace.z);
			}

			newMesh.vertices = newVertices.ToArray();
			newMesh.uv = newUvs.ToArray();
			newMesh.triangles = newTriangles.ToArray();

			meshes.Add(newMesh);
		}
		return meshes;
	}

	private static int GetNewVertexIndex(int oldVertexIndex, List<Vector3> oldVertices, List<Vector2> oldUvs, List<Vector3> newVertices, List<Vector2> newUvs, Dictionary<int, int> sourceVertexIndexToDestVertexIndexDictionary)
	{
		if (sourceVertexIndexToDestVertexIndexDictionary.ContainsKey(oldVertexIndex))
			return sourceVertexIndexToDestVertexIndexDictionary[oldVertexIndex];

		int newVertexIndex = DuplicateVertexIntoMesh(oldVertexIndex, oldVertices, oldUvs, newVertices, newUvs);
		sourceVertexIndexToDestVertexIndexDictionary.Add(oldVertexIndex, newVertexIndex);
		return newVertexIndex;
	}

	private static Mesh CreateMesh(List<Vector3> vertices, List<Vector2> uvs, List<int> indices)
	{
		Mesh m = new Mesh();
		m.vertices = vertices.ToArray();
		m.uv = uvs.ToArray();
		m.triangles = indices.ToArray();
		return m;
	}

	private static int DuplicateVertexIntoMesh(int index, List<Vector3> oldVertices, List<Vector2> oldUvs, List<Vector3> newVertices, List<Vector2> newUvs)
	{
		int ret = -1;

		Debug.Assert(index >= 0 && index < oldVertices.Count, "Vertex index out of range.");

		if (oldVertices.Count > 0)
		{
			newVertices.Add(oldVertices[index]);
			ret = newVertices.Count - 1;
		}
		
		Debug.Assert(newUvs.Count == ret);
		newUvs.Add(oldUvs[index]);

		return ret;
	}

}
