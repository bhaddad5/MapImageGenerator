using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CultureParser
{
	public static Dictionary<string, CultureModel> CultureData = new Dictionary<string, CultureModel>();

	public static void LoadCultures()
	{
		CultureData = ParserHelpers.ParseTypes<CultureModel>("cultures");
	}
}

[Serializable]
public class SettlementPlacementInfo
{
	public string SettlementType;
	public int PlacementsPer20Square;
}

[Serializable]
public class CultureModel : ParsableData
{
	public string CultureName;

	public StoredTexture HeraldryOverlayImage;
	public List<StoredTexture> HeraldryForegrounds = new List<StoredTexture>();
	public string HeraldryForegroundColorSource;
	public List<StoredTexture> HeraldryBackgrounds = new List<StoredTexture>();
	public string HeraldryBackgroundColorSource;
	public List<SettlementPlacementInfo> SettlementTypes = new List<SettlementPlacementInfo>();
}