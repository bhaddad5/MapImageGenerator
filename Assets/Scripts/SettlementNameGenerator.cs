using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettlementNameGenerator
{
	public static string GetSettlementName(Culture culture, List<Settlement.CityTrait> constraints)
	{
		CultureDefinitions.GetFullCulture(culture);
		string name = GetNameChunk(culture.prefixes, constraints) + GetNameChunk(culture.suffixes, constraints);
		string area = GetNameChunk(culture.areaInfo, constraints);

		return area.Replace("%n", name);
	}

	private static string GetNameChunk(List<NameOption> options, List<Settlement.CityTrait> constraints)
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
}
