using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldParser
{
	public static Dictionary<string, WorldModel> WorldData = new Dictionary<string, WorldModel>();

	public static void LoadWorlds()
	{
		WorldData = ParserHelpers.ParseTypes<WorldModel>("worlds");
	}
}

[Serializable]
public class WorldModel : ParsableData
{
	public int MapWidth;
	public int MapHeight;
	public List<RealmPlacementModel> Realms = new List<RealmPlacementModel>();
	public List<CulturePlacementModel> Cultures = new List<CulturePlacementModel>();
}

public class WorldPlacementData
{
	public float MinLatitude;
	public float MaxLatitude;
}

[Serializable]
public class RealmPlacementModel : WorldPlacementData
{
	public string RealmId;
	public RealmModel Realm { get { return RealmParser.RealmsData[RealmId]; } }
}

[Serializable]
public class CulturePlacementModel : WorldPlacementData
{
	public string CultureId;
	public CultureModel Culture { get { return CultureParser.CultureData[CultureId]; } }
}