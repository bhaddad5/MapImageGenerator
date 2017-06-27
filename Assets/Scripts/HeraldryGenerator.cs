using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeraldryGenerator
{
	public static Texture2D GetHeraldry(Culture culture, List<Settlement.CityTrait> constraints, Kingdom region)
	{
		int imageSize = 256;
		var background = GetHeraldryTexture(new Color[imageSize * imageSize], culture.heraldryBackground, region);
		var finalHeraldry = GetHeraldryTexture(background, culture.heraldryForeground, region);

		Texture2D final = new Texture2D(imageSize, imageSize);
		final.SetPixels(finalHeraldry);
		final.Apply();
		return final;
	}

	private static Color[] GetHeraldryTexture(Color[] baseTex, List<HeraldryOption> options, Kingdom region)
	{
		var newTex = options[Random.Range(0, options.Count)].image.GetPixels();

		for(int i = 0; i < baseTex.Length; i++)
		{
			if (newTex[i].a > 0)
				baseTex[i] = (GetActualColor(newTex[i], region));
		}
		return baseTex;
	}

	private static Color GetActualColor(Color inColor, Kingdom region)
	{
		if (inColor == Color.red)
			return region.mainColor;
		if (inColor == Color.blue)
			return region.secondaryColor;
		if (inColor == Color.green)
			return region.tertiaryColor;
		else return inColor;
	}
	
}
