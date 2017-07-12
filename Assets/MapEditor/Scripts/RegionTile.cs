using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RegionTile
{
	public Settlement settlement;
	public float holdingStrength;

	public RegionTile(Settlement s)
	{
		settlement = s;
	}

	public bool TrySetRegion(Settlement s, float strength)
	{
		if(strength >= holdingStrength)
		{
			settlement = s;
			holdingStrength = strength;
			return true;
		}
		return false;
	}

	public Color GetColor()
	{
		return settlement.kingdom.mainColor;
	}
}

