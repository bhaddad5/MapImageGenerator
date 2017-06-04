using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

class RegionTile
{
	public Region region;
	public bool isSettlement = false;

	public RegionTile(Region r)
	{
		region = r;
	}

	public void SetIsSettlement(bool settlement)
	{
		isSettlement = settlement;
	}

	public Color GetColor()
	{
		if (isSettlement)
			return Color.White;
		else return region.color;
	}
}

