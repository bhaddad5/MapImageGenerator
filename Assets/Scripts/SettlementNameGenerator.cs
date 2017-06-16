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

public class Culture
{
	public List<NameOption> prefixes;
	public List<NameOption> suffixes;
	public List<NameOption> areaInfo;

	public string GenerateName(List<Settlement.CityTrait> constraints)
	{		
		string name = GetNameChunk(prefixes, constraints) + GetNameChunk(suffixes, constraints);
		string area = GetNameChunk(areaInfo, constraints);

		return area.Replace("%n", name);
	}

	private string GetNameChunk(List<NameOption> options, List<Settlement.CityTrait> constraints)
	{
		List<NameOption> usableOptions = new List<NameOption>();
		foreach (var option in options)
		{
			bool optionGood = true;
			foreach(var constraint in option.constraints)
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
			return usableOptions[Random.Range(0, usableOptions.Count - 1)].nameChunk;
		else return "NO SUITABLE NAME FOUND!!!";
	}
}

public static class SettlementNameGenerator
{
	public static Culture Anglo = new Culture();
	static bool culturesPopulated = false;
	
	public static void PopulateCultures()
	{
		Anglo.prefixes = new List<NameOption>()
		{
			new NameOption("Lun"),
			new NameOption("Berry"),
			new NameOption("Mannin"),
			new NameOption("Hetten"),
			new NameOption("Stok"),
			new NameOption("Rusk"),
			new NameOption("An"),
			new NameOption("Ewer"),
			new NameOption("How"),
			new NameOption("Setter"),
			new NameOption("Somer"),
		};

		Anglo.suffixes = new List<NameOption>()
		{
			new NameOption("", 3),
			new NameOption("ton"),
			new NameOption("moor"),
			new NameOption("mont", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new NameOption("rent"),
			new NameOption("eter"),
			new NameOption("by"),
			new NameOption("nall"),
			new NameOption("ford", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new NameOption("ham"),
			new NameOption("don"),
			new NameOption("wick"),
			new NameOption("well"),
			new NameOption("light"),
		};

		Anglo.areaInfo = new List<NameOption>()
		{
			new NameOption("%n", new List<Settlement.CityTrait>(), 2),
			new NameOption("Port %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Port }, 3),
			new NameOption("%n Harbor", new List<Settlement.CityTrait> { Settlement.CityTrait.Port }, 3),
			new NameOption("%n Anchorage", new List<Settlement.CityTrait> { Settlement.CityTrait.Port, Settlement.CityTrait.Small }, 3),
			new NameOption("%n Mountain", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new NameOption("Mount %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new NameOption("%nshire", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile }),
			new NameOption("%n Vale", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile }),
			new NameOption("Vale %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile }),
			new NameOption("%n Castle", new List<Settlement.CityTrait> { Settlement.CityTrait.Medium }),
			new NameOption("Castle %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Medium }),
			new NameOption("%n Hold", new List<Settlement.CityTrait> { Settlement.CityTrait.Small }),
			new NameOption("%n Hall", new List<Settlement.CityTrait> { Settlement.CityTrait.Small }),
		};


		culturesPopulated = true;
	}

	public static string GetSettlementName(Culture culture, List<Settlement.CityTrait> constraints)
	{
		if (!culturesPopulated)
			PopulateCultures();

		return culture.GenerateName(constraints);
	}
}
