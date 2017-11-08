using System;
using UnityEngine;

[Serializable]
public class KingdomModel
{
	public string KingdomId;

	public string KingdomName;
	public string CultureId;
	public CultureModel Culture { get { return CultureParser.CultureData[CultureId]; } }
	public string HeraldrySymbol;
	public string HeraldryBackground;
	public Color PrimaryColor;
	public Color SecondaryColor;
}
