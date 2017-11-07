using System;
using System.Collections.Generic;

[Serializable]
public class MapModel
{
	public List<KingdomModel> Kingdoms = new List<KingdomModel>();
	public Map2D<MapTileModel> Map;
	public List<EntityModel> Entities = new List<EntityModel>();

	public MapModel(int w, int h)
	{
		Map = new Map2D<MapTileModel>(w, h);
	}
}
