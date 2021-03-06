﻿using System;
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
public class RealmModel : ParsableData
{
	public string DisplayName;
	public List<string> PreRiverCommands = new List<string>();
	public List<string> RiverCommands = new List<string>();
	public List<string> PostRiverCommands = new List<string>();
}