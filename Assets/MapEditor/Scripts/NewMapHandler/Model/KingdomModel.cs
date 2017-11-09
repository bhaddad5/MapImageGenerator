﻿using System;
using UnityEngine;

[Serializable]
public class KingdomModel : ParsableData
{
	public string KingdomName;
	public string CultureId;
	public CultureModel Culture { get { return CultureParser.CultureData[CultureId]; } }
	public string HeraldrySymbol;
	public string HeraldryBackground;
	public Color PrimaryColor;
	public Color SecondaryColor;

	public void SetNamesAndHeraldry()
	{
	}
}
