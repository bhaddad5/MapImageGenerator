using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CultureParser
{
	public static List<Culture> LoadCultures()
	{
		List<Culture> LoadedCultures = new List<Culture>();

		string culturesFile = File.ReadAllText(Application.streamingAssetsPath + "/cultures.txt");
		string[] cultures = culturesFile.Split(new[] { "|" }, StringSplitOptions.None);
		foreach (string cul in cultures)
		{
			StoredCulture storedCulture = JsonUtility.FromJson<StoredCulture>(cul);
			//LoadedCultures.Add(storedCulture.ToCulture());
		}

		return LoadedCultures;
	}


	public StoredCulture TmpConvertCultureToJson(Culture c)
	{
		StoredCulture sc = new StoredCulture();
		sc.CultureName = c.CultureName;
		sc.HeraldryOverlayImage = c.heraldryOverlay;


		List<StoredStringOption> prefixes = new List<StoredStringOption>();
		foreach (var prefix in c.prefixes)
			prefixes.Add(new StoredStringOption() {storedString = prefix.nameChunk, prevelance = prefix.prevelance, conditions = ToStringList(prefix.constraints)});
		sc.SettlementPrefixes = prefixes.ToArray();

		List<StoredStringOption> suffixes = new List<StoredStringOption>();
		foreach (var suffix in c.suffixes)
			suffixes.Add(new StoredStringOption() { storedString = suffix.nameChunk, prevelance = suffix.prevelance, conditions = ToStringList(suffix.constraints) });
		sc.SettlementSuffixes = suffixes.ToArray();

		List<StoredStringOption> areaInfos = new List<StoredStringOption>();
		foreach (var areaInfo in c.areaInfo)
			areaInfos.Add(new StoredStringOption() { storedString = areaInfo.nameChunk, prevelance = areaInfo.prevelance, conditions = ToStringList(areaInfo.constraints) });
		sc.SettlementAreaInfo = areaInfos.ToArray();

		List<StoredStringOption> kingdomTitles = new List<StoredStringOption>();
		foreach (var kingdomTitle in c.kingdomTitles)
			kingdomTitles.Add(new StoredStringOption() { storedString = kingdomTitle.nameChunk, prevelance = kingdomTitle.prevelance, conditions = ToStringList(kingdomTitle.constraints) });
		sc.KingdomTitles = kingdomTitles.ToArray();

		List<StoredStringOption> heraldrySymbols = new List<StoredStringOption>();
		foreach (HeraldryOption heraldryOption in c.heraldryForeground)
			heraldrySymbols.Add(new StoredStringOption() {storedString = heraldryOption.imagePath, prevelance = heraldryOption.prevelance, conditions = ToStringList(heraldryOption.constraints)});
		sc.HeraldryForegrounds = heraldrySymbols.ToArray();

		List<StoredStringOption> heraldryBackgrounds = new List<StoredStringOption>();
		foreach (HeraldryOption heraldryOption in c.heraldryBackground)
			heraldryBackgrounds.Add(new StoredStringOption() { storedString = heraldryOption.imagePath, prevelance = heraldryOption.prevelance, conditions = ToStringList(heraldryOption.constraints) });
		sc.HeraldryBackgrounds = heraldryBackgrounds.ToArray();

		List<StoredGroundValue> groundValues = new List<StoredGroundValue>();
		foreach (var tileValue in c.tileValues)
			groundValues.Add(new StoredGroundValue() {groundType = tileValue.Key.ToString(), groundValue = tileValue.Value});
		sc.GroundPropertyValues = groundValues.ToArray();

		Debug.Log(JsonUtility.ToJson(sc));
		return sc;
	}

	public string[] ToStringList(List<Settlement.CityTrait> options)
	{
		List<string> strs = new List<string>();
		foreach (Settlement.CityTrait trait in options)
		{
			strs.Add(trait.ToString());
		}
		return strs.ToArray();
	}

	public string[] ToStringList(List<Kingdom.KingdomTrait> options)
	{
		List<string> strs = new List<string>();
		foreach (Settlement.CityTrait trait in options)
		{
			strs.Add(trait.ToString());
		}
		return strs.ToArray();
	}

}


[Serializable]
public class StoredStringOption
{
	public string storedString;
	public int prevelance;
	public string[] conditions = new string[0];
}

[Serializable]
public class StoredGroundValue
{
	public string groundType;
	public float groundValue;
}

[Serializable]
public class StoredCultureModelPlacementByTrait
{
	public string TerrainTrait;
	public StoredModelPlacementInfo[] ModelPlacementInfos;
}

[Serializable]
public class StoredCulture
{
	public string CultureName;
	public StoredStringOption[] SettlementPrefixes = new StoredStringOption[0];
	public StoredStringOption[] SettlementSuffixes = new StoredStringOption[0];
	public StoredStringOption[] SettlementAreaInfo = new StoredStringOption[0];
	public StoredStringOption[] KingdomTitles = new StoredStringOption[0];

	public string HeraldryOverlayImage;
	public StoredStringOption[] HeraldryForegrounds = new StoredStringOption[0];
	public StoredStringOption[] HeraldryBackgrounds = new StoredStringOption[0];

	public StoredGroundValue[] GroundPropertyValues = new StoredGroundValue[0];

	public StoredCultureModelPlacementByTrait[] ModelPlacementByTraitCultureInfos = new StoredCultureModelPlacementByTrait[0];
}
