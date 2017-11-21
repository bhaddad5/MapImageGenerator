using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplayHandler
{
	public static void DisplayMapText(MapModel Map, TextInstantiationController instantiator)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).TextEntry != null)
			{
				instantiator.DisplayText(new Vector3(point.X, 0, point.Y), Map.Map.Get(point).TextEntry);
			}
		}
	}
	
}
