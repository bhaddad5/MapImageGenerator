using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeraldryGenerator
{
	public static Texture2D GetHeraldry(Culture culture, List<Settlement.CityTrait> constraints, Region region)
	{
		var background = GetHeraldryTexture(CultureDefinitions.GetFullCulture(culture).heraldryBackground, region);

		return background;
	}

	private static Texture2D GetHeraldryTexture(List<HeraldryOption> options, Region region)
	{
		var tex = options[Random.Range(0, options.Count)].image;

		Texture2D copy = new Texture2D(tex.width, tex.height);
		var copyPixels = new List<Color>();
		foreach(var pixel in tex.GetPixels())
		{
			copyPixels.Add(GetActualColor(pixel, region));
		}
		copy.SetPixels(copyPixels.ToArray());
		copy.Apply();

		return copy;
	}

	private static Color GetActualColor(Color inColor, Region region)
	{
		if (inColor == Color.red)
			return region.mainColor;
		if (inColor == Color.blue)
			return region.secondaryColor;
		if (inColor == Color.green)
			return region.tertiaryColor;
		else return new Color(0, 0, 0, 0);
	}
	
}
