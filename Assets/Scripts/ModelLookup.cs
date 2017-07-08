using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLookup
{
	public Dictionary<string, string> Lookup = new Dictionary<string, string>();

	public GameObject LookupModel(string str)
	{
		GameObject g = Resources.Load(Lookup[str]) as GameObject;
		if(g == null)
			Debug.Log(str);
		return g;
	}
}

public static class LookupDefinitions
{
	public static ModelLookup Lookup = new ModelLookup()
	{
		Lookup =  new Dictionary<string, string>()
		{
			{"OrcHut", "Models/Orcs/OrcHut" },
			{"OrcWall", "Models/Orcs/OrcWoodenWall" },
			{"OrcGate", "Models/Orcs/OrcWoodenGate" },
			{"OrcTower", "Models/Orcs/OrcTower" },
			{"DwarfHouse", "Models/Dwarves/DwarfHouse" },
			{"DwarfWall", "Models/Dwarves/Dwarf Wall" },
			{"DwarfGate", "Models/Dwarves/Dwarf Gates" },
			{"DwarfTower", "Models/Dwarves/Dwarf Tower" },
			{"Pine", "Models/Nature/PineTree" },
			{"Rushes", "Models/Nature/Rushes" },
			{"Willow", "Models/Nature/Willow" },
			{"WheatField", "Models/Nature/WheatField" },
			{"AngloHovel", "Models/Anglo/Hovel" },
			{"AngloStoneWall", "Models/Anglo/Wall" },
			{"AngloStoneGate", "Models/Anglo/Gates" },
			{"AngloStoneTurret", "Models/Anglo/Turret" },
			{"AngloTownHouse", "Models/Anglo/CityHouse" },
			{"AngloWoodenWall", "Models/Anglo/WoodenWall" },
			{"AngloWoodenGate", "Models/Anglo/WoodenGate" },
			{"AngloBridge", "Models/Anglo/Bridge" },
			{"GiantMushroom", "Models/Cave/GiantMushroom" },
		}
	};
}