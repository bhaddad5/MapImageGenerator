﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInstantiationController : MonoBehaviour
{
	public SettlementInfoDisplay SettlementInfoDisplay;

	public void DisplayText(Vector3 position, SettlementTextModel textModel, Transform textParent)
	{
		if (textModel is SettlementTextModel)
		{
			SettlementInfoDisplay display = Instantiate(SettlementInfoDisplay, textParent);
			display.settlementDescr.text = textModel.SettlementDescription;
			display.settlementName.text = textModel.Text;
			display.kingdomName.text = textModel.KingdomName;
			display.heraldry.sprite = GetHeraldryTexture(textModel);
			display.transform.position = position;
			display.crown.gameObject.SetActive(textModel.Capitol);
		}
	}

	private Sprite GetHeraldryTexture(SettlementTextModel textModel)
	{
		Map2D<Color> background = textModel.BackgroundTexture.GetTexture();
		Map2D<Color> foreground = textModel.ForegroundTexture.GetTexture();
		Map2D<Color> overlay = textModel.OverlayTexture.GetTexture();

		Map2D<Color> result = new Map2D<Color>(background);
		foreach (Int2 mapPoint in result.GetMapPoints())
		{
			Color c = result.Get(mapPoint);
			if (c.r.Equals(1f))
				c = textModel.BackgroundColor1;
			else if (c.b.Equals(1f))
				c = textModel.BackgroundColor2;

			Color foreBase = foreground.Get(mapPoint);
			Color fore = new Color();
			fore.r = foreBase.r + (1 - foreBase.r) * textModel.ForegroundColor.x;
			fore.g = foreBase.g + (1 - foreBase.g) * textModel.ForegroundColor.y;
			fore.b = foreBase.b + (1 - foreBase.b) * textModel.ForegroundColor.z;
			fore.a = foreBase.a;

			c = OverlayColors(c, fore);
			c = OverlayColors(c, overlay.Get(mapPoint));
			result.Set(mapPoint, c);
		}

		Texture2D tex = new Texture2D(result.Width, result.Height);
		tex.SetPixels(result.GetMapValuesFlipped().ToArray());
		tex.Apply();
		return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
	}

	private Color OverlayColors(Color orig, Color over)
	{
		float srcF = over.a;
		float destF = 1f - over.a;
		float alpha = srcF + destF * orig.a;
		Color R = (over * srcF + orig * orig.a * destF) / alpha;
		R.a = alpha;
		return R;
	}
}
