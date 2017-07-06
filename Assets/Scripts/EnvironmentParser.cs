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
			new StoredGroundDisplayInfo() {GroundType = "Ocean", Texture = "Midlands/Sand"},
			new StoredGroundDisplayInfo() {GroundType = "Wilderness", Texture = "Midlands/Wilderness"},
		};
		string str = JsonUtility.ToJson(store);
		Debug.Log(str);*/

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

public class GroundDisplayInfo
{
	public string groundType;
	public Color lookupColor;
	public Texture2D texture;
}

public class MapEnvironment
{
	public enum GroundTypes
	{
		Ocean,
		River,
		Swamp,
		Mountain,
		Forest,
		Wilderness,
		Fertile,
		City,
		Road,
	}

	public string displayName;
	public Dictionary<string, GroundDisplayInfo> groundTypes = new Dictionary<string, GroundDisplayInfo>();
	public IMapGenerator HeightGenerator;

	public GroundDisplayInfo GetGround(GroundTypes type)
	{
		return groundTypes[type.ToString()];
	}

	public bool ViableCityTerrain(GroundDisplayInfo groundDisplay)
	{
		return groundDisplay != groundTypes[GroundTypes.Ocean.ToString()] &&
		       groundDisplay != groundTypes[GroundTypes.River.ToString()] &&
		       groundDisplay != groundTypes[GroundTypes.Mountain.ToString()];
	}
}

[Serializable]
public class StoredGroundDisplayInfo
{
	public string GroundType;
	public string Texture;

	public GroundDisplayInfo ToDisplayInfo()
	{
		var gdi = new GroundDisplayInfo();
		gdi.lookupColor = Helpers.RandomColor();

		Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + Texture);
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(file);
		gdi.texture = tex;
		gdi.groundType = GroundType;
		return gdi;
	}
}

[Serializable]
public class StoredEnvironment
{
	public string displayName;
	public StoredGroundDisplayInfo[] groundTypes;

	public MapEnvironment ToEnvironment()
	{
		MapEnvironment env = new MapEnvironment();
		env.displayName = displayName;
		
		//TEMP:
		if(env.displayName == "Midlands")
			env.HeightGenerator = new MidlandGenerator();
		else if(env.displayName == "Underground")
			env.HeightGenerator = new UndergroundGenerator();
		foreach (StoredGroundDisplayInfo groundType in groundTypes)
		{
			GroundDisplayInfo displayInfo = groundType.ToDisplayInfo();
			env.groundTypes[groundType.GroundType] = displayInfo;
		}
		return env;
	}
}
