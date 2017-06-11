using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Map2D<T>
{
	public int Width { get { return map.Length; } }
	public int Height { get { return map[0].Length; } }

	T[][] map;
	public Map2D(int width, int height)
	{
		map = new T[width][];
		for(int i = 0; i < width; i++)
		{
			map[i] = new T[height];
		}
	}

	public List<Int2> GetMapPoints()
	{
		List<Int2> points = new List<Int2>();
		for (int i = 0; i < Width; i++)
		{
			for(int j = 0; j < Height; j++)
			{
				points.Add(new Int2(i, j));
			}
		}

		return points;
	}

	public List<T> GetMapValues()
	{
		List<T> points = new List<T>();
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				points.Add(map[i][j]);
			}
		}

		return points;
	}

	public T GetValueAt(Int2 point)
	{
		return map[point.X][point.Y];
	}

	public void SetPoint(Int2 point, T val)
	{
		map[point.X][point.Y] = val;
	}
}