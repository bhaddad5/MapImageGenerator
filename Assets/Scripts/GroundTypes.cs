using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GroundTypes
{
	public enum Type
	{
		Ocean,
		River,
		Swamp,
		Mountain,
		Forest,
		Grass,
		Fertile,
		City,
		Road,
	}

	public static float startOceanDifficulty = 0.35f;
	public static Dictionary<Type, Color> tileColors = new Dictionary<Type, Color>()
	{
		{ Type.Ocean, new Color(0, 0, 255/255f) },
		{ Type.River,  new Color(0, 0, 150/255f) },
		{ Type.Swamp, new Color(0, 188/255f, 106/255f) },
		{ Type.Mountain, new Color(144/255f, 92/255f, 0) },
		{ Type.Forest, new Color(0, 130/255f, 0) },
		{ Type.Grass, new Color(0, 0, 0) },
		{ Type.Fertile, new Color(255/255f, 255/255f, 0) },
		{ Type.City, new Color(255/255f, 255/255f, 255/255f) },
		{ Type.Road, new Color(193/255f, 97/255f, 32/255f) },
	};
}

