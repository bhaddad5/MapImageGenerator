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

	/*public List<Settlement.CityTrait> GetCityTraits()
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
	}*/
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

	public Dictionary<string, float> GroundPropertyValues = new Dictionary<string, float>();

	public float TileAreaValue(Int2 pos, MapModel Map)
	{
		float value = GetTileValue(pos, Map) * 2;

		float oneWaterBorderValue = 3f;
		float someWaterValue = 2f;
		float allWaterValue = -1f;

		int numWaterBorders = 0;
		foreach (Int2 t in Map.Map.GetAdjacentPoints(pos))
		{
			if (Map.Map.Get(t).TerrainId == "Ocean" || Map.Map.Get(t).TerrainId == "River")
				numWaterBorders++;
			value += GetTileValue(t, Map);
		}

		if (numWaterBorders == 1)
			value += oneWaterBorderValue;
		else if (numWaterBorders > 1 && numWaterBorders < 4)
			value += someWaterValue;
		else if (numWaterBorders == 4)
			value += allWaterValue;

		return value;
	}

	public float GetTileValue(Int2 tile, MapModel Map)
	{
		float value = 0;
		foreach (string trait in Map.Map.Get(tile).Terrain().Traits)
		{
			if (GroundPropertyValues.ContainsKey(trait))
				value += GroundPropertyValues[trait];
		}
		return value;
	}
}