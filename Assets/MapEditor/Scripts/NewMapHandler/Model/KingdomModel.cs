using System;
using UnityEngine;

[Serializable]
public class KingdomModel : ParsableData
{
	public string KingdomName;
	public string CultureId;

	public CultureModel Culture()
	{
		return CultureParser.CultureData[CultureId];
	}
	public string HeraldrySymbol;
	public string HeraldryBackground;
	public Vector4 Color1;
	public Vector4 Color2;

	public Color PrimaryColor()
	{
		return new Color(Color1.x, Color1.y, Color1.z, Color1.w);
	}

	public Color SecondaryColor()
	{
		return new Color(Color2.x, Color2.y, Color2.z, Color2.w);
	}

	public void SetNamesAndHeraldry()
	{
	}
}
