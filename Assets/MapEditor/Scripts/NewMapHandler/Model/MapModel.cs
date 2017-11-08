using System;
using System.Collections.Generic;

[Serializable]
public class MapModel
{
	public Dictionary<string, KingdomModel> Kingdoms = new Dictionary<string, KingdomModel>();
	public Map2D<MapTileModel> Map;

	public MapModel(int w, int h)
	{
		Map = new Map2D<MapTileModel>(w, h);
	}
}
