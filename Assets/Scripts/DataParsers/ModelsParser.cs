using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModelsParser
{
	public static void ParseModels()
	{
		string modelsFile = File.ReadAllText(Application.streamingAssetsPath + "/models.txt");
		string[] models = modelsFile.Split(new[] { "|" }, StringSplitOptions.None);
		foreach (string md in models)
		{
			StoredModels storedModels = JsonUtility.FromJson<StoredModels>(md);
			storedModels.WriteTooLookup();
		}
	}
}

[Serializable]
public class StoredModelEntry
{
	public string ModelId;
	public string ModelPath;
	public bool UnlitTransparent;
}

public class StoredModels
{
	public StoredModelEntry[] Models = new StoredModelEntry[0];

	public void WriteTooLookup()
	{
		foreach (StoredModelEntry entry in Models)
		{
			GameObject g = OBJLoader.LoadOBJFile(Application.streamingAssetsPath + "/" + entry.ModelPath);
			foreach (MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>())
			{
				mr.gameObject.AddComponent<BoxCollider>();
				mr.gameObject.layer = LayerMask.NameToLayer("PlacedModel");
				if (entry.UnlitTransparent)
					mr.material.shader = Shader.Find("Unlit/Transparent Cutout");
				else
				{
					mr.material.shader = Shader.Find("Standard");
					mr.material.SetFloat("_Glossieness", 0);
					mr.material.SetFloat("_Metallic", 0);
				}
			}
			ModelLookup.Models[entry.ModelId] = g;
			g.SetActive(false);
		}
	}
}
