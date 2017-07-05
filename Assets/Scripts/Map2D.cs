﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

	public Map2D(Map2D<T> mapToCopy)
	{
		map = new T[mapToCopy.Width][];
		for (int i = 0; i < mapToCopy.Width; i++)
		{
			map[i] = new T[mapToCopy.Height];
		}
		foreach (Int2 point in mapToCopy.GetMapPoints())
		{
			map[point.X][point.Y] = mapToCopy.Get(point);
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

	public List<Int2> GetMapPointsFlipped()
	{
		List<Int2> points = new List<Int2>();
		for (int j = 0; j < Height; j++)
		{
			for (int i = 0; i < Width; i++)
			{
				points.Add(new Int2(i, j));
			}
		}

		return points;
	}

	public List<T> GetMapValues()
	{
		List<T> points = new List<T>();
		foreach (Int2 pt in GetMapPoints())
			points.Add(Get(pt));
		return points;
	}

	public List<T> GetMapValuesFlipped()
	{
		List<T> points = new List<T>();
		foreach (Int2 pt in GetMapPointsFlipped())
			points.Add(Get(pt));
		return points;
	}

	public List<T> GetAdjacentValues(Int2 pos)
	{
		List<T> values = new List<T>();
		foreach (var point in GetAdjacentPoints(pos))
			values.Add(Get(point));
		return values;
	}

	public List<T> GetDiagonalValues(Int2 pos)
	{
		List<T> values = new List<T>();
		foreach (var point in GetDiagonalPoints(pos))
			values.Add(Get(point));
		return values;
	}

	public List<T> GetAllNeighboringValues(Int2 pos)
	{
		List<T> values = new List<T>();
		values = GetAdjacentValues(pos).ToArray().Concat(GetDiagonalValues(pos).ToArray()).ToList();
		return values;
	}

	public List<Int2> GetAdjacentPoints(Int2 pos)
	{
		List<Int2> values = new List<Int2>();
		TryAdd(pos + new Int2(0, 1), values);
		TryAdd(pos + new Int2(0, -1), values);
		TryAdd(pos + new Int2(1, 0), values);
		TryAdd(pos + new Int2(-1, 0), values);
		return values;
	}

	public List<Int2> GetDiagonalPoints(Int2 pos)
	{
		List<Int2> values = new List<Int2>();
		TryAdd(pos + new Int2(1, 1), values);
		TryAdd(pos + new Int2(1, -1), values);
		TryAdd(pos + new Int2(-1, 1), values);
		TryAdd(pos + new Int2(-1, -1), values);
		return values;
	}

	public List<Int2> GetAllNeighboringPoints(Int2 pos)
	{
		List<Int2> values = new List<Int2>();
		values = GetAdjacentPoints(pos).ToArray().Concat(GetDiagonalPoints(pos).ToArray()).ToList();
		return values;
	}

	private void TryAdd(Int2 pos, List<Int2> values)
	{
		if (PosInBounds(pos))
			values.Add(pos);
	}

	public bool PosInBounds(Int2 pos)
	{
		return pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;
	}

	public T Get(Int2 point)
	{
		return map[point.X][point.Y];
	}

	public void Set(Int2 point, T val)
	{
		map[point.X][point.Y] = val;
	}

	public void FillMap(T val)
	{
		for (int i = 0; i < map.Length; i++)
		{
			for (int j = 0; j < map[0].Length; j++)
			{
				Set(new Int2(i, j), val);
			}
		}
	}
}