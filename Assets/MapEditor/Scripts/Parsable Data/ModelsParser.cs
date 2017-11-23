using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ModelsParser
{
	public static Dictionary<string, StoredModelEntry> ModelData = new Dictionary<string, StoredModelEntry>();

	public static void LoadModels()
	{
		ModelData = ParserHelpers.ParseTypes<StoredModelEntry>("models");

		foreach (StoredModelEntry modelEntry in ModelData.Values)
		{
			foreach (string modelPath in modelEntry.ModelPaths)
			{
				GameObject g = OBJLoader.LoadOBJFile(Application.streamingAssetsPath + "/" + modelPath);
				foreach (MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>())
				{
					mr.gameObject.AddComponent<MeshCollider>();
					mr.gameObject.layer = LayerMask.NameToLayer("PlacedModel");

					if (mr.material.mainTexture != null && (mr.material.mainTexture as Texture2D).TextureContainsTransparency())
						SetShaders(mr, "Unlit/Transparent Cutout");
					else SetShaders(mr, "Standard");
				}
				ModelData[modelEntry.Id].ParsedObjs.Add(g);
				g.SetActive(false);
			}
		}
	}

	private static void SetShaders(MeshRenderer mr, string shader)
	{
		foreach (Material material in mr.materials)
		{
			material.shader = Shader.Find(shader);
			material.SetFloat("_Glossieness", 0);
			material.SetFloat("_Metallic", 0);
		}
	}
}

[Serializable]
public class StoredModelEntry : ParsableData
{
	public List<string> ModelPaths;

	public List<GameObject> ParsedObjs = new List<GameObject>();

	public GameObject GetGameObject(int index)
	{
		if (index == -1)
			return ParsedObjs[GetRandomIndex()];
		else return ParsedObjs[index];
	}

	public int GetRandomIndex()
	{
		return UnityEngine.Random.Range(0, ParsedObjs.Count);
	}
}