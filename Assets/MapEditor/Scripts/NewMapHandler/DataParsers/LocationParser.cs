using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LocationParser
{
	public static Dictionary<string, StoredLocation> ParsedLocations = new Dictionary<string, StoredLocation>();

	public static void LoadLocations()
	{
		ParsedLocations = ParserHelpers.ParseTypes<StoredLocation>("locations");
	}
}

[Serializable]
public class StoredLocation : ParsableData
{
	public string LocationType;
	public string Model;
	public string[] PlacementProperties;
	public StoredStringModel[] LocationPrefixes = new StoredStringModel[0];
	public StoredStringModel[] LocationSuffixes = new StoredStringModel[0];
}
