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
		tileDifficulties = new Dictionary<TerrainTile.TileType, float>()
		{
			{ TerrainTile.TileType.Ocean, .1f },
			{ TerrainTile.TileType.River, .5f },
			{ TerrainTile.TileType.Swamp, .35f },
			{ TerrainTile.TileType.Mountain, .6f },
			{ TerrainTile.TileType.Forest, .25f },
			{ TerrainTile.TileType.Grass, .2f },
			{ TerrainTile.TileType.Fertile, .1f },
			{ TerrainTile.TileType.City, .05f },
			{ TerrainTile.TileType.Road, .05f },
		},
		tileValues = new Dictionary<TerrainTile.TileType, float>()
		{
			{ TerrainTile.TileType.Ocean, 0f },
			{ TerrainTile.TileType.River, 0f },
			{ TerrainTile.TileType.Swamp, .1f },
			{ TerrainTile.TileType.Mountain, .1f },
			{ TerrainTile.TileType.Forest, .2f },
			{ TerrainTile.TileType.Grass, .2f },
			{ TerrainTile.TileType.Fertile, .4f },
			{ TerrainTile.TileType.City, 5f },
			{ TerrainTile.TileType.Road, 3f },
		}
	};



	public static Culture Dwarf = new Culture()
	{
		prefixes = new List<SettlementNameOption>()
		{
			new SettlementNameOption("Daz"),
			new SettlementNameOption("Kurz"),
			new SettlementNameOption("Inuk"),
			new SettlementNameOption("Tar"),
			new SettlementNameOption("Prak"),
			new SettlementNameOption("Wrokk"),
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
			new SettlementNameOption("-bendum"),
			new SettlementNameOption("-silrak", new List<Settlement.CityTrait> { Settlement.CityTrait.Forest }, 4),
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
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.OneCity}),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Small }),
			new KingdomNameOption("%n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Medium }),
			new KingdomNameOption("Empire of %n", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			new KingdomNameOption("The %n Empire", new List<Kingdom.KingdomTrait> { Kingdom.KingdomTrait.Large }),
			
		},

		heraldryBackground = new List<HeraldryOption>()
		{
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_0"),
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_1"),
			new HeraldryOption("Dwarf/Heraldry/Backgrounds/Dwarf_Back_2"),
		},

		heraldryForeground = new List<HeraldryOption>()
		{
			new HeraldryOption("Dwarf/Heraldry/Symbols/Anvil"),
			new HeraldryOption("Dwarf/Heraldry/Symbols/Dragon"),
		},
		tileDifficulties = new Dictionary<TerrainTile.TileType, float>()
		{
			{ TerrainTile.TileType.Ocean, .4f },
			{ TerrainTile.TileType.River, .5f },
			{ TerrainTile.TileType.Swamp, .45f },
			{ TerrainTile.TileType.Mountain, .1f },
			{ TerrainTile.TileType.Forest, .25f },
			{ TerrainTile.TileType.Grass, .2f },
			{ TerrainTile.TileType.Fertile, .2f },
			{ TerrainTile.TileType.City, .05f },
			{ TerrainTile.TileType.Road, .05f },
		},
		tileValues = new Dictionary<TerrainTile.TileType, float>()
		{
			{ TerrainTile.TileType.Ocean, 0f },
			{ TerrainTile.TileType.River, 0f },
			{ TerrainTile.TileType.Swamp, .1f },
			{ TerrainTile.TileType.Mountain, .7f },
			{ TerrainTile.TileType.Forest, .2f },
			{ TerrainTile.TileType.Grass, .2f },
			{ TerrainTile.TileType.Fertile, .2f },
			{ TerrainTile.TileType.City, 5f },
			{ TerrainTile.TileType.Road, 3f },
		}
	};
}
