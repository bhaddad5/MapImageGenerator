﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TerrainTile
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
		Coast,
		Delta,
	}

	public static Dictionary<TileType, float> tileDifficulties = new Dictionary<TileType, float>()
	{
		{ TileType.Ocean, 1f },
		{ TileType.River, .3f },
		{ TileType.Swamp, .35f },
		{ TileType.Mountain, .35f },
		{ TileType.Forest, .25f },
		{ TileType.Grass, .2f },
		{ TileType.Fertile, .1f },
		{ TileType.Coast, .1f },
		{ TileType.Delta, .1f },
	};

	public static Dictionary<TileType, float> tileValues = new Dictionary<TileType, float>()
	{
		{ TileType.Ocean, .8f },
		{ TileType.River, .8f },
		{ TileType.Swamp, .1f },
		{ TileType.Mountain, .1f },
		{ TileType.Forest, .2f },
		{ TileType.Grass, .2f },
		{ TileType.Fertile, .4f },
		{ TileType.Coast, .7f },
		{ TileType.Delta, 1.0f },
	};

	public static Dictionary<Color, TileType> tileColors = new Dictionary<Color, TileType>()
	{
		{ new Color(0, 0, 255/255f), TileType.Ocean },
		{ new Color(0, 255/255f, 255/255f), TileType.River },
		{ new Color(0, 188/255f, 106/255f), TileType.Swamp },
		{ new Color(144/255f, 92/255f, 0), TileType.Mountain },
		{ new Color(0, 0, 0), TileType.Forest },
		{ new Color(0, 130/255f, 0), TileType.Grass},
		{ new Color(255/255f, 255/255f, 0), TileType.Fertile },
	};

	public TileType tileType = TileType.Grass;

	public TerrainTile(Color c, bool nextToOcean, bool nextToRiver)
	{
		tileType = tileColors[c];
		if (tileType != TileType.River && tileType != TileType.Ocean && nextToOcean)
		{
			if (nextToRiver)
				tileType = TileType.Delta;
			else tileType = TileType.Coast;
		}
	}

	public float GetValue()
	{
		return tileValues[tileType];
	}

	public float GetDifficulty()
	{
		return tileDifficulties[tileType];
	}

}

