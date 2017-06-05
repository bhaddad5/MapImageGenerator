using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RegionTile
{
	public Region region;
	public float holdingStrength;
	public bool isSettlement = false;

	public RegionTile(Region r)
	{
		region = r;
	}

	public void SetIsSettlement(bool settlement)
	{
		isSettlement = settlement;
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
		if (isSettlement)
			return Color.white;
		else return region.color;
	}
}

