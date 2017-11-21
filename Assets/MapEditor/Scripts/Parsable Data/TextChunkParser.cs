using System;
using System.Collections.Generic;
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

	public string GetText(List<string> traits)
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

				res += TextChunkParser.TextData[lookup].GetText(traits);
			}
			else res += chars[i];
		}

		return res;
	}

	private string GetRandomTextOption(List<string> traits)
	{
		int startCheck = UnityEngine.Random.Range(0, TextOptions.Count);
		for (int i = 0; i < TextOptions.Count; i++)
		{
			int offset = i;
			if (i >= TextOptions.Count - startCheck)
				offset = i - TextOptions.Count;

			bool valid = true;
			foreach (string condition in TextOptions[startCheck + offset].Conditions)
			{
				if (!traits.Contains(condition))
					valid = false;
			}

			if (valid)
				return TextOptions[startCheck + offset].StoredString;
		}

		return TextOptions.First().StoredString;
	}
}

[Serializable]
public class StoredStringModel
{
	public string StoredString;
	public List<string> Conditions = new List<string>();
}