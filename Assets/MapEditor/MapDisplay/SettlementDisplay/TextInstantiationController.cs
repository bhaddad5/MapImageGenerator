using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInstantiationController : MonoBehaviour
{
	public SettlementInfoDisplay SettlementInfoDisplay;
	public Sprite Transparent;
	private Dictionary<string, Sprite> instantiatedHeraldry = new Dictionary<string, Sprite>();

	public void DisplayText(Vector3 position, SettlementTextModel textModel, Transform textParent)
	{
		if (textModel is SettlementTextModel)
		{
			SettlementInfoDisplay display = Instantiate(SettlementInfoDisplay, textParent);
			display.SettlementDescr.text = textModel.SettlementDescription;
			display.SettlementName.text = textModel.Text;
			display.KingdomName.text = textModel.KingdomName;
			display.transform.position = position;
			display.Crown.gameObject.SetActive(textModel.Capitol);
			display.KingdomHeraldry.sprite = GetHeraldryTexture(textModel.KingdomHeraldry);
			if (textModel.KingdomHeraldry.GetKey() != textModel.SettlementHeraldry.GetKey())
				display.ProvinceHeraldry.sprite = GetHeraldryTexture(textModel.SettlementHeraldry);
			else display.ProvinceHeraldry.sprite = Transparent;
		}
	}

	private Sprite GetHeraldryTexture(HeraldryModel heraldry)
	{
		if (instantiatedHeraldry.ContainsKey(heraldry.GetKey()))
			return instantiatedHeraldry[heraldry.GetKey()];

		Map2D<Color> background = heraldry.BackgroundTexture.GetTexture();
		Map2D<Color> foreground = heraldry.ForegroundTexture.GetTexture();
		Map2D<Color> overlay = heraldry.OverlayTexture.GetTexture();

		Map2D<Color> result = new Map2D<Color>(background);
		foreach (Int2 mapPoint in result.GetMapPoints())
		{
			Color c = result.Get(mapPoint);
			if (c.r.Equals(1f))
				c = heraldry.BackgroundColor1;
			else if (c.b.Equals(1f))
				c = heraldry.BackgroundColor2;

			Color foreBase = foreground.Get(mapPoint);
			Color fore = new Color();
			fore.r = foreBase.r + (1 - foreBase.r) * heraldry.ForegroundColor.x;
			fore.g = foreBase.g + (1 - foreBase.g) * heraldry.ForegroundColor.y;
			fore.b = foreBase.b + (1 - foreBase.b) * heraldry.ForegroundColor.z;
			fore.a = foreBase.a;

			c = OverlayColors(c, fore);
			c = OverlayColors(c, overlay.Get(mapPoint));
			result.Set(mapPoint, c);
		}

		Texture2D tex = new Texture2D(result.Width, result.Height);
		tex.SetPixels(result.GetMapValuesFlipped().ToArray());
		tex.Apply();
		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
		instantiatedHeraldry[heraldry.GetKey()] = sprite;
		return sprite;
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
