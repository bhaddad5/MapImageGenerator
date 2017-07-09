using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class SettlementNameOption
{
	public List<Settlement.CityTrait> constraints;
	public string nameChunk;
	public int prevelance;

	public SettlementNameOption(string str, List<Settlement.CityTrait> constr, int odds = 1)
	{
		nameChunk = str;
		constraints = constr;
		prevelance = odds;
	}

	public SettlementNameOption(string str, int odds = 1)
	{
		nameChunk = str;
		constraints = new List<Settlement.CityTrait>();
		prevelance = odds;
	}
}

public class KingdomNameOption
{
	public List<Kingdom.KingdomTrait> constraints;
	public string nameChunk;
	public int prevelance;

	public KingdomNameOption(string str, List<Kingdom.KingdomTrait> constr, int odds = 1)
	{
		nameChunk = str;
		constraints = constr;
		prevelance = odds;
	}

	public KingdomNameOption(string str, int odds = 1)
	{
		nameChunk = str;
		constraints = new List<Kingdom.KingdomTrait>();
		prevelance = odds;
	}
}


public class HeraldryOption
{
	public Texture2D image;
	public List<Settlement.CityTrait> constraints;
	public int prevelance;

	public HeraldryOption(string imagePath, List<Settlement.CityTrait> constr, int odds = 1)
	{
		Byte[] file = File.ReadAllBytes(Application.streamingAssetsPath + "/" + imagePath);
		image = new Texture2D(2, 2);
		image.LoadImage(file);
		prevelance = odds;
		constraints = constr;
	}
}

public class Culture
{
	public string CultureId;
	public string CultureName;

	public List<SettlementNameOption> prefixes;
	public List<SettlementNameOption> suffixes;
	public List<SettlementNameOption> areaInfo;

	public List<KingdomNameOption> kingdomTitles;

	public Texture2D heraldryOverlay;
	public List<HeraldryOption> heraldryBackground;
	public List<HeraldryOption> heraldryForeground;

	public Dictionary<GroundInfo.GroundTraits, float> tileValues;

	public Dictionary<GroundInfo.GroundTraits, List<ModelPlacementInfo>> tileModelPlacement;

	public string GetKingdomName(string coreName, List<Kingdom.KingdomTrait> traits)
	{
		var kingdomTitle = GetKingdomNameChunk(kingdomTitles, traits);
		return kingdomTitle.Replace("%n", coreName);
	}

	private string GetKingdomNameChunk(List<KingdomNameOption> options, List<Kingdom.KingdomTrait> constraints)
	{
		List<KingdomNameOption> usableOptions = new List<KingdomNameOption>();
		foreach (var option in options)
		{
			bool optionGood = true;
			foreach (var constraint in option.constraints)
			{
				if (!constraints.Contains(constraint))
				{
					optionGood = false;
					break;
				}
			}
			if (optionGood)
			{
				for (int i = 0; i < option.prevelance; i++)
					usableOptions.Add(option);
			}
		}

		if (usableOptions.Count > 0)
			return usableOptions[Random.Range(0, usableOptions.Count)].nameChunk;
		else return "NO SUITABLE NAME FOUND!!!";
	}

	private HashSet<string> settlementNamesCreated = new HashSet<string>() { "NoneFound" };
	public KeyValuePair<string, string> GetSettlementName(List<Settlement.CityTrait> constraints)
	{
		string coreName = "NoneFound";
		string finalName = "";
		int sanity = 20;
		int currSanity = 0;

		while(settlementNamesCreated.Contains(coreName) && currSanity <= sanity)
		{
			coreName = GetNameChunk(prefixes, constraints) + GetNameChunk(suffixes, constraints);
			string area = GetNameChunk(areaInfo, constraints);
			finalName = area.Replace("%n", coreName);
			currSanity++;
		}

		return new KeyValuePair<string, string>(coreName, finalName);
	}

