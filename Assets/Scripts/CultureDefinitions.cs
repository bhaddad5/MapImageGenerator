using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CultureDefinitions
{
	public static Culture Anglo = new Culture()
	{
		CultureName = "Humans",
		AttackMultiplier = 1f,
		DefenseMultiplier = 1f,
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
			new SettlementNameOption("Wend"),
			new SettlementNameOption("Torrent"),
			new SettlementNameOption("Comb"),
			new SettlementNameOption("Slak"),
			new SettlementNameOption("Brix"),
			new SettlementNameOption("Bux"),
			new SettlementNameOption("Win"),
			new SettlementNameOption("Fur"),
			new SettlementNameOption("Hather"),
		},

		suffixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption(""),
			new SettlementNameOption("ton"),
			new SettlementNameOption("moor"),
			new SettlementNameOption("mont", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("rent"),
			new SettlementNameOption("eter"),
			new SettlementNameOption("by"),
			new SettlementNameOption("nall"),
			new SettlementNameOption("ford", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new SettlementNameOption("brook", new List<Settlement.CityTrait> { Settlement.CityTrait.River }),
			new SettlementNameOption("field", new List<Settlement.CityTrait> { Settlement.CityTrait.Fertile }),
			new SettlementNameOption("ham"),
			new SettlementNameOption("dun"),
			new SettlementNameOption("wick"),
			new SettlementNameOption("well"),
			new SettlementNameOption("light"),
			new SettlementNameOption("sor"),
			new SettlementNameOption("ley"),
			new SettlementNameOption("end"),
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
			new SettlementNameOption("%n Mill", new List<Settlement.CityTrait> { Settlement.CityTrait.Small, Settlement.CityTrait.Fertile }),
			new SettlementNameOption("%n Heath", new List<Settlement.CityTrait> { Settlement.CityTrait.Small }),
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

		heraldryOverlay = "Anglo/Heraldry/Overlay",

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
		tileValues = new Dictionary<GroundInfo.GroundTraits, float>()
		{
			{ GroundInfo.GroundTraits.Water, 0f },
			{ GroundInfo.GroundTraits.Impassable, 0f },
			{ GroundInfo.GroundTraits.Muddy, .1f },
			{ GroundInfo.GroundTraits.Rocky, .1f },
			{ GroundInfo.GroundTraits.Hunting, .2f },
			{ GroundInfo.GroundTraits.Fertile, .4f },
			{ GroundInfo.GroundTraits.City, 5f },
			{ GroundInfo.GroundTraits.Road, 3f },
		}
	};



	public static Culture Dwarf = new Culture()
	{
		CultureName = "Dwarves",
		AttackMultiplier = 1f,
		DefenseMultiplier = 1f,
		prefixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption("Daz"),
			new SettlementNameOption("Kurz"),
			new SettlementNameOption("Inuk"),
			new SettlementNameOption("Tar"),
			new SettlementNameOption("Prak"),
			new SettlementNameOption("Wrokk"),
			new SettlementNameOption("Mentung"),
			new SettlementNameOption("Jekrat"),
			new SettlementNameOption("Kazdul"),
			new SettlementNameOption("Kazan"),
			new SettlementNameOption("Kazren"),
			new SettlementNameOption("Azorn"),
			new SettlementNameOption("Azdal"),
			new SettlementNameOption("Azlaak"),
			new SettlementNameOption("Kajar"),
			new SettlementNameOption("Draken"),
		},

		suffixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption(""),
			new SettlementNameOption("-kull", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }, 4),
			new SettlementNameOption("-kuzdal", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }, 4),
			new SettlementNameOption("-tazul"),
			new SettlementNameOption("-mikdun"),
			new SettlementNameOption("-trokk"),
			new SettlementNameOption("-pirok"),
			new SettlementNameOption("-tarmuk"),
			new SettlementNameOption("-ostrok"),
			new SettlementNameOption("-restrok"),
			new SettlementNameOption("-clarrok"),
			new SettlementNameOption("-sorrok"),
			new SettlementNameOption("-bendum"),
			new SettlementNameOption("-silrak", new List<Settlement.CityTrait> { Settlement.CityTrait.Forest }, 4),
			new SettlementNameOption("-revvrok", new List<Settlement.CityTrait> { Settlement.CityTrait.River }, 4),
			new SettlementNameOption("-serok", new List<Settlement.CityTrait> { Settlement.CityTrait.Port }, 4),
		},

		areaInfo = new List<SettlementNameOption>()
		{
			new SettlementNameOption("%n", new List<Settlement.CityTrait>(), 4),
			new SettlementNameOption("%n Post", new List<Settlement.CityTrait> {Settlement.CityTrait.Small }),
			new SettlementNameOption("%n Outpost", new List<Settlement.CityTrait> {Settlement.CityTrait.Small }),
			new SettlementNameOption("Kadrul %n", new List<Settlement.CityTrait> {Settlement.CityTrait.Large }),
			new SettlementNameOption("Nagut %n", new List<Settlement.CityTrait> {Settlement.CityTrait.Port }),
			new SettlementNameOption("%n Nagut", new List<Settlement.CityTrait> {Settlement.CityTrait.Port }),
			new SettlementNameOption("%n Mountain", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("Halls of %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("Mines of %n", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
		},

		kingdomTitles = new List<KingdomNameOption>()
		{
			new KingdomNameOption("Kingdom of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("Kingdom of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Kingdom of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			new KingdomNameOption("%n Expidition", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("Freehalls of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("Guilds of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("League of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("League of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("The Empire of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),

		},

		heraldryOverlay = "Dwarf/Heraldry/Overlay",

		heraldryBackground = new List<HeraldryOption>()
		{
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_0"),
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_1"),
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_2"),
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_3"),
		},

		heraldryForeground = new List<HeraldryOption>()
		{
			new HeraldryOption("Dwarf/Heraldry/Symbols/Anvil"),
			new HeraldryOption("Dwarf/Heraldry/Symbols/Dragon"),
			new HeraldryOption("Dwarf/Heraldry/Symbols/DragonSkull"),
			new HeraldryOption("Dwarf/Heraldry/Symbols/AleHorn"),
			new HeraldryOption("Dwarf/Heraldry/Symbols/Helmet"),
		},
		tileValues = new Dictionary<GroundInfo.GroundTraits, float>()
		{
			{ GroundInfo.GroundTraits.Water, 0f },
			{ GroundInfo.GroundTraits.Impassable, 0f },
			{ GroundInfo.GroundTraits.Muddy, 0f },
			{ GroundInfo.GroundTraits.Rocky, .6f },
			{ GroundInfo.GroundTraits.Hunting, .2f },
			{ GroundInfo.GroundTraits.Fertile, .2f },
			{ GroundInfo.GroundTraits.City, 6f },
			{ GroundInfo.GroundTraits.Road, 3f },
		}
	};



	public static Culture Orc = new Culture()
	{
		CultureName = "Orcs",
		AttackMultiplier = 1f,
		DefenseMultiplier = 1f,
		prefixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption("Org"),
			new SettlementNameOption("Trog"),
			new SettlementNameOption("Kolog"),
			new SettlementNameOption("Dolg"),
			new SettlementNameOption("Riek"),
			new SettlementNameOption("Warg"),
			new SettlementNameOption("Urg"),
			new SettlementNameOption("Prak"),
			new SettlementNameOption("Olgon"),
			new SettlementNameOption("Drak"),
		},

		suffixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption(""),
			new SettlementNameOption("uz", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }, 4),
			new SettlementNameOption("az", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }, 4),
			new SettlementNameOption("as"),
			new SettlementNameOption("isdun"),
			new SettlementNameOption("is"),
			new SettlementNameOption("lok"),
			new SettlementNameOption("dol"),
			new SettlementNameOption("tol"),
			new SettlementNameOption("sil", new List<Settlement.CityTrait> { Settlement.CityTrait.Forest }, 4),
		},

		areaInfo = new List<SettlementNameOption>()
		{
			new SettlementNameOption("%n", new List<Settlement.CityTrait>(), 4),
			new SettlementNameOption("%n Camp", new List<Settlement.CityTrait> {Settlement.CityTrait.Small }),
			new SettlementNameOption("Camp %n", new List<Settlement.CityTrait> {Settlement.CityTrait.Small }),
			new SettlementNameOption("%n Warcamp", new List<Settlement.CityTrait> {Settlement.CityTrait.Medium }),
			new SettlementNameOption("%n Hold", new List<Settlement.CityTrait> {Settlement.CityTrait.Medium }),
			new SettlementNameOption("%n Hold", new List<Settlement.CityTrait> {Settlement.CityTrait.Large }),
			new SettlementNameOption("%n Docks", new List<Settlement.CityTrait> {Settlement.CityTrait.Port }),
			new SettlementNameOption("%n Mountain", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("%n Peak", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
			new SettlementNameOption("%n Pit", new List<Settlement.CityTrait> { Settlement.CityTrait.Mountains }),
		},

		kingdomTitles = new List<KingdomNameOption>()
		{
			new KingdomNameOption("Despot of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("%n Clan", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("%n Clan", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("%n Clans", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("%n Tribe", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n Tribe", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Clans of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			new KingdomNameOption("%n Warband", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Shamans of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n Empire", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),

		},

		heraldryOverlay = "Orc/Heraldry/Overlay",

		heraldryBackground = new List<HeraldryOption>()
		{
			new HeraldryOption("Orc/Heraldry/Backgrounds/Orc_Background_0"),
			new HeraldryOption("Orc/Heraldry/Backgrounds/Orc_Background_1"),
			new HeraldryOption("Orc/Heraldry/Backgrounds/Orc_Background_2"),
			new HeraldryOption("Orc/Heraldry/Backgrounds/Orc_Background_3"),
		},

		heraldryForeground = new List<HeraldryOption>()
		{
			new HeraldryOption("Orc/Heraldry/Symbols/DragonFace"),
			new HeraldryOption("Orc/Heraldry/Symbols/Dragon"),
			new HeraldryOption("Orc/Heraldry/Symbols/Skull"),
		},
		tileValues = new Dictionary<GroundInfo.GroundTraits, float>()
		{
			{ GroundInfo.GroundTraits.Water, 0f },
			{ GroundInfo.GroundTraits.Impassable, 0f },
			{ GroundInfo.GroundTraits.Muddy, .4f },
			{ GroundInfo.GroundTraits.Rocky, .3f },
			{ GroundInfo.GroundTraits.Hunting, .35f },
			{ GroundInfo.GroundTraits.Fertile, .15f },
			{ GroundInfo.GroundTraits.City, 4f },
			{ GroundInfo.GroundTraits.Road, 2f },
		}
	};
}