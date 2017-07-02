using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameOption
{
	public List<Settlement.CityTrait> constraints;
	public string nameChunk;
	public int prevelance;

	public NameOption(string str, List<Settlement.CityTrait> constr, int odds = 1)
	{
		nameChunk = str;
		constraints = constr;
		prevelance = odds;
	}

	public NameOption(string str, int odds = 1)
	{
		nameChunk = str;
		constraints = new List<Settlement.CityTrait>();
		prevelance = odds;
	}
}

public class HeraldryOption
{
	public Texture2D image;
	public List<Settlement.CityTrait> constraints;
	public int prevelance;

	public HeraldryOption(string imageName, int odds = 1)
	{
		image = (Texture2D)Resources.Load(imageName, typeof(Texture2D));
		prevelance = odds;
	}

	public HeraldryOption(string imageName, List<Settlement.CityTrait> constr, int odds = 1)
	{
		image = (Texture2D)Resources.Load(imageName, typeof(Texture2D));
		prevelance = odds;
		constraints = constr;
	}
}

public class Culture
{
	public List<NameOption> prefixes;
	public List<NameOption> suffixes;
	public List<NameOption> areaInfo;

	public List<HeraldryOption> heraldryBackground;
	public List<HeraldryOption> heraldryForeground;

	private HashSet<string> namesCreated = new HashSet<string>() { "NoneFound" };

	public string GetSettlementName(List<Settlement.CityTrait> constraints)
	{
		string finalName = "NoneFound";
		int sanity = 20;
		int currSanity = 0;

		while(namesCreated.Contains(finalName) && currSanity <= sanity)
		{
			string name = GetNameChunk(prefixes, constraints) + GetNameChunk(suffixes, constraints);
			string area = GetNameChunk(areaInfo, constraints);
			finalName = area.Replace("%n", name);
			currSanity++;
		}

		return finalName;
	}

	private string GetNameChunk(List<NameOption> options, List<Settlement.CityTrait> constraints)
	{
		List<NameOption> usableOptions = new List<NameOption>();
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
		if (inColor == Color.red)
			return region.mainColor;
		if (inColor == Color.blue)
			return region.secondaryColor;
		if (inColor == Color.green)
			return region.tertiaryColor;
		else return inColor;
	}
}
