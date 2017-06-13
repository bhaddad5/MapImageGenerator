﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collab.Base.Graphics;
using Collab.Base.Math;

class MeshBuilder
{
	private List<UnityEngine.Mesh> builtMeshes = new List<UnityEngine.Mesh>();
	public MeshBuilder(StoredTerrainMap map, Map2D<float> heights)
	{
		BuildMeshes(map, heights);
	}

	private Map2D<float> vertHeights;

	private List<Vector3> vertices;
	private List<Vector2> uvCoords;
	private List<int> indices;

	private void BuildMeshes(StoredTerrainMap map, Map2D<float> pixelHeights)
	{
		float heightScaler = 1f;
		int vertsPerTileAcross = 5;
		List<Vector3> vertices = new List<Vector3>();
		populateVertHeights(map, vertsPerTileAcross, pixelHeights);
		RandomizeVertHeights();
		ScaleVertHeights(heightScaler);
		SetVerticesFromHeights(vertices, vertsPerTileAcross);		
		SetUVsAndTriangles(vertHeights.Width, vertHeights.Height);

		Collab.Base.Graphics.Mesh meshIn = new Collab.Base.Graphics.Mesh();
		meshIn.CreateVertexList();
		meshIn.CreateFacesList();
		meshIn.CreateTextureCoordinateList();
		List<Float3> verts = new List<Float3>();
		foreach(var vert in vertices)
		{
			verts.Add(new Float3(vert.x, vert.y, vert.z));
		}
		meshIn.AddVertices(verts);

		List<Float2> uvs = new List<Float2>();
		foreach (var uv in uvCoords)
		{
			uvs.Add(new Float2(uv.x, uv.y));
		}
		meshIn.AddTextureCoordinates(uvs);

		List<Int3> tries = new List<Int3>();
		for(int i = 0; i < indices.Count; i+= 3)
		{
			tries.Add(new Int3(indices[i], indices[i + 1], indices[i + 2]));
		}

		meshIn.AddTriangles(tries);

		var meshesOut = MeshSplitter.Split(meshIn, 10000, 10000);
		
		foreach(var meshOut in meshesOut)
		{
			UnityEngine.Mesh m = new UnityEngine.Mesh();
			List<Vector3> vs = new List<Vector3>();
			foreach (Float3 vert in meshOut.Vertices)
			{
				vs.Add(new Vector3(vert.X, vert.Y, vert.Z));
			}
			m.vertices = vs.ToArray();

			List<Vector2> u = new List<Vector2>();
			foreach (Float2 uv in meshOut.TextureCoordinates)
			{
				u.Add(new Vector2(uv.X, uv.Y));
			}
			m.uv = u.ToArray();

			List<int> ind = new List<int>();
			foreach (Int3 index in meshOut.Faces)
			{
				ind.Add(index.X);
				ind.Add(index.Y);
				ind.Add(index.Z);
			}
			m.triangles = ind.ToArray();

			List<Vector3> norms = new List<Vector3>();
			foreach(var vert in m.vertices)
			{
				norms.Add(new Vector3(0f, 1f, 0f));
			}
			m.normals = norms.ToArray();

			builtMeshes.Add(m);
		}
	}

	private void populateVertHeights(StoredTerrainMap map, int vertsPerTileAcross, Map2D<float> pixelHeights)
	{
		vertHeights = new Map2D<float>(map.Width * (vertsPerTileAcross - 1) + 1, map.Height * (vertsPerTileAcross - 1) + 1);

		foreach(var pixle in map.MapPixels())
		{
			fillHeightsForTile(pixle, vertsPerTileAcross, pixelHeights.GetValueAt(pixle), map.Width, map.Height);
		}
	}

	private void fillHeightsForTile(Int2 pixle, int vertsPerTileAcross, float tileHeight, int mapWidth, int mapHeight)
	{
		int baseI = pixle.X * (vertsPerTileAcross - 1);
		int baseJ = pixle.Y * (vertsPerTileAcross - 1);
		for (int x = 0; x < vertsPerTileAcross-1; x++)
		{
			for (int y = 0; y < vertsPerTileAcross-1; y++)
			{
				vertHeights.SetPoint(new Int2(baseI + x, baseJ + y), tileHeight);
			}
		}

		if (pixle.X == mapWidth - 1)
		{
			for(int y = 0; y < vertsPerTileAcross - 1; y++)
				vertHeights.SetPoint(new Int2(baseI + vertsPerTileAcross - 1, baseJ + y), tileHeight);
		}
		if (pixle.Y == mapHeight - 1)
		{
			for (int x = 0; x < vertsPerTileAcross - 1; x++)
				vertHeights.SetPoint(new Int2(baseI + x, baseJ + vertsPerTileAcross - 1), tileHeight);
		}
		if (pixle.X == mapWidth - 1 && pixle.Y == mapHeight - 1)
		{
			vertHeights.SetPoint(new Int2(baseI + vertsPerTileAcross - 1, baseJ + vertsPerTileAcross - 1), tileHeight);
		}
	}

	private void RandomizeVertHeights()
	{
		int numPasses = 3;
		for(int i = 0; i < numPasses; i++)
		{
			RandomizeVertHeightsPass();
		}

		int numCoastPasses = 2;
		for(int i = 0; i < numCoastPasses; i++)
		{
			RandomizeCoastHeights();
		}
	}

	private void RandomizeVertHeightsPass()
	{
		foreach(Int2 pos in vertHeights.GetMapPoints())
		{
			if (vertHeights.GetValueAt(pos) >= Globals.MinGroundHeight)
			{
				float newHeight = Mathf.Max(Globals.MinGroundHeight, (vertHeights.GetValueAt(pos) + NeighborAverageHeight(pos)) / 2 * Random.Range(1f, 1.3f));
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
			float rand = Random.Range(0f, 1f);
			if (IsOceanCoastVert(point) && rand < 0.5f)
				newHeights.SetPoint(point, Globals.MinGroundHeight);
		}
		vertHeights = newHeights;
	}

	private bool IsOceanCoastVert(Int2 pos)
	{
		if (vertHeights.GetValueAt(pos) > Globals.MinGroundHeight - 0.05f)
			return false;

		foreach(var neighbor in vertHeights.GetAdjacentValues(pos))
		{
			if (neighbor > Globals.MinGroundHeight - 0.05f)
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

	private void SetVerticesFromHeights(List<Vector3> vertices, float vertsPerTileAcross)
	{
		foreach(Int2 pos in vertHeights.GetMapPointsFlipped())
		{
			vertices.Add(new Vector3(pos.X / vertsPerTileAcross, vertHeights.GetValueAt(pos) * 2f, pos.Y / vertsPerTileAcross));
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
