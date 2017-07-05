using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Kingdom
{
	public enum KingdomTrait
	{
		OneCity,
		Small,
		Medium,
		Large,
		CapitolPort
	}

	public Culture culture;
	public List<Settlement> settlements = new List<Settlement>();

	public string name;
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

	public void SetNamesAndHeraldry()
	{
		foreach (var sett in settlements)
		{
			var subCityTraits = sett.GetCityTraits();
			var names = culture.GetSettlementName(subCityTraits);
			sett.name = names.Value;
			if(sett == settlements[0])
			{
				SetKingdomName(names.Key);
			}
		}
		if(settlements.Count > 0)
		{
			var cityTraits = settlements[0].GetCityTraits();
			heraldry = culture.GetHeraldry(cityTraits, this);
		}
	}

	private void SetKingdomName(string coreSettlementName)
	{
		name = culture.GetKingdomName(coreSettlementName, GetKingdomTraits());
	}

	public void PrintKingdomInfo()
	{
		foreach (var sett in settlements)
		{
			string adj = "";
			foreach (var se in sett.adjacentSettlements.GetList())
			{
				if (se.Value != sett)
					adj += se.Value.name + ":" + se.Key + ", ";
			}

			Debug.Log(sett.name + " value = " + sett.GetSettlementValue() + ", defense = " + sett.GetSettlementDefensibility() + ", Adjacent to: " + adj);
		}
	}

	public float Strength()
	{
		float strength = 0;
		foreach(var sett in settlements)
		{
			strength += sett.GetSettlementValue();
		}
		return strength;
	}

	public Settlement ClosestEnemySettlement()
	{
		Settlement closest = null;
		float closestDist = 10000f;
		foreach(var sett in settlements)
		{
			foreach(var adj in sett.adjacentSettlements.GetList())
			{
				if (!settlements.Contains(adj.Value))
				{
					var dist = adj.Key;
					if (settlements[0].adjacentSettlements.ContainsValue(adj.Value))
					{
						dist -= 3f;
					}

					if(closest == null || dist < closestDist)
					{
						closest = adj.Value;
						closestDist = dist;
					}
				}
			}
		}
		return closest;
	}

	public void AddSettlement(Settlement sett)
	{
		if (sett.cityTiles.Count > settlements[0].cityTiles.Count + 1)
			settlements.Insert(0, sett);
		else settlements.Add(sett);
	}

	public List<KingdomTrait> GetKingdomTraits()
	{
		List<KingdomTrait> traits = new List<KingdomTrait>();
		if (settlements.Count == 1)
			traits.Add(KingdomTrait.OneCity);
		else if (settlements.Count <= 3)
			traits.Add(KingdomTrait.Small);
		else if (settlements.Count <= 5)
			traits.Add(KingdomTrait.Medium);
		else if (settlements.Count > 5)
			traits.Add(KingdomTrait.Large);

		if(settlements.Count > 0)
		{
			var capitolTraits = settlements[0].GetCityTraits();
			if (capitolTraits.Contains(Settlement.CityTrait.Port))
			{
				traits.Add(KingdomTrait.CapitolPort);
			}
		}
		return traits;
	}

	private Color GetHeraldryColor()
	{
		Color c = RandomColor();
		int minDiff = 200;
		int maxSanity = 30;
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

