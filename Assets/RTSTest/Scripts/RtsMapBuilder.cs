using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsMapBuilder : MonoBehaviour
{
	public Texture2D placeholderTex;

	// Use this for initialization
	void Start () {
		SceneGraph.Setup(400, 400, new List<RtsModelPlacement>());

		List<Mesh> meshes = MeshConstructor.BuildMeshes(SceneGraph.HeightGraph, 1, 1);

		int meshNum = 0;
		foreach (Mesh m in meshes)
		{
			GameObject g = new GameObject("Mesh" + meshNum);
			g.transform.SetParent(transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>();
			g.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
			g.GetComponent<MeshRenderer>().material.mainTexture = placeholderTex;
			if (m.vertices.Length > 1)
				g.AddComponent<MeshCollider>();
			g.layer = LayerMask.NameToLayer("Ground");
			meshNum++;
		}
	}
}
