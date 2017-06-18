using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CultureDefinitions
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
			new NameOption("%n Ford", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new NameOption("%n Bridge", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new NameOption("%n Bend", new List<Settlement.CityTrait> { Settlement.CityTrait.Small, Settlement.CityTrait.River }),
		};

		Anglo.heraldryBackground = new List<HeraldryOption>()
		{
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_0"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_1"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_2"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_3"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_4"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_5"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_5"),
			new HeraldryOption("Heraldry/Backgrounds/Anglo_Back_Ocean", new List<Settlement.CityTrait> { Settlement.CityTrait.Port }),
		};

		Anglo.heraldryForeground = new List<HeraldryOption>()
		{
			new HeraldryOption("Heraldry/Symbols/Lion"),
			new HeraldryOption("Heraldry/Symbols/Blank"),
			new HeraldryOption("Heraldry/Symbols/Dragon"),
			new HeraldryOption("Heraldry/Symbols/EagleHead"),
			new HeraldryOption("Heraldry/Symbols/Feather"),
			new HeraldryOption("Heraldry/Symbols/Gryphon"),
			new HeraldryOption("Heraldry/Symbols/Horse"),
		};

		culturesPopulated = true;
	}

	public static Culture GetFullCulture(Culture c)
	{
		if (!culturesPopulated)
			PopulateCultures();
		return c;
	}
}
