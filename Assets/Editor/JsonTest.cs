using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JsonTests
{
	[MenuItem("My Menu/Test WorldModel Json")]
	public static void TestWorldModelJson()
	{
		WorldModel world = new WorldModel();
		world.Realms.Add(new RealmPlacementModel()
		{
			RealmId = "Egypt",
			MaxLatitude = .5f,
			MinLatitude = 0f
		});

		Debug.Log(JsonUtility.ToJson(world));
	}

	[MenuItem("My Menu/Test OverlayPlacementModel Json")]
	public static void TestOverlayPlacementModelJson()
	{
		OverlayPlacementModel overlay = new OverlayPlacementModel();

		Debug.Log(JsonUtility.ToJson(overlay));
	}


	[MenuItem("My Menu/Test MapModel Json")]
	public static void TestMapModelJson()
	{
		MapModel Map = new MapModel(5, 5);
		Map.Kingdoms["a"] = new KingdomModel();

		Debug.Log(Map.ToJson());
	}

	[MenuItem("My Menu/Test Map2D Json")]
	public static void TestMap2DJson()
	{
		Map2D<MapTileModel> TestMap2D = new Map2D<MapTileModel>(5, 5);
		foreach (Int2 mapPoint in TestMap2D.GetMapPoints())
		{
			TestMap2D.Set(mapPoint, new MapTileModel());
		}
		
		Debug.Log(TestMap2D.ToSerializable());
	}

	[MenuItem("My Menu/Test Terrain Json")]
	public static void TestTerrainJson()
	{
		TerrainModel terrain = new TerrainModel();
		terrain.LookupColor = Color.blue;

		Debug.Log(JsonUtility.ToJson(terrain));
	}

	[MenuItem("My Menu/Test Realm Json")]
	public static void TestRealmJson()
	{
		RealmModel Realm = new RealmModel()
		{
			Id = "Midland",
			DisplayName = "Midland",
			Cultures = new List<StoredCulturePrevelance>()
			{
				new StoredCulturePrevelance()
				{
					avgSettlementsPer80Square = 20,
					cultureId = "Anglo"
				}
			},
			PreRiverCommands = new List<string>()
			{
				"HeightsDefaultFill 0",
			},
			RiverCommands = new List<string>()
			{
				"HeightsDefaultFill 0",
			},
			PostRiverCommands = new List<string>()
			{
				"HeightsDefaultFill 0",
			}
		};

		Debug.Log(JsonUtility.ToJson(Realm));
	}

	[MenuItem("My Menu/Test Culture Json")]
	public static void TestObjectJson()
	{
		CultureModel Culture = new CultureModel()
		{
			Id = "Anglo",
			CultureName = "Midlander",
			GroundPropertyValues = new Dictionary<string, float>()
			{
				{"ocean", 3f},
				{"fertile", 4f }
			},
			HeraldryBackgrounds = new List<StoredStringModel>()
			{
				new StoredStringModel()
				{
					storedString = "heraldryPath",
					conditions = new []{"shit", "stuff"}
				},
				new StoredStringModel()
				{
					storedString = "heraldryPath2"
				}
			},
			HeraldryForegrounds = new List<StoredStringModel>()
			{
				new StoredStringModel()
				{
					storedString = "heraldryPath2"
				}
			},
			HeraldryOverlayImage = "HeraldryOverlay",
			KingdomTitles = new List<StoredStringModel>()
			{
				new StoredStringModel()
				{
					storedString = "heraldryPath2"
				}
			},
			SettlementAreaInfo = new List<StoredStringModel>()
			{
				new StoredStringModel()
				{
					storedString = "heraldryPath2"
				}
			},
			SettlementPrefixes = new List<StoredStringModel>()
			{
				new StoredStringModel()
				{
					storedString = "heraldryPath2"
				}
			},
			SettlementSuffixes = new List<StoredStringModel>()
			{
				new StoredStringModel()
				{
					storedString = "heraldryPath2"
				}
			}
		};

		Debug.Log(JsonUtility.ToJson(Culture));
	}
}