using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

class StoredTerrainMap
{
	List<List<TerrainTile>> mapTiles = new List<List<TerrainTile>>();

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

	public float TileAreaValue(int i, int j)
	{
		float value = 0f;

		foreach(TerrainTile t in GetTwoLayersAdjacentTiles(i, j))
		{
			value += t.GetValue();
		}

		return value;
	}

	private List<TerrainTile> GetTwoLayersAdjacentTiles(int i, int j)
	{
		List<TerrainTile> tiles = new List<TerrainTile>();
		for(int x = i-2; x <= i+2; x++)
		{
			for(int y = j-2; y <= j+2; y++)
			{
				TerrainTile tile = GetTile(x, y);
				if (tile != null)
					tiles.Add(tile);
			}
		}
		return tiles;
	}

	private TerrainTile GetTile(int i, int j)
	{
		if(i > 0 && i < mapTiles.Count)
		{
			if(j > 0 && j < mapTiles[i].Count)
			{
				return mapTiles[i][j];
			}
		}
		return null;
	}
}