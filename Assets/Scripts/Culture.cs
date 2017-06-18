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
}
