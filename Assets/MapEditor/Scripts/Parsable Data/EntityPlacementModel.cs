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
	}

	public PlacementMode Mode()
	{
		return (PlacementMode)System.Enum.Parse(typeof(PlacementMode), placementMode);
	}

	public GameObject Model()
	{
		return ModelsParser.ModelData[model];
	}

	public int NumToPlace()
	{
		return UnityEngine.Random.Range(min, max + 1);
	}

	public string model;
	public string placementMode;
	public int min;
	public int max;
}