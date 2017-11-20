using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

[Serializable]
public class StoredStringModel
{
	public string StoredString;
	public List<string> Conditions = new List<string>();
}