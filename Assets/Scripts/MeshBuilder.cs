using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MeshBuilder
{
	private Mesh builtMesh = new Mesh();
	private List<Vector3> vertices = new List<Vector3>();
	public MeshBuilder(StoredTerrainMap map)
	{
		BuildMesh(map);
	}

	private void BuildMesh(StoredTerrainMap map)
	{
		for(int i = 0; i < map.Height; i++)
		{
			for(int j = 0; j < map.Width; j++)
			{
				SetVerticesFromTile(new Int2(j, i), map.TileAt(new Int2(j, i)), (float)map.Width, (float)map.Height);
			}
		}
		builtMesh.vertices = vertices.ToArray();
		SetUVsAndTriangles(builtMesh, map.Width, map.Height);
		builtMesh.name = "MapMesh";
	}

	private void SetVerticesFromTile(Int2 pos, TerrainTile tile, float width, float height)
	{
		vertices.Add(new Vector3(pos.X/width, TerrainTile.tileHeights[tile.tileType], pos.Y/height));
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
