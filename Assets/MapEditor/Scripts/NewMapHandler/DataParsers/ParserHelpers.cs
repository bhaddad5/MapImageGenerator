using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ParserHelpers
{
	public static string ClearOutComments(string file)
	{
		return RemoveBetween(file, "//", "\n");
	}

	private static string RemoveBetween(string s, string begin, string end)
	{
		Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
		return regex.Replace(s, string.Empty);
	}

	public static Dictionary<string, T> ParseTypes<T>(string fileName)
		where T : ParsableData
	{
		Dictionary<string, T> parsedResults = new Dictionary<string, T>();

		string locationsFile = File.ReadAllText(Application.streamingAssetsPath + "/" + fileName + ".txt");
		locationsFile = ParserHelpers.ClearOutComments(locationsFile);
		string[] locations = locationsFile.Split(new[] { "|" }, StringSplitOptions.None);
		foreach (string md in locations)
		{
			T store = JsonUtility.FromJson<T>(md);
			parsedResults[store.Id] = store;
		}

		return parsedResults;
	}
}

[Serializable]
public class ParsableData
{
	public string Id;
}
