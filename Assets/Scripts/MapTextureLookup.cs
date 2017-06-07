﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTextureLookup : MonoBehaviour {

	public Texture2D Mountains;
	public Texture2D Fertile;
	public Texture2D Wilderness;
	public Texture2D Forest;
	public Texture2D Swamp;
	public Texture2D Sand;

	public Texture2D GetTileTypeTexture(TerrainTile.TileType type)
	{
		if (type == TerrainTile.TileType.Mountain)
			return Mountains;
		if (type == TerrainTile.TileType.Grass)
			return Wilderness;
		if (type == TerrainTile.TileType.Forest)
			return Forest;
		if (type == TerrainTile.TileType.Fertile)
			return Fertile;
		if (type == TerrainTile.TileType.Swamp)
			return Swamp;
		if (type == TerrainTile.TileType.Ocean)
			return Sand;
		return Fertile;
	}
}