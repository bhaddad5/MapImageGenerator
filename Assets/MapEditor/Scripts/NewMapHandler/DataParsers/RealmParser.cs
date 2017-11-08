using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class RealmParser
{
	public static Dictionary<string, RealmModel> RealmsData = new Dictionary<string, RealmModel>();
	public static void LoadRealms()
	{
		RealmsData = ParserHelpers.ParseTypes<RealmModel>("realms");
	}
}

[Serializable]
public class StoredCulturePrevelance
{
	public string cultureId;
	public int avgSettlementsPer80Square;
}

[Serializable]
public class RealmModel : ParsableData
{
	public string DisplayName;
	public string[] MapBuildingCommands = new string[0];
	public StoredCulturePrevelance[] Cultures = new StoredCulturePrevelance[0];
}