	private string GetNameChunk(List<SettlementNameOption> options, List<Settlement.CityTrait> constraints)
	{
		List<SettlementNameOption> usableOptions = new List<SettlementNameOption>();
		foreach (var option in options)
		{
			bool optionGood = true;
			foreach (var constraint in option.constraints)
			{
				if (!constraints.Contains(constraint))
				{
					optionGood = false;
					break;
				}
			}
			if (optionGood)
			{
				for (int i = 0; i < option.prevelance; i++)
					usableOptions.Add(option);
			}
		}

		if (usableOptions.Count > 0)
			return usableOptions[Random.Range(0, usableOptions.Count)].nameChunk;
		else return "NO SUITABLE NAME FOUND!!!";
	}

	public Texture2D GetHeraldry(List<Settlement.CityTrait> constraints, Kingdom region)
	{
		int imageSize = 256;
		var background = GetHeraldryTexture(new Color[imageSize * imageSize], heraldryBackground, region);
		var finalHeraldry = GetHeraldryTexture(background, heraldryForeground, region);

		Texture2D final = new Texture2D(imageSize, imageSize);
		final.SetPixels(finalHeraldry);
		final = ImageHelpers.AlphaBlend(final, heraldryOverlay);
		final.Apply();
		return final;
	}

	private Color[] GetHeraldryTexture(Color[] baseTex, List<HeraldryOption> options, Kingdom region)
	{
		var newTex = options[Random.Range(0, options.Count)].image.GetPixels();

		for (int i = 0; i < baseTex.Length; i++)
		{
			if (newTex[i].a > 0)
				baseTex[i] = (GetActualColor(newTex[i], region));
		}
		return baseTex;
	}

	private Color GetActualColor(Color inColor, Kingdom region)
	{
		if (inColor.r == 1f && inColor.g == 0f && inColor.b == 0f)
			return region.mainColor;
		if (inColor.r == 0f && inColor.g == 1f && inColor.b == 0f)
			return region.secondaryColor;
		else
		{
			Vector4 newColor = new Vector4();
			newColor.x = inColor.r + (1 - inColor.r) * region.tertiaryColor.r;
			newColor.y = inColor.g + (1 - inColor.g) * region.tertiaryColor.g;
			newColor.z = inColor.b + (1 - inColor.b) * region.tertiaryColor.b;
			newColor.w = inColor.a;
			return newColor;
		}
	}

	public float GetTileValue(Int2 tile)
	{
		float value = 0;
		foreach (GroundInfo.GroundTraits trait in MapGenerator.Terrain.Get(tile).traits)
		{
			if(tileValues.ContainsKey(trait))
				value += tileValues[trait];
		}
		return value;
	}

	public float TileAreaValue(Int2 pos, bool includeDiag = false)
	{
		float value = GetTileValue(pos) * 2;

		float oneWaterBorderValue = 3f;
		float someWaterValue = 2f;
		float allWaterValue = -1f;

		int numWaterBorders = 0;
		foreach (Int2 t in MapGenerator.Terrain.GetAdjacentPoints(pos))
		{
			if (MapGenerator.Terrain.Get(t).groundType == "Ocean" || MapGenerator.Terrain.Get(t).groundType == "River")
				numWaterBorders++;
			value += GetTileValue(t);
		}

		if (includeDiag)
		{
			foreach (Int2 t in MapGenerator.Terrain.GetDiagonalPoints(pos))
				value += GetTileValue(t) * .8f;
		}

		if (numWaterBorders == 1)
			value += oneWaterBorderValue;
		else if (numWaterBorders > 1 && numWaterBorders < 4)
			value += someWaterValue;
		else if (numWaterBorders == 4)
			value += allWaterValue;

		return value;
	}
}

public static class ImageHelpers
{
	public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
	{
		if (aBottom.width != aTop.width || aBottom.height != aTop.height)
			throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
		var bData = aBottom.GetPixels();
		var tData = aTop.GetPixels();
		int count = bData.Length;
		var rData = new Color[count];
		for (int i = 0; i < count; i++)
		{
			Color B = bData[i];
			Color T = tData[i];
			float srcF = T.a;
			float destF = 1f - T.a;
			float alpha = srcF + destF * B.a;
			Color R = (T * srcF + B * B.a * destF) / alpha;
			R.a = alpha;
			rData[i] = R;
		}
		var res = new Texture2D(aTop.width, aTop.height);
		res.SetPixels(rData);
		res.Apply();
		return res;
	}
}