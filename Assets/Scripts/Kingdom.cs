using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Kingdom
{
	public Culture culture;
	public List<Settlement> settlements = new List<Settlement>();

	public Color mainColor;
	public Color secondaryColor;
	public Color tertiaryColor;
	public Texture2D heraldry;
	public float value;

	public Kingdom(Culture c, Int2 startingCityTile)
	{
		culture = c;
		if (startingCityTile != null)
			settlements.Add(new Settlement(startingCityTile, this));


		mainColor = GetHeraldryColor();
		secondaryColor = GetHeraldryColor();
		tertiaryColor = GetHeraldryColor();
	}

	public void SetNamesAndHeraldry(Map2D<TerrainTile> terrainMap)
	{
		foreach (var sett in settlements)
		{
			var subCityTraits = sett.GetCityTraits(terrainMap);
			sett.name = culture.GetSettlementName(subCityTraits);
		}
		if(settlements.Count > 0)
		{
			var cityTraits = settlements[0].GetCityTraits(terrainMap);
			heraldry = culture.GetHeraldry(cityTraits, this);
		}
	}

	public void PrintKingdomInfo(Map2D<TerrainTile> terrainMap)
	{
		foreach (var sett in settlements)
		{
			string adj = "";
			foreach (var se in sett.adjacentSettlements.GetList())
			{
				if (se.Value != sett)
					adj += se.Value.name + ":" + se.Key + ", ";
			}

			Debug.Log(sett.name + " value = " + sett.GetSettlementValue(terrainMap) + ", defense = " + sett.GetSettlementDefensibility(terrainMap) + ", Adjacent to: " + adj);
		}
	}

	public float Strength(Map2D<TerrainTile> terrainMap)
	{
		float strength = 0;
		foreach(var sett in settlements)
		{
			strength += sett.GetSettlementValue(terrainMap);
		}
		return strength;
	}

	private Color GetHeraldryColor()
	{
		Color c = RandomColor();
		int minDiff = 100;
		int maxSanity = 20;
		int sanity = 0;
		while (sanity <= maxSanity && (ColorDiff(c, mainColor) < minDiff || ColorDiff(c, secondaryColor) < minDiff || ColorDiff(c, tertiaryColor) < minDiff))
		{
			sanity++;
			c = RandomColor();
		}
		return c;
	}

	private Color RandomColor()
	{
		return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}

	// distance in RGB space.  From: https://stackoverflow.com/questions/27374550/how-to-compare-color-object-and-get-closest-color-in-an-color
	private int ColorDiff(Color c1, Color c2)
	{
		return (int)Mathf.Sqrt((c1.r*255 - c2.r * 255) * (c1.r * 255 - c2.r * 255)
							   + (c1.g * 255 - c2.g * 255) * (c1.g * 255 - c2.g * 255)
							   + (c1.b * 255 - c2.b * 255) * (c1.b * 255 - c2.b * 255));
	}
}

