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
public class StoredCultureEntityPlacementByTrait
{
	public string TerrainTrait;
	public EntityPlacementModel[] Entity;
}

[Serializable]
public class CultureModel : ParsableData
{
	public string CultureName;

	public string HeraldryOverlayImage;
	public List<StoredTexture> HeraldryForegrounds = new List<StoredTexture>();
	public List<StoredTexture> HeraldryBackgrounds = new List<StoredTexture>();

	/*public float TileAreaValue(Int2 pos, MapModel Map)
	{
		float value = GetTileValue(pos, Map) * 2;

		float oneWaterBorderValue = 3f;
		float someWaterValue = 2f;
		float allWaterValue = -1f;

		int numWaterBorders = 0;
		foreach (Int2 t in Map.Map.GetAdjacentPoints(pos))
		{
			if (Map.Map.Get(t).HasTrait(MapTileModel.TileTraits.Ocean) || Map.Map.Get(t).HasTrait(MapTileModel.TileTraits.River))
				numWaterBorders++;
			value += GetTileValue(t, Map);
		}

		if (numWaterBorders == 1)
			value += oneWaterBorderValue;
		else if (numWaterBorders > 1 && numWaterBorders < 4)
			value += someWaterValue;
		else if (numWaterBorders == 4)
			value += allWaterValue;

		return value;
	}

	public float GetTileValue(Int2 tile, MapModel Map)
	{
		float value = 0;
		foreach (string trait in Map.Map.Get(tile).Terrain().Traits)
		{
			if (GroundPropertyValues.ContainsKey(trait))
				value += GroundPropertyValues[trait];
		}
		return value;
	}*/
}