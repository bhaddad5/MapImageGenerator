using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RegionTile
{
	public Region region;
	public float holdingStrength;

	public RegionTile(Region r)
	{
		region = r;
	}

	public bool TrySetRegion(Region r, float strength)
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
		return region.color;
	}
}

