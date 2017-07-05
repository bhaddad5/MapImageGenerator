using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypes : MonoBehaviour
{
	public enum Type
	{
		Ocean,
		River,
		Swamp,
		Mountain,
		Forest,
		Grass,
		Fertile,
		City,
		Road,
		//Underground
		SolidRock,
		MushroomForest,
		CaveFloor,
	}

	[System.Serializable]
	public struct GroundDisplayInfo
	{
		public Type type;
		public Color lookupColor;
		public Texture2D texture;
	}

	public List<GroundDisplayInfo> GroundDisplayData = new List<GroundDisplayInfo>();

	public GroundDisplayInfo GetDisplayInfo(Type type)
	{
		foreach (GroundDisplayInfo displayInfo in GroundDisplayData)
		{
			if (displayInfo.type == type)
				return displayInfo;
		}
		return new GroundDisplayInfo();
	}

	public static bool ViableCityTerrain(Type t)
	{
		return t != Type.SolidRock && t != Type.River && t != Type.Mountain && t != Type.Ocean;
	}
}
