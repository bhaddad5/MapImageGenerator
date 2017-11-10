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

		foreach (Int2 point in Map.GetMapPoints())
		{
			Map.Set(point, new MapTileModel());
		}
	}

	public void FillMapWithHeight(float height)
	{
		foreach (MapTileModel mapTileModel in Map.GetMapValues())
		{
			mapTileModel.Height = height;
		}
	}

	public void FillMapWithTerrain(string terrainId)
	{
		foreach (MapTileModel mapTileModel in Map.GetMapValues())
		{
			mapTileModel.TerrainId = terrainId;
		}
	}
}
