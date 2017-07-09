using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class EnvironmentParser
{
	public static List<MapEnvironment> LoadEnvironments()
	{
		/*StoredEnvironment store = new StoredEnvironment();
		store.displayName = "Midlands";
		store.groundTypes = new []
		{
			new StoredGroundDisplayInfo()
			{
				GroundType = "Ocean",
				Texture = "Midlands/Sand.png",
				StoredTraits = new []
				{
					"Water",
					"Impassable"
				}
			},
			new StoredGroundDisplayInfo()
			{
				GroundType = "Swamp",
				Texture = "Midlands/Swamp.png",
				DefaultModelPlacement = new []
				{
					new StoredModelPlacementInfo()
					{
						max = 1,
						min = 1,
						model = "Willow",
						placementMode = "Scattered"
					},
					new StoredModelPlacementInfo()
					{
						max = 1,
						min = 1,
						model = "Rushes",
						placementMode = "Scattered"
					},
				},
				StoredTraits = new []
				{
					"Muddy",
				}
			},
			new StoredGroundDisplayInfo()
			{
				GroundType = "Wilderness",
				Texture = "Midlands/Wilderness.png",
				DefaultModelPlacement = new []
				{
					new StoredModelPlacementInfo()
					{
						max = 1,
						min = 1,
						model = "PineTree",
						placementMode = "Scattered"
					},
				},
				StoredTraits = new []
				{
					"Hunting"
				}
			},
		};
		string str = JsonUtility.ToJson(store);
		Debug.Log(str);*/

		CultureParser parser = new CultureParser();
		parser.TmpConvertCultureToJson(CultureDefinitions.Anglo);

		List<Culture> cultures = CultureParser.LoadCultures();

		CultureDefinitions.Anglo = cultures[0];

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
	public string cultureName;
	public int avgSettlementsPer80Square;
}

[Serializable]
public class StoredEnvironment
{
	public string displayName;
	public StoredCulturePrevelance[] Cultures = new StoredCulturePrevelance[0];
	public StoredGroundDisplayInfo[] groundTypes = new StoredGroundDisplayInfo[0];

	public MapEnvironment ToEnvironment()
	{
		MapEnvironment env = new MapEnvironment();
		env.displayName = displayName;
		
		//TEMP:
		if(env.displayName == "Midlands")
			env.HeightGenerator = new MidlandGenerator();
		else if(env.displayName == "Underground")
			env.HeightGenerator = new UndergroundGenerator();

		foreach (StoredCulturePrevelance culturePrevelance in Cultures)
		{
			//TMP
			var culture = CultureDefinitions.Anglo;
			if (culturePrevelance.cultureName == "Dwarf")
				culture = CultureDefinitions.Dwarf;
			if (culturePrevelance.cultureName == "Orc")
				culture = CultureDefinitions.Orc;

			env.Cultures.Add(new CulturePrevelance() {culture =  culture, numPlacementsPer80Square = culturePrevelance.avgSettlementsPer80Square});
		}

		foreach (StoredGroundDisplayInfo groundType in groundTypes)
		{
			GroundInfo info = groundType.ToDisplayInfo();
			env.groundTypes[groundType.GroundType] = info;
		}
		return env;
	}
}
