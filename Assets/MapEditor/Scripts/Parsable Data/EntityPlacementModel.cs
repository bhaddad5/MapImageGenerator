using System;
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

	public PlacementMode Mode { get { return (PlacementMode)System.Enum.Parse(typeof(PlacementMode), placementMode); } }
	public GameObject Model { get { return ModelsParser.ModelData[model]; } }
	public int NumToPlace { get { return UnityEngine.Random.Range(min, max + 1); } }

	public string model;
	public string placementMode;
	public int min;
	public int max;
}