using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RegionTile
{
	public Kingdom region;
	public float holdingStrength;

	public RegionTile(Kingdom r)
	{
		region = r;
	}

	public bool TrySetRegion(Kingdom r, float strength)
	{
		if(strength >= holdingStrength)
		{
			region = r;
			holdingStrength = strength;
			return true;
		}
		return false;
	}

	public Color GetColor()
	{
		return region.mainColor;
	}
}

