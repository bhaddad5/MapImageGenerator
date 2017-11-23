using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class EntityPlacementModel
{
	public enum PlacementMode
	{
		Scattered,
		Center,
		Bridge,
		Rot0,
		Rot90,
		Rot180,
		Rot270
	}

	public PlacementMode Mode()
	{
		return (PlacementMode)System.Enum.Parse(typeof(PlacementMode), placementMode);
	}

	public GameObject Model()
	{
		return ModelsParser.ModelData[model].GetGameObject(modelIndex);
	}

	public int NumToPlace()
	{
		return UnityEngine.Random.Range(min, max + 1);
	}

	public void PreBakeModelIndex()
	{
		modelIndex = ModelsParser.ModelData[model].GetRandomIndex();
	}

	public string model;
	public string placementMode = PlacementMode.Center.ToString();
	public int min;
	public int max;
	public int modelIndex = -1;
}