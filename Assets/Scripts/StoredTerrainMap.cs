using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class StoredTerrainMap
{
	List<List<TerrainTile>> mapTiles = new List<List<TerrainTile>>();
	public int Width { get { return mapTiles.Count; } }
	public int Height { get { return mapTiles[0].Count; } }

	public StoredTerrainMap(TerrainTile[][] map)
	{
		for(int i = 0; i < map.Length; i++)
		{
			mapTiles.Insert(i, new List<TerrainTile>());
			for(int j = 0; j < map[0].Length; j++)
			{
				mapTiles[i].Insert(j, map[i][j]);
			}
		}
	}

	public StoredTerrainMap(Texture2D mapIn)
	{
		for (int i = 0; i < mapIn.width; i++)
		{
			mapTiles.Insert(i, new List<TerrainTile>());
			for (int j = 0; j < mapIn.height; j++)
			{
				Int2 pos = new Int2(i, j);
				mapTiles[i].Insert(j, new TerrainTile(mapIn.GetPixel(i, j), TileNextToOcean(pos, mapIn)));			
			}
		}
	}

	public float TileAreaValue(Int2 pos)
	{
		float value = TileAt(pos).GetValue();

		foreach (TerrainTile t in GetAdjacentTiles(pos))
			value += t.GetValue();
		foreach (TerrainTile t in GetTwoLayersAdjacentTiles(pos))
			value += t.GetValue();

		return value;
	}

	private List<TerrainTile> GetAdjacentTiles(Int2 pos)
	{
		List<TerrainTile> tiles = new List<TerrainTile>();
		for (int x = pos.X - 1; x <= pos.X + 1; x++)
		{
			for (int y = pos.Y - 1; y <= pos.Y + 1; y++)
			{
				if (TileInBounds(new Int2(x, y)))
					tiles.Add(TileAt(new Int2(x, y)));
			}
		}
		return tiles;
	}

	private List<TerrainTile> GetTwoLayersAdjacentTiles(Int2 pos)
	{
		List<TerrainTile> tiles = new List<TerrainTile>();
		for(int x = pos.X - 2; x <= pos.X + 2; x++)
		{
			for(int y = pos.Y - 2; y <= pos.Y + 2; y++)
			{
				if (TileInBounds(new Int2(x, y)))
					tiles.Add(TileAt(new Int2(x, y)));
			}
		}
		return tiles;
	}

	public bool TileInBounds(Int2 pos)
	{
		return pos.X >= 0 && pos.X < mapTiles.Count &&
			pos.Y >= 0 && pos.Y < mapTiles[pos.X].Count;
	}

	public bool TileIsOcean(Int2 pos)
	{
		return TileInBounds(pos) && TileAt(pos).tileType == TerrainTile.TileType.Ocean;
	}

	private bool TileNextToOcean(Int2 pos, Texture2D mapIn)
	{
		foreach (Color c in GetAdjacentTiles(pos, mapIn))
		{
			if (TerrainTile.tileColors[c] == TerrainTile.TileType.Ocean)
				return true;
		}
		return false;
	}

	private List<Color> GetAdjacentTiles(Int2 pos, Texture2D mapIn)
	{
		List<Color> colors = new List<Color>();
		for (int x = pos.X - 1; x <= pos.X + 1; x++)
		{
			for (int y = pos.Y - 1; y <= pos.Y + 1; y++)
			{
				if (TileInBounds(new Int2(x, y)))
					colors.Add(mapIn.GetPixel(x, y));
			}
		}
		return colors;
	}

	public float TileDifficulty(Int2 pos)
	{
		return TileAt(pos).GetDifficulty();
	}

	public TerrainTile TileAt(Int2 pos)
	{
		return mapTiles[pos.X][pos.Y];
	}

	public int LandPixelCount()
	{
		int numTiles = 0;
		foreach(var column in mapTiles)
		{
			foreach(var tile in column)
			{
				if (tile.tileType != TerrainTile.TileType.Ocean)
					numTiles++;
			}
		}
		return numTiles;
	}
}