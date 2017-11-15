using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Helpers
{
	public static Color RandomColor()
	{
		return new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
	}

	public static bool TextureContainsTransparency(this Texture2D tex)
	{
		foreach (Color pixel in tex.GetPixels())
		{
			if (pixel.a < .9f)
				return true;
		}
		return false;
	}

	public static void DEBUGSphereAtPoint(Vector3 point, float scale)
	{
		GameObject sphere = GameObject.Instantiate(Resources.Load("DebugSphere") as GameObject);
		sphere.transform.position = point;
		sphere.transform.localScale = new Vector3(scale, scale, scale);
		sphere.GetComponent<MeshRenderer>().material.color = Color.red;
	}

	public static List<T> RandomEnumerate<T>(this List<T> list)
	{
		List<T> finalList = new List<T>();
		if(list.Count > 0)
		{
			int startEnumerate = UnityEngine.Random.Range(0, finalList.Count - 1);
			for (int i = 0; i < list.Count; i++)
			{
				int index = i + startEnumerate;
				if (index > list.Count - 1)
					index = index - list.Count;
				finalList.Add(list[index]);
			}
		}
		return finalList;
	}

	public static bool Odds(float percentChance)
	{
		return UnityEngine.Random.Range(0, 1f) < percentChance;
	}

	public static float Randomize(float average)
	{
		return UnityEngine.Random.Range(average * 0.75f, average * 1.25f);
	}

	public static Vector3 FromTo(this Vector3 from, Vector3 to)
	{
		return to - from;
	}

	public static Vector2 FromTo(this Vector2 from, Vector2 to)
	{
		return to - from;
	}
}

public class Int2
{
	public int X;
	public int Y;

	public Int2(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override bool Equals(object obj)
	{
		return obj is Int2 && (obj as Int2).X == X && (obj as Int2).Y == Y;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static Int2 operator +(Int2 a, Int2 b)
	{
		return new Int2(a.X + b.X, a.Y + b.Y);
	}

	public static Int2 operator -(Int2 a, Int2 b)
	{
		return new Int2(a.X - b.X, a.Y - b.Y);
	}

	public static Int2 operator *(Int2 a, int b)
	{
		return new Int2(a.X * b, a.Y * b);
	}

	public static Int2 operator /(Int2 a, int b)
	{
		return new Int2(a.X / b, a.Y / b);
	}

	public override string ToString()
	{
		return X + ", " + Y;
	}

	public static Int2 Random(int min, int max)
	{
		return new Int2(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
	}
}

public class SortedDupList<T>
{
	private class DupEntry
	{
		public float comparator;
		public T value;

		public DupEntry(float c, T v)
		{
			comparator = c;
			value = v;
		}
	}

	private List<DupEntry> entries = new List<DupEntry>();
	public int Count { get { return entries.Count; } }

	public void Insert(T value, float cmp)
	{
		DupEntry newEntry = new DupEntry(cmp, value);
		for(int i = 0; i <=entries.Count; i++)
		{
			if(i == entries.Count || newEntry.comparator >= entries[i].comparator)
			{
				entries.Insert(i, newEntry);
				break;
			}
		}
	}

	public T ValueAt(int index)
	{
		return entries[index].value;
	}

	public float KeyAt(int index)
	{
		return entries[index].comparator;
	}

	public List<T> FirstNumValues(int numValues)
	{
		List<T> vals = new List<T>();
		for(int i = 0; i < numValues && i < entries.Count; i++)
		{
			vals.Add(entries[i].value);
		}
		return vals;
	}

	public List<KeyValuePair<float, T>> GetList()
	{
		List<KeyValuePair<float, T>> list = new List<KeyValuePair<float, T>>();
		foreach(var entry in entries)
		{
			list.Add(new KeyValuePair<float, T>(entry.comparator, entry.value));
		}

		return list;
	}

	public bool ContainsValue(T val)
	{
		foreach(var e in entries)
		{
			if (e.value.Equals(val))
				return true;
		}
		return false;
	}

	public float TopKey()
	{
		return entries[0].comparator;
	}

	public T TopValue()
	{
		return entries[0].value;
	}

	public float MinKey()
	{
		return entries[entries.Count-1].comparator;
	}

	public T MinValue()
	{
		return entries[entries.Count - 1].value;
	}

	public T PopMin()
	{
		T val = entries[entries.Count - 1].value;
		entries.RemoveAt(entries.Count - 1);
		return val;
	}

	public T Pop()
	{
		T val = entries[0].value;
		entries.RemoveAt(0);
		return val;
	}
}
