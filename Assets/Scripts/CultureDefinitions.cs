using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CultureDefinitions
{
	public static Culture Anglo = new Culture()
	{
		prefixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption("Lunn"),
			new SettlementNameOption("Berry"),
			new SettlementNameOption("Mannin"),
			new SettlementNameOption("Hetten"),
			new SettlementNameOption("Stok"),
			new SettlementNameOption("Rusk"),
			new SettlementNameOption("An"),
			new SettlementNameOption("Ewer"),
			new SettlementNameOption("How"),
			new SettlementNameOption("Setter"),
			new SettlementNameOption("Somer"),
		},

		suffixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption("", 3),
			new SettlementNameOption("ton"),
			new SettlementNameOption("moor"),
			new SettlementNameOption("mont", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("rent"),
			new SettlementNameOption("eter"),
			new SettlementNameOption("by"),
			new SettlementNameOption("nall"),
			new SettlementNameOption("ford", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new SettlementNameOption("ham"),
			new SettlementNameOption("dun"),
			new SettlementNameOption("wick"),
			new SettlementNameOption("well"),
			new SettlementNameOption("light"),
		},

		areaInfo = new List<SettlementNameOption>()
		{
			new SettlementNameOption("%n", new List<Settlement.CityTrait>(), 2),
			new SettlementNameOption("Port %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Port, Settlement.CityTrait.Large }, 3),
			new SettlementNameOption("%n Harbor", new List<Settlement.CityTrait> { Settlement.CityTrait.Port, Settlement.CityTrait.Medium }, 3),
			new SettlementNameOption("%n Anchorage", new List<Settlement.CityTrait> { Settlement.CityTrait.Port, Settlement.CityTrait.Small }, 3),
			new SettlementNameOption("%n Mountain", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("Mount %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("%nshire", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile, Settlement.CityTrait.Landlocked, Settlement.CityTrait.Small }),
			new SettlementNameOption("%n Vale", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile, Settlement.CityTrait.Landlocked, Settlement.CityTrait.Small }),
			new SettlementNameOption("Vale %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile, Settlement.CityTrait.Landlocked, Settlement.CityTrait.Small }),
			new SettlementNameOption("%n Castle", new List<Settlement.CityTrait> { Settlement.CityTrait.Large }),
			new SettlementNameOption("Castle %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Large }),
			new SettlementNameOption("%n Hold", new List<Settlement.CityTrait> { Settlement.CityTrait.Medium }),
			new SettlementNameOption("%n Hall", new List<Settlement.CityTrait> { Settlement.CityTrait.Medium }),
			new SettlementNameOption("%n Stead", new List<Settlement.CityTrait> { Settlement.CityTrait.Small }),
			new SettlementNameOption("%n Ford", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new SettlementNameOption("%n Bridge", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new SettlementNameOption("%n Bend", new List<Settlement.CityTrait> { Settlement.CityTrait.Small, Settlement.CityTrait.River }),
			new SettlementNameOption("New %n", new List<Settlement.CityTrait>()),
		},

		kingdomTitles = new List<KingdomNameOption>()
		{
			new KingdomNameOption("Kingdom of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Kingdom of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			new KingdomNameOption("Empire of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			new KingdomNameOption("The %n Empire", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			new KingdomNameOption("Crown of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Principality of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Principality of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("Grand Barony of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Barony of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Barony of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("Dukedom of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("The %n Prices", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Trade Princes of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium, Kingdom.KingdomTrait.CapitolPort }),
			new KingdomNameOption("Trade Princes of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small, Kingdom.KingdomTrait.CapitolPort }),
			new KingdomNameOption("Trade Council of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small, Kingdom.KingdomTrait.CapitolPort }),
			new KingdomNameOption("Trade Council of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity, Kingdom.KingdomTrait.CapitolPort }),
			new KingdomNameOption("Council of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("Free City of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
		},

		heraldryBackground = new List<HeraldryOption>()
		{
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_0"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_1"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_2"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_3"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_4"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_5"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_5"),
			new HeraldryOption("Anglo/Heraldry/Backgrounds/Anglo_Back_Ocean", new List<Settlement.CityTrait> { Settlement.CityTrait.Port }),
		},

		heraldryForeground = new List<HeraldryOption>()
		{
			new HeraldryOption("Anglo/Heraldry/Symbols/Lion"),
			new HeraldryOption("Anglo/Heraldry/Symbols/Blank"),
			new HeraldryOption("Anglo/Heraldry/Symbols/Dragon"),
			new HeraldryOption("Anglo/Heraldry/Symbols/EagleHead"),
			new HeraldryOption("Anglo/Heraldry/Symbols/Feather"),
			new HeraldryOption("Anglo/Heraldry/Symbols/Gryphon"),
			new HeraldryOption("Anglo/Heraldry/Symbols/Horse"),
		},
	};
}
