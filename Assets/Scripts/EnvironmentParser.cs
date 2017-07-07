using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class EnvironmentParser
{
	public static ModelLookup modelLookup;

	public static List<MapEnvironment> LoadEnvironments()
	{
		//TODO: Kill Soon...
		modelLookup = GameObject.Find("ModelLookup").GetComponent<ModelLookup>();

		/*StoredEnvironment store = new StoredEnvironment();
		store.displayName = "Midlands";
		store.groundTypes = new []
		{
			new StoredGroundDisplayInfo() {GroundType = "Ocean", Texture = "Midlands/Sand.png"},
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
				}
			},
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

public class ModelPlacementInfo
{
	public enum PlacementMode
	{
		Scattered,
		ScatteredBordered,
		Edges,
		Corners,
	}

	public PlacementMode Mode;
	public GameObject Model;
	private int min;
	private int max;
	public int NumToPlace { get { return UnityEngine.Random.Range(min, max+1); } }

	public ModelPlacementInfo(string model, string mode, int mi, int ma)
	{
		min = mi;
		max = ma;
		Model = EnvironmentParser.modelLookup.LookupModel(model);
		Mode = (PlacementMode)System.Enum.Parse(typeof(PlacementMode), mode);
	}
}

public class GroundDisplayInfo
{
	public string groundType;
	public Color lookupColor;
	public Texture2D texture;
	public List<ModelPlacementInfo> placementInfos;
}

public class MapEnvironment
{public GroundDisplayInfo Ocean {get { return groundTypes["Ocean"]; } }
	public GroundDisplayInfo River { get { return groundTypes["River"]; } }
	public GroundDisplayInfo City { get { return groundTypes["City"]; } }
	public GroundDisplayInfo Road { get { return groundTypes["Road"]; } }

	public string displayName;
	public Dictionary<string, GroundDisplayInfo> groundTypes = new Dictionary<string, GroundDisplayInfo>();
	public IMapGenerator HeightGenerator;

	public GroundDisplayInfo GetGround(string type)
	{
		return groundTypes[type];
	}

	public bool ViableCityTerrain(GroundDisplayInfo groundDisplay)
	{
		return groundDisplay != Ocean &&
		       groundDisplay != River &&
		       groundDisplay != groundTypes["Mountain"];
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
	public StoredModelPlacementInfo[] DefaultModelPlacement;

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
