using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorOptionsParser
{
	public static Dictionary<string, ColorOptionsModel> ColorOptions = new Dictionary<string, ColorOptionsModel>();

	public static void ParseColorOptions()
	{
		ColorOptions = ParserHelpers.ParseTypes<ColorOptionsModel>("colorOptions");
	}
}

[Serializable]
public class ColorOptionsModel : ParsableData
{
	public List<Color32> Colors = new List<Color32>();

	public Color GetRandColor()
	{
		return Colors[UnityEngine.Random.Range(0, Colors.Count)];
	}
}