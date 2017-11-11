using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

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

	public void FillMapWithTerrain(string terrainId)
	{
		foreach (MapTileModel mapTileModel in Map.GetMapValues())
		{
			mapTileModel.TerrainId = terrainId;
		}
	}

	public string ToJson()
	{
		var serializable = new SerializableMapModel(Kingdoms, Map);
		return JsonConvert.SerializeObject(serializable);
	}

	public static MapModel FromJson(string json)
	{
		return JsonConvert.DeserializeObject<SerializableMapModel>(json).ToMap();
	}

	public class SerializableMapModel
	{
		public List<KingdomModel> Kingdoms;
		public Map2D<MapTileModel>.Map2DSerializable Map;

		public SerializableMapModel(){}

		public SerializableMapModel(Dictionary<string, KingdomModel> kingdoms, Map2D<MapTileModel> map)
		{
			Kingdoms = kingdoms.Values.ToList();
			Map = map.ToSerializable();
		}

		public MapModel ToMap()
		{
			MapModel newMapModel = new MapModel(Map.Width, Map.Height);
			newMapModel.Map = Map.ToMap2D();

			foreach (KingdomModel kingdom in Kingdoms)
			{
				newMapModel.Kingdoms[kingdom.Id] = kingdom;
			}
			return newMapModel;
		}
	}
}
