using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Kingdom
{
	public Settlement settlement;
	public Color mainColor;
	public Color secondaryColor;
	public Color tertiaryColor;
	public float value;

	public Kingdom(string name, Int2 cityTile)
	{
		if(cityTile != null)
			settlement = new Settlement(name, cityTile);
		mainColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

		secondaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
		while(Vector3.Magnitude(new Vector3(mainColor.r, mainColor.g, mainColor.b) - new Vector3(secondaryColor.r, secondaryColor.g, secondaryColor.b)) < 0.2f)
			secondaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

		tertiaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
		while (Vector3.Magnitude(new Vector3(mainColor.r, mainColor.g, mainColor.b) - new Vector3(tertiaryColor.r, tertiaryColor.g, tertiaryColor.b)) < 0.2f &&
			Vector3.Magnitude(new Vector3(secondaryColor.r, secondaryColor.g, secondaryColor.b) - new Vector3(tertiaryColor.r, tertiaryColor.g, tertiaryColor.b)) < 0.2f)
			tertiaryColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}
}

