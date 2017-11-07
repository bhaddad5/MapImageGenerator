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
		string locationsFile = File.ReadAllText(Application.streamingAssetsPath + "/locations.txt");
		locationsFile = ParserHelpers.ClearOutComments(locationsFile);
		string[] locations = locationsFile.Split(new[] { "|" }, StringSplitOptions.None);
		foreach (string md in locations)
		{
			StoredLocation store = JsonUtility.FromJson<StoredLocation>(md);
			ParsedLocations[store.LocationType] = store;
		}
	}
}

[Serializable]
public class StoredLocation
{
	public string LocationType;
	public string Model;
	public string[] PlacementProperties;
	public StoredStringOption[] LocationPrefixes = new StoredStringOption[0];
	public StoredStringOption[] LocationSuffixes = new StoredStringOption[0];
}

public class LocationType
{
	public string LocationId;
	public GameObject Model;
	public List<StoredStringOption> LocationPrefixes = new List<StoredStringOption>();
	public List<StoredStringOption> LocationSuffixes = new List<StoredStringOption>();

	public LocationType(string id, string model, StoredStringOption[] prefixes, StoredStringOption[] suffixes)
	{
		LocationId = id;

		if (ModelLookup.Models.ContainsKey(model))
			Model = ModelLookup.Models[model];
		if (Model == null)
			Debug.Log(model);

		LocationPrefixes = prefixes.ToList();
		LocationSuffixes = suffixes.ToList();
	}

	public Location GetLocation()
	{
		string name = LocationPrefixes.RandomValue().storedString.Replace("%n", LocationSuffixes.RandomValue().storedString);
		return new Location(name, Model);
	}
}
