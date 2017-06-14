using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class StoredTerrainMap
{
	Map2D<TerrainTile> map;
	public int Width { get { return map.Width; } }
	public int Height { get { return map.Height; } }

	public StoredTerrainMap(Map2D<TerrainTile> mapIn)
	{
		map = mapIn;
	}

	public StoredTerrainMap(Texture2D mapIn)
	{
		map = new Map2D<TerrainTile>(mapIn.width, mapIn.height);
		foreach(var point in map.GetMapPoints())
		{
			map.SetPoint(point, new TerrainTile(mapIn.GetPixel(point.X, point.Y), TileNextToOceanTexture(point, mapIn)));
		}
	}

	public float TileAreaValue(Int2 pos)
	{
		float value = TileAt(pos).GetValue();

		foreach (TerrainTile t in map.GetAdjacentValues(pos))
			value += t.GetValue();

		return value;
	}

	public bool TileInBounds(Int2 pos)
	{
		return pos.X >= 0 && pos.X < map.Width && pos.Y >= 0 && pos.Y < map.Height;
	}

	public bool TileIsType(Int2 pos, TerrainTile.TileType t)
	{
		return TileInBounds(pos) && TileAt(pos).tileType == t;
	}

	private bool TileNextToOceanTexture(Int2 pos, Texture2D mapIn)
	{
		foreach(Int2 neighbor in map.GetAllNeighboringPoints(pos))
		{
			if (TerrainTile.tileColors[mapIn.GetPixel(neighbor.X, neighbor.Y)] == TerrainTile.TileType.Ocean)
				return true;
		}
		return false;
	}

	public float TileDifficulty(Int2 pos)
	{
		return TileAt(pos).GetDifficulty();
	}

	public TerrainTile TileAt(Int2 pos)
	{
		return map.GetValueAt(pos);
	}

	public int LandPixelCount()
	{
		int numTiles = 0;
		foreach(var tile in map.GetMapValues())
		{
			if (tile.tileType != TerrainTile.TileType.Ocean)
				numTiles++;
		}
		return numTiles;
	}

	public List<Int2> MapPixels()
	{
		return map.GetMapPoints();
	}
}