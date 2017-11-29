using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class MapModel
{
	public Map2D<MapTileModel> Map;

	public float OccurancesPer20Scaler(int minH, int maxH)
	{
		int mapNumTiles = Map.Size / (20 * 20);
		float heightSizeScaler = (maxH - minH) / (float) Map.Height;
		return mapNumTiles * heightSizeScaler;
	}

	public MapModel(int w, int h)
	{
		Map = new Map2D<MapTileModel>(w, h);

		foreach (Int2 point in Map.GetMapPoints())
		{
			Map.Set(point, new MapTileModel());
		}
	}

	public void FillMapWithTerrain(string terrainId)
	{
		foreach (MapTileModel mapTileModel in Map.GetMapValues())
		{
			mapTileModel.TerrainId = terrainId;
		}
	}

	public string ToJson()
	{
		var serializable = new SerializableMapModel(Map);
		return JsonConvert.SerializeObject(serializable);
	}

	public static MapModel FromJson(string json)
	{
		return JsonConvert.DeserializeObject<SerializableMapModel>(json).ToMap();
	}

	public class SerializableMapModel
	{
		public Map2D<MapTileModel>.Map2DSerializable Map;

		public SerializableMapModel(){}

		public SerializableMapModel(Map2D<MapTileModel> map)
		{
			Map = map.ToSerializable();
		}

		public MapModel ToMap()
		{
			MapModel newMapModel = new MapModel(Map.Width, Map.Height);
			newMapModel.Map = Map.ToMap2D();
			return newMapModel;
		}
	}
}
