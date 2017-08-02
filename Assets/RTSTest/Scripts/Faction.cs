using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour
{
	public Color factionColor;
	public Texture2D overlayLayer;
	public Texture2D colorLayer;
	public Texture2D backgroundLayer;

	private Texture2D TroopTexture;

	public Texture2D GetTroopTexture()
	{
		if (TroopTexture == null)
		{
			Texture2D finalColorTex = new Texture2D(colorLayer.width, colorLayer.height);
			List<Color> coloredPixels = new List<Color>();
			foreach (Color pixel in colorLayer.GetPixels())
			{
				if(pixel == Color.white)
					coloredPixels.Add(factionColor);
				else coloredPixels.Add(new Color(0, 0, 0, 0));
			}
			finalColorTex.SetPixels(coloredPixels.ToArray());
			finalColorTex.Apply();

			TroopTexture = new Texture2D(backgroundLayer.width, backgroundLayer.height);
			TroopTexture = backgroundLayer.AlphaBlend(finalColorTex);
			TroopTexture.Apply();
			TroopTexture = TroopTexture.AlphaBlend(overlayLayer);
			TroopTexture.Apply();
		}
		return TroopTexture;
	}

}
