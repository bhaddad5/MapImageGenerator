using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class MeshSplitter
{
	public static List<Mesh> Split(List<Vector3> vertices, List<Vector2> uvs, List<int> faces, int maxFaceCount, int maxVertexCount)
	{
		List<Mesh> meshes = new List<Mesh>();

		if (((faces.Count <= maxFaceCount) && (vertices.Count <= maxVertexCount)))
		{
			Mesh m = new Mesh();
			m.vertices = vertices.ToArray();
			m.uv = uvs.ToArray();
			meshes.Add(m);
			m.triangles = faces.ToArray();
			return meshes;
		}

		int faceIndex = 0;

		while (faceIndex <faces.Count)
		{
			Mesh newMesh = new Mesh();

			Dictionary<int, int> sourceVertexIndexToDestVertexIndexDictionary = new Dictionary<int, int>();

			while ((faceIndex < faces.Count) && (faces.Count < maxFaceCount) && (vertices.Count < (maxVertexCount - 3)))
			{
				Vector3 face = mesh.Faces[faceIndex++];
				Int3 newFace = new Int3();

				newFace.X = GetNewVertexIndex(face.X, mesh, newMesh, sourceVertexIndexToDestVertexIndexDictionary);
				newFace.Y = GetNewVertexIndex(face.Y, mesh, newMesh, sourceVertexIndexToDestVertexIndexDictionary);
				newFace.Z = GetNewVertexIndex(face.Z, mesh, newMesh, sourceVertexIndexToDestVertexIndexDictionary);

				newMesh.Faces.Add(newFace);
			}

			meshes.Add(newMesh);
		}
		return meshes;
	}

	private static int GetNewVertexIndex(int oldVertexIndex, Mesh oldMesh, Mesh newMesh, Dictionary<int, int> sourceVertexIndexToDestVertexIndexDictionary)
	{
		if (sourceVertexIndexToDestVertexIndexDictionary.ContainsKey(oldVertexIndex))
			return sourceVertexIndexToDestVertexIndexDictionary[oldVertexIndex];

		int newVertexIndex = oldMesh.DuplicateVertexIntoMesh(oldVertexIndex, newMesh);
		sourceVertexIndexToDestVertexIndexDictionary.Add(oldVertexIndex, newVertexIndex);
		return newVertexIndex;
	}

	public static List<Mesh> SplitMeshByTriangleIndices(Mesh mesh, List<int> trianglemeshselection)
	{
		List<Mesh> meshes = new List<Mesh>();
		List<Dictionary<int, int>> vertexMappings = new List<Dictionary<int, int>>();

		for (int face = 0; face < trianglemeshselection.Count; face++)
		{
			int targetmesh = trianglemeshselection[face];
			
			while (meshes.Count <= targetmesh)
			{
				meshes.Add(null);
				vertexMappings.Add(null);
			}
			if (meshes[targetmesh] == null)
			{
				meshes[targetmesh] = new Mesh();
				meshes[targetmesh].CreateFacesList();
			}
			if (vertexMappings[targetmesh] == null)
				vertexMappings[targetmesh] = new Dictionary<int, int>();

			Mesh targetMesh = meshes[targetmesh];
			Dictionary<int, int> targetVertexMap = vertexMappings[targetmesh];

			Int3 sourceFace = mesh.Faces[face];
			Int3 resultFace = new Int3();

			if (targetVertexMap.ContainsKey(sourceFace.x))
				resultFace.x = targetVertexMap[sourceFace.x];
			else
			{
				int newindex = mesh.DuplicateVertexIntoMesh(sourceFace.x, targetMesh);
				targetVertexMap.Add(sourceFace.x, newindex);
				resultFace.x = newindex;
			}

			if (targetVertexMap.ContainsKey(sourceFace.y))
				resultFace.y = targetVertexMap[sourceFace.y];
			else
			{
				int newindex = mesh.DuplicateVertexIntoMesh(sourceFace.y, targetMesh);
				targetVertexMap.Add(sourceFace.y, newindex);
				resultFace.y = newindex;
			}

			if (targetVertexMap.ContainsKey(sourceFace.z))
				resultFace.z = targetVertexMap[sourceFace.z];
			else
			{
				int newindex = mesh.DuplicateVertexIntoMesh(sourceFace.z, targetMesh);
				targetVertexMap.Add(sourceFace.z, newindex);
				resultFace.z = newindex;
			}

			targetMesh.AddTriangle(resultFace);
		}

		return meshes;
	}
}*/