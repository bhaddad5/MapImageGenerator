using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class RealmParser
{
	public static Dictionary<string, RealmCreationInfo> RealmsData = new Dictionary<string, RealmCreationInfo>();
	public static void LoadRealms()
	{
		ModelsParser.LoadModels();
		CultureParser.LoadCultures();
		LocationParser.LoadLocations();
		TerrainParser.LoadTerrainTypes();

		var storedRealms = ParserHelpers.ParseTypes<StoredRealm>("realms");
		foreach (StoredRealm realm in storedRealms.Values)
		{
			RealmsData[realm.Id] = realm.ToEnvironment();
		}
	}
}


[Serializable]
public class StoredModelPlacementInfo
{
	public string model;
	public string placementMode;
	public int min;
	public int max;

	public ModelPlacementInfo ToModelPlacementInfo()
	{
		return new ModelPlacementInfo(model, placementMode, min, max);
	}
}

public class ModelPlacementInfo
{
	public enum PlacementMode
	{
		Scattered,
		ScatteredBordered,
		CityWalls,
		CityGates,
		Corners,
		Bridge,
	}

	public PlacementMode Mode;
	public GameObject Model { get { return ModelsParser.ModelData[modelId]; } }
	private string modelId;
	private int min;
	private int max;
	public int NumToPlace { get { return UnityEngine.Random.Range(min, max + 1); } }

	public ModelPlacementInfo(string model, string mode, int mi, int ma)
	{
		modelId = model;
		min = mi;
		max = ma;
		Mode = (PlacementMode)System.Enum.Parse(typeof(PlacementMode), mode);
	}
}

[Serializable]
public class StoredCulturePrevelance
{
	public string cultureId;
	public int avgSettlementsPer80Square;
}

[Serializable]
public class StoredRealm : ParsableData
{
	public string DisplayName;
	public string[] MapBuildingCommands = new string[0];
	public StoredCulturePrevelance[] Cultures = new StoredCulturePrevelance[0];

	public RealmCreationInfo ToEnvironment()
	{
		RealmCreationInfo env = new RealmCreationInfo();
		env.EnvironmentId = Id;
		env.DisplayName = DisplayName;
		env.MapBuildingCommands = MapBuildingCommands;

		foreach (StoredCulturePrevelance culturePrevelance in Cultures)
		{
			Culture culture = new Culture();
			foreach (Culture parsedCulture in CultureParser.CultureData.Values)
			{
				if (culturePrevelance.cultureId == parsedCulture.CultureId)
					culture = parsedCulture;
			}

			env.Cultures.Add(new CulturePrevelance() {culture =  culture, numPlacementsPer80Square = culturePrevelance.avgSettlementsPer80Square});
		}
		return env;
	}
}

public class CulturePrevelance
{
	public Culture culture;
	public int numPlacementsPer80Square;
}

public class RealmCreationInfo
{
	public string EnvironmentId;
	public string DisplayName;
	public string[] MapBuildingCommands = new string[0];
	public List<CulturePrevelance> Cultures = new List<CulturePrevelance>();
}