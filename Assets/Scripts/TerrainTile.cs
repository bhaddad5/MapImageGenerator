using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TerrainTile
{
	public enum TileType
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
	public static Dictionary<Color, TileType> tileColors = new Dictionary<Color, TileType>()
	{
		{ new Color(0, 0, 255/255f), TileType.Ocean },
		{ new Color(0, 0, 150/255f), TileType.River },
		{ new Color(0, 188/255f, 106/255f), TileType.Swamp },
		{ new Color(144/255f, 92/255f, 0), TileType.Mountain },
		{ new Color(0, 130/255f, 0), TileType.Forest },
		{ new Color(0, 0, 0), TileType.Grass},
		{ new Color(255/255f, 255/255f, 0), TileType.Fertile },
		{ new Color(255/255f, 255/255f, 255/255f), TileType.City },
		{ new Color(193/255f, 97/255f, 32/255f), TileType.Road },
	};

	public Color GetTileColor()
	{
		foreach(var colorPair in tileColors)
		{
			if (colorPair.Value == tileType)
				return colorPair.Key;
		}
		return new Color(1f, .4f, .6f);
	}

	public TileType tileType = TileType.Grass;

	public TerrainTile(TileType t)
	{
		tileType = t;
	}
}

