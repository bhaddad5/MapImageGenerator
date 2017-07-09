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
			LoadedCultures.Add(storedCulture.ToCulture());
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
			groundValues.Add(new StoredGroundValue() {groundTrait = tileValue.Key.ToString(), groundValue = tileValue.Value});
		sc.GroundPropertyValues = groundValues.ToArray();

		Debug.Log(JsonUtility.ToJson(sc));
		return sc;
	}

	private string[] ToStringList(List<Settlement.CityTrait> options)
	{
		List<string> strs = new List<string>();
		foreach (Settlement.CityTrait trait in options)
		{
			strs.Add(trait.ToString());
		}
		return strs.ToArray();
	}

	private string[] ToStringList(List<Kingdom.KingdomTrait> options)
	{
		List<string> strs = new List<string>();
		foreach (Kingdom.KingdomTrait trait in options)
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

	public Culture ToCulture()
	{
		Culture c = new Culture();
		c.CultureName = CultureName;
		c.heraldryOverlay = HeraldryOverlayImage;

		c.prefixes = new List<SettlementNameOption>();
		foreach (StoredStringOption option in SettlementPrefixes)
			c.prefixes.Add(new SettlementNameOption(option.storedString, option.GetCityTraits(), option.prevelance));
		c.suffixes = new List<SettlementNameOption>();
		foreach (StoredStringOption option in SettlementSuffixes)
			c.suffixes.Add(new SettlementNameOption(option.storedString, option.GetCityTraits(), option.prevelance));
		c.areaInfo = new List<SettlementNameOption>();
		foreach (StoredStringOption option in SettlementAreaInfo)
			c.areaInfo.Add(new SettlementNameOption(option.storedString, option.GetCityTraits(), option.prevelance));

		c.kingdomTitles = new List<KingdomNameOption>();
		foreach (StoredStringOption option in KingdomTitles)
			c.kingdomTitles.Add(new KingdomNameOption(option.storedString, option.GetKingdomTraits(), option.prevelance));

		c.heraldryBackground = new List<HeraldryOption>();
		foreach (StoredStringOption option in HeraldryBackgrounds)
			c.heraldryBackground.Add(new HeraldryOption(option.storedString, option.GetCityTraits(), option.prevelance));
		c.heraldryForeground = new List<HeraldryOption>();
		foreach (StoredStringOption option in HeraldryForegrounds)
			c.heraldryForeground.Add(new HeraldryOption(option.storedString, option.GetCityTraits(), option.prevelance));

		c.tileValues = new Dictionary<GroundInfo.GroundTraits, float>();
		foreach (StoredGroundValue groundValue in GroundPropertyValues)
			c.tileValues[(GroundInfo.GroundTraits) System.Enum.Parse(typeof(GroundInfo.GroundTraits), groundValue.groundTrait)] = groundValue.groundValue;

		c.tileModelPlacement = new Dictionary<GroundInfo.GroundTraits, List<ModelPlacementInfo>>();
		foreach (StoredCultureModelPlacementByTrait placementInfo in ModelPlacementByTraitCultureInfos)
		{
			List<ModelPlacementInfo> modelPlacers = new List<ModelPlacementInfo>();
			foreach (StoredModelPlacementInfo info in placementInfo.ModelPlacementInfos)
				modelPlacers.Add(new ModelPlacementInfo(info.model, info.placementMode, info.min, info.max));
			c.tileModelPlacement[(GroundInfo.GroundTraits) System.Enum.Parse(typeof(GroundInfo.GroundTraits), placementInfo.TerrainTrait)] = modelPlacers;
		}

		return c;
	}
}
