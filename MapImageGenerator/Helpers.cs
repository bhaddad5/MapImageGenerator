using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	public void Insert(float cmp, T value)
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

	public List<T> FirstNumValues(int numValues)
	{
		List<T> vals = new List<T>();
		for(int i = 0; i < numValues && i < entries.Count; i++)
		{
			vals.Add(entries[i].value);
		}
		return vals;
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
}
