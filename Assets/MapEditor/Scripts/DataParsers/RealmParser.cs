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
	public CultureModel Culture { get { return CultureParser.CultureData[cultureId]; } }
	public int avgSettlementsPer80Square;
}

[Serializable]
public class RealmModel : ParsableData
{
	public string DisplayName;
	public List<string> MapBuildingCommands = new List<string>();
	public List<StoredCulturePrevelance> Cultures = new List<StoredCulturePrevelance>();
}