using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
		Coast
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
	};

	public static Dictionary<Color, TileType> tileColors = new Dictionary<Color, TileType>()
	{
		{ Color.FromArgb(255, 0, 0, 255), TileType.Ocean },
		{ Color.FromArgb(255, 0, 255, 255), TileType.River },
		{ Color.FromArgb(255, 0, 188, 106), TileType.Swamp },
		{ Color.FromArgb(255, 144, 92, 0), TileType.Mountain },
		{ Color.FromArgb(255, 0, 0, 0), TileType.Forest },
		{ Color.FromArgb(255, 0, 130, 0), TileType.Grass},
		{ Color.FromArgb(255, 255, 255, 0), TileType.Fertile },
		{ Color.FromArgb(255, 255, 255, 255), TileType.Coast },
	};

	public TileType tileType = TileType.Grass;

	public TerrainTile(Color c)
	{
		tileType = tileColors[c];
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

