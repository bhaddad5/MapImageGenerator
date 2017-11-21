using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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

	public string GetText()
	{
		string res = "";
		char[] chars = TextOptions[UnityEngine.Random.Range(0, TextOptions.Count)].StoredString.ToCharArray();
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

				res += TextChunkParser.TextData[lookup].GetText();
			}
			else res += chars[i];
		}

		return res;
	}
}

[Serializable]
public class StoredStringModel
{
	public string StoredString;
	public List<string> Conditions = new List<string>();
}