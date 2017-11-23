using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplayHandler
{
	public static void DisplayMapText(MapModel Map, TextInstantiationController instantiator, Transform textParent)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).TextEntry != null)
			{
				instantiator.DisplayText(new Vector3(point.X + 0.5f, 0, point.Y + 0.5f), Map.Map.Get(point).TextEntry, textParent);
			}
		}
	}
	
}
