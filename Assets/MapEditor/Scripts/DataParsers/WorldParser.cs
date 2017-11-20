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
	public List<RealmPlacementModel> Realms = new List<RealmPlacementModel>();
}

[Serializable]
public class RealmPlacementModel
{
	public string RealmId;
	public float MinLatitude;
	public float MaxLatitude;

	public RealmModel Realm { get { return RealmParser.RealmsData[RealmId]; } }
}