using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightsGen
{
	public static Map2D<float> Map;
	private Texture2D heightMapImage;

	public HeightsGen(int width, int height, IHeightGenerator generator)
	{
		Map = generator.GenerateHeightMap(width, height);

		List<Color> pixels = new List<Color>();
		foreach (float h in Map.GetMapValues())
		{
			if (h.Equals(-1))
				pixels.Add(Color.red);
			else pixels.Add(new Color(h, h, h));
		}

		heightMapImage = new Texture2D(width, height);
		heightMapImage.filterMode = FilterMode.Point;
		heightMapImage.anisoLevel = 0;
		heightMapImage.SetPixels(pixels.ToArray());
	}

	public Texture2D GetHeightMapTexture()
	{
		heightMapImage.Apply();
		return heightMapImage;
	}
}