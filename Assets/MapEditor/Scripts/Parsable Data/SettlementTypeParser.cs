using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettlementTypeParser
{
	public static Dictionary<string, SettlementTypeModel> SettlementsData = new Dictionary<string, SettlementTypeModel>();

	public static void ParseSettlementTypes()
	{
		SettlementsData = ParserHelpers.ParseTypes<SettlementTypeModel>("settlements");
	}
}

[Serializable]
public class TraitPreferance
{
	public string Trait;
	public float Preference;
}

[Serializable]
public class SettlementTypeModel : ParsableData
{
	public string SettlementTypeName;
	public string NameChunk;
	public EntityPlacementModel Entity;
	public EntityPlacementModel PortEntity;
	public List<TraitPreferance> TraitPreferences;
}
