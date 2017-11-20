using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JsonTests
{
	[MenuItem("Json Tests/Test SettlementType Json")]
	public static void TestSettlementTypeJson()
	{
		SettlementTypeModel sett = new SettlementTypeModel()
		{
			Id = "Castle",
			SettlementTypeName = " Castle",
			NameChunk = "AngloCastle",
			Entity = new EntityPlacementModel()
			{
				model = "model",
				max = 1,
				min = 1,
				placementMode = "Center"
			},
			TraitPreferences = new List<TraitPreferance>()
			{
				new TraitPreferance()
				{
					Preference = .3f,
					Trait = "River"
				}
			}
		};
		

		Debug.Log(JsonUtility.ToJson(sett));
	}

	[MenuItem("Json Tests/Test TextChunk Json")]
	public static void TestTextChunkJson()
	{
		TextChunkModel text = new TextChunkModel();
		text.TextOptions.Add(new StoredStringModel()
		{
			StoredString = "hi"
		});
		text.TextOptions.Add(new StoredStringModel()
		{
			StoredString = "hi2",
			Conditions = new List<string>() { "cond1", "cont2" }
		});

		Debug.Log(JsonUtility.ToJson(text));
	}

	[MenuItem("Json Tests/Test WorldModel Json")]
	public static void TestWorldModelJson()
	{
		WorldModel world = new WorldModel();
		world.Realms.Add(new RealmPlacementModel()
		{
			RealmId = "Egypt",
			MaxLatitude = .5f,
			MinLatitude = 0f
		});
		world.Cultures = new List<CulturePlacementModel>()
		{
			new CulturePlacementModel()
			{
				CultureId = "Anglo"
			}
		};
		Debug.Log(JsonUtility.ToJson(world));
	}

	[MenuItem("Json Tests/Test OverlayPlacementModel Json")]
	public static void TestOverlayPlacementModelJson()
	{
		OverlayPlacementModel overlay = new OverlayPlacementModel();

		Debug.Log(JsonUtility.ToJson(overlay));
	}


	[MenuItem("Json Tests/Test MapModel Json")]
	public static void TestMapModelJson()
	{
		MapModel Map = new MapModel(5, 5);
		Map.Kingdoms["a"] = new KingdomModel();

		Debug.Log(Map.ToJson());
	}

	[MenuItem("Json Tests/Test Map2D Json")]
	public static void TestMap2DJson()
	{
		Map2D<MapTileModel> TestMap2D = new Map2D<MapTileModel>(5, 5);
		foreach (Int2 mapPoint in TestMap2D.GetMapPoints())
		{
			TestMap2D.Set(mapPoint, new MapTileModel());
		}
		
		Debug.Log(TestMap2D.ToSerializable());
	}

	[MenuItem("Json Tests/Test Terrain Json")]
	public static void TestTerrainJson()
	{
		TerrainModel terrain = new TerrainModel();
		terrain.Texture = new StoredTexture()
		{
			TexturePath = "path"
		};

		Debug.Log(JsonUtility.ToJson(terrain));
	}

	[MenuItem("Json Tests/Test Realm Json")]
	public static void TestRealmJson()
	{
		RealmModel Realm = new RealmModel()
		{
			Id = "Midland",
			DisplayName = "Midland",
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

	[MenuItem("Json Tests/Test Culture Json")]
	public static void TestObjectJson()
	{
		CultureModel Culture = new CultureModel()
		{
			Id = "Anglo",
			CultureName = "Midlander",
			HeraldryOverlayImage = "HeraldryOverlay",
			SettlementTypes = new List<SettlementPlacementInfo>()
			{
				new SettlementPlacementInfo()
				{
					SettlementType = "AngloCastle",
					PlacementsPer20Square = 2
				}
			}
		};

		Debug.Log(JsonUtility.ToJson(Culture));
	}
}