using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class EnvironmentParser
{
	public static List<Culture> ParsedCultures;

	public static List<MapEnvironment> LoadEnvironments()
	{
		ParsedCultures = CultureParser.LoadCultures();

		var LoadedEnvironments = new List<MapEnvironment>();

		string environmentsFile = File.ReadAllText(Application.streamingAssetsPath + "/environments.txt");
		string[] environments = environmentsFile.Split(new[] { "|" }, StringSplitOptions.None);
		foreach (string env in environments)
		{
			StoredEnvironment storedEnvironment = JsonUtility.FromJson<StoredEnvironment>(env);
			LoadedEnvironments.Add(storedEnvironment.ToEnvironment());
		}
		return LoadedEnvironments;
	}
}


[Serializable]
public class StoredModelPlacementInfo
{
	public string model;
	public string placementMode;
	public int min;
	public int max;
}

[Serializable]
public class StoredGroundDisplayInfo
{
	public string GroundType;
	public string Texture;
	public float Difficulty = 0.1f;
	public string[] StoredTraits = new string[0];
	public StoredModelPlacementInfo[] DefaultModelPlacement = new StoredModelPlacementInfo[0];

	public GroundInfo ToDisplayInfo()
	{
		var gdi = new GroundInfo();
		gdi.lookupColor = Helpers.RandomColor();

		Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + Texture);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(file);
		gdi.texture = tex;
		gdi.groundType = GroundType;
		gdi.difficulty = Difficulty;

		gdi.traits = new List<GroundInfo.GroundTraits>();
		foreach (string trait in StoredTraits)
		{
			gdi.traits.Add((GroundInfo.GroundTraits)System.Enum.Parse(typeof(GroundInfo.GroundTraits), trait));
		}

		gdi.placementInfos = new List<ModelPlacementInfo>();
		foreach (StoredModelPlacementInfo info in DefaultModelPlacement)
		{
			gdi.placementInfos.Add(new ModelPlacementInfo(info.model, info.placementMode, info.min, info.max));
		}

		return gdi;
	}
}

[Serializable]
public class StoredCulturePrevelance
{
	public string cultureId;
	public int avgSettlementsPer80Square;
}

[Serializable]
public class StoredEnvironment
{
	public string EnvironmentId;
	public string DisplayName;
	public string[] MapBuildingCommands = new string[0];
	public StoredCulturePrevelance[] Cultures = new StoredCulturePrevelance[0];
	public StoredGroundDisplayInfo[] GroundTypes = new StoredGroundDisplayInfo[0];

	public MapEnvironment ToEnvironment()
	{
		MapEnvironment env = new MapEnvironment();
		env.EnvironmentId = EnvironmentId;
		env.DisplayName = DisplayName;
		env.MapBuildingCommands = MapBuildingCommands;

		foreach (StoredCulturePrevelance culturePrevelance in Cultures)
		{
			Culture culture = new Culture();
			foreach (Culture parsedCulture in EnvironmentParser.ParsedCultures)
			{
				if (culturePrevelance.cultureId == parsedCulture.CultureId)
					culture = parsedCulture;
			}

			env.Cultures.Add(new CulturePrevelance() {culture =  culture, numPlacementsPer80Square = culturePrevelance.avgSettlementsPer80Square});
		}

		foreach (StoredGroundDisplayInfo groundType in GroundTypes)
		{
			GroundInfo info = groundType.ToDisplayInfo();
			env.groundTypes[groundType.GroundType] = info;
		}
		return env;
	}
}
