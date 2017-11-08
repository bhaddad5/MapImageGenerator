using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CultureParser
{
	public static Dictionary<string, CultureModel> CultureData = new Dictionary<string, CultureModel>();

	public static void LoadCultures()
	{
		CultureData = ParserHelpers.ParseTypes<CultureModel>("cultures");
	}
}


[Serializable]
public class StoredStringModel
{
	public string storedString;
	public string[] conditions = new string[0];

	public List<Settlement.CityTrait> GetCityTraits()
	{
		List<Settlement.CityTrait> traits = new List<Settlement.CityTrait>();
		foreach (string trait in conditions)
			traits.Add((Settlement.CityTrait)System.Enum.Parse(typeof(Settlement.CityTrait), trait));
		return traits;
	}

	public List<Kingdom.KingdomTrait> GetKingdomTraits()
	{
		List<Kingdom.KingdomTrait> traits = new List<Kingdom.KingdomTrait>();
		foreach (string trait in conditions)
			traits.Add((Kingdom.KingdomTrait)System.Enum.Parse(typeof(Kingdom.KingdomTrait), trait));
		return traits;
	}
}

[Serializable]
public class StoredGroundValue
{
	public string groundTrait;
	public float groundValue;
}

[Serializable]
public class StoredCultureEntityPlacementByTrait
{
	public string TerrainTrait;
	public EntityPlacementModel[] Entity;
}

[Serializable]
public class CultureModel : ParsableData
{
	public string CultureName;
	public List<StoredStringModel> SettlementPrefixes = new List<StoredStringModel>();
	public List<StoredStringModel> SettlementSuffixes = new List<StoredStringModel>();
	public List<StoredStringModel> SettlementAreaInfo = new List<StoredStringModel>();
	public List<StoredStringModel> KingdomTitles = new List<StoredStringModel>();

	public string HeraldryOverlayImage;
	public List<StoredStringModel> HeraldryForegrounds = new List<StoredStringModel>();
	public List<StoredStringModel> HeraldryBackgrounds = new List<StoredStringModel>();

	public List<StoredGroundValue> GroundPropertyValues = new List<StoredGroundValue>();

	public List<StoredCultureEntityPlacementByTrait> ModelPlacementByTraitCultureInfos = new List<StoredCultureEntityPlacementByTrait>();
}