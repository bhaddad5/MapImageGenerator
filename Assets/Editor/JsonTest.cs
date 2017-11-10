using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEditor;

public class Tests
{
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
			MapBuildingCommands = new List<string>()
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