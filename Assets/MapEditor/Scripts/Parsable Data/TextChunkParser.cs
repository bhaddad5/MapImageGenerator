using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class TextChunkParser
{
	public static Dictionary<string, TextChunkModel> TextData = new Dictionary<string, TextChunkModel>();

	public static void LoadTextChunks()
	{
		TextData = ParserHelpers.ParseTypes<TextChunkModel>("textChunks");
	}
}

[Serializable]
public class TextChunkModel : ParsableData
{
	public List<StoredStringModel> TextOptions = new List<StoredStringModel>();

	public string GetText(List<string> traits, string n = null)
	{
		string res = "";
		char[] chars = GetRandomTextOption(traits).ToCharArray();
		for (int i = 0; i < chars.Length; i++)
		{
			if (chars[i] == '%')
			{
				string lookup = "";
				for (int j = i + 1; j < chars.Length; j++)
				{
					if (chars[j] == '%')
					{
						i = j;
						break;
					}
					else lookup += chars[j];
				}
				if (lookup == "n")
				{
					res += n;
				}
				else
				{
					if(!TextChunkParser.TextData.ContainsKey(lookup))
						UnityEngine.Debug.LogError("Cannot find text lookup table: " + lookup);
					res += TextChunkParser.TextData[lookup].GetText(traits);
				}
			}
			else res += chars[i];
		}

		return res;
	}

	private string GetRandomTextOption(List<string> traits)
	{
		List<string> validOptions = new List<string>();

		foreach (StoredStringModel option in TextOptions)
		{
			bool validOption = true;
			foreach (string condition in option.Conditions)
			{
				validOption = traits.Contains(condition);
			}
			if (validOption)
				validOptions.Add(option.StoredString);
		}

		if(validOptions.Count == 0)
			validOptions.Add("!NO VALID STRING FOUND!");

		return validOptions[UnityEngine.Random.Range(0, validOptions.Count)];
	}
}

[Serializable]
public class StoredStringModel
{
	public string StoredString;
	public List<string> Conditions = new List<string>();
}