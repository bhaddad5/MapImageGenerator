using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

class StoredTerrainMap
{
	List<List<TerrainTile>> mapTiles = new List<List<TerrainTile>>();
	public int Width { get { return mapTiles.Count; } }
	public int Height { get { return mapTiles[0].Count; } }

	public StoredTerrainMap(Bitmap mapIn)
	{
		for (int i = 0; i < mapIn.Width; i++)
		{
			mapTiles.Insert(i, new List<TerrainTile>());
			for (int j = 0; j < mapIn.Height; j++)
			{
				mapTiles[i].Insert(j, new TerrainTile(mapIn.GetPixel(i, j)));
			}
		}
	}

	public float TileAreaValue(Int2 pos)
	{
		float value = 0f;

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
				TerrainTile tile = GetTile(new Int2(x, y));
				if (tile != null)
					tiles.Add(tile);
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
				TerrainTile tile = GetTile(new Int2(x, y));
				if (tile != null)
					tiles.Add(tile);
			}
		}
		return tiles;
	}

	private TerrainTile GetTile(Int2 pos)
	{
		if(pos.X > 0 && pos.X < mapTiles.Count)
		{
			if(pos.Y > 0 && pos.Y < mapTiles[pos.X].Count)
			{
				return mapTiles[pos.X][pos.Y];
			}
		}
		return null;
	}

	public bool TileIsOceanOrRiver(Int2 pos)
	{
		var tile = GetTile(pos);
		return tile != null && (tile.tileType == TerrainTile.TileType.Ocean || tile.tileType == TerrainTile.TileType.River);
	}
}