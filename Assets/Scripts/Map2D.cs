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

	public List<T> GetAdjacentValues(Int2 pos)
	{
		List<T> values = new List<T>();
		foreach (var point in GetAdjacentPoints(pos))
			values.Add(GetValueAt(pos));
		return values;
	}

	public List<T> GetDiagonalValues(Int2 pos)
	{
		List<T> values = new List<T>();
		foreach (var point in GetDiagonalPoints(pos))
			values.Add(GetValueAt(pos));
		return values;
	}

	public List<T> GetAllNeighboringValues(Int2 pos)
	{
		List<T> values = new List<T>();
		values = GetAdjacentValues(pos).ToArray().Concat(GetDiagonalValues(pos).ToArray()).ToList();
		return values;
	}

	private void TryAddValue(Int2 pos, List<T> values)
	{
		if (PosInBounds(pos))
			values.Add(GetValueAt(pos));
	}

	public List<Int2> GetAdjacentPoints(Int2 pos)
	{
		List<Int2> values = new List<Int2>();
		TryAddPoint(pos + new Int2(0, 1), values);
		TryAddPoint(pos + new Int2(0, -1), values);
		TryAddPoint(pos + new Int2(1, 0), values);
		TryAddPoint(pos + new Int2(-1, 0), values);
		return values;
	}

	public List<Int2> GetDiagonalPoints(Int2 pos)
	{
		List<Int2> values = new List<Int2>();
		TryAddPoint(pos + new Int2(1, 1), values);
		TryAddPoint(pos + new Int2(1, -1), values);
		TryAddPoint(pos + new Int2(-1, 1), values);
		TryAddPoint(pos + new Int2(-1, -1), values);
		return values;
	}

	public List<Int2> GetAllNeighboringPoints(Int2 pos)
	{
		List<Int2> values = new List<Int2>();
		values = GetAdjacentPoints(pos).ToArray().Concat(GetDiagonalPoints(pos).ToArray()).ToList();
		return values;
	}

	private void TryAddPoint(Int2 pos, List<Int2> values)
	{
		if (PosInBounds(pos))
			values.Add(pos);
	}

	public bool PosInBounds(Int2 pos)
	{
		return pos.X > 0 && pos.X < Width && pos.Y > 0 && pos.Y < Height;
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