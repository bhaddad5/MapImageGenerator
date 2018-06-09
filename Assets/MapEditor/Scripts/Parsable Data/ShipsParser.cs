using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsParser
{
	public static Dictionary<string, ShipModel> ParsedShips = new Dictionary<string, ShipModel>();

	public static void ParseShips()
	{
		ParsedShips = ParserHelpers.ParseTypes<ShipModel>("ships");
	}
}

[Serializable]
public class ShipModel : ParsableData
{
	public string Model;
	public string Name;
}
