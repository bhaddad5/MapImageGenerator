using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class Map2D<T>
{
	public int Width { get { return map.Length; } }
	public int Height { get { return map[0].Length; } }

	public int Size { get { return Width * Height; } }

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

	public T GetOrDefault(Int2 point, T defaultVal)
	{
		if (PosInBounds(point))
			return Get(point);
		else return defaultVal;
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

	public Map2D<T> GetMapBlock(Int2 startingPoint, int width, int height)
	{
		Map2D<T> block = new Map2D<T>(width, height);

		for (int i = startingPoint.X; i < startingPoint.X + width; i++)
		{
			for (int j = startingPoint.Y; j < startingPoint.Y + height; j++)
			{
				block.Set(new Int2(i - startingPoint.X, j - startingPoint.Y), Get(new Int2(i, j)));
			}
		}

		return block;
	}

	[Serializable]
	public class Map2DSerializable
	{
		public int Width;
		public int Height;
		public List<T> CondensedMap = new List<T>();

		public Map2DSerializable(int w, int h, T[][] map)
		{
			Width = w;
			Height = h;
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					CondensedMap.Add(map[i][j]);
				}
			}
		}

		public Map2D<T> ToMap2D()
		{
			Map2D<T> map = new Map2D<T>(Width, Height);
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					map.map[i][j] = CondensedMap[i*Height + j];
				}
			}
			return map;
		}
	}

	public Map2DSerializable ToSerializable()
	{
		return new Map2DSerializable(Width, Height, map);
	}

	public static Map2D<T> FromSerializable(Map2DSerializable serializable)
	{
		return serializable.ToMap2D();
	}
}