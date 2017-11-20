using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ModelsParser
{
	public static Dictionary<string, GameObject> ModelData = new Dictionary<string, GameObject>();

	public static void LoadModels()
	{
		Dictionary<string, StoredModelEntry> ModelTypes = ParserHelpers.ParseTypes<StoredModelEntry>("models");

		foreach (StoredModelEntry modelEntry in ModelTypes.Values)
		{
			GameObject g = OBJLoader.LoadOBJFile(Application.streamingAssetsPath + "/" + modelEntry.ModelPath);
			foreach (MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>())
			{
				mr.gameObject.AddComponent<MeshCollider>();
				mr.gameObject.layer = LayerMask.NameToLayer("PlacedModel");

				if (mr.material.mainTexture != null && (mr.material.mainTexture as Texture2D).TextureContainsTransparency())
					SetShaders(mr, "Unlit/Transparent Cutout");
				else SetShaders(mr, "Standard");
			}
			ModelData[modelEntry.Id] = g;
			g.SetActive(false);
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
	public string ModelPath;
}