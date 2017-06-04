using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Settlement
{
	int population;
	string name;
	public Settlement(string n, int p)
	{
		population = p;
		name = n;
	}
}

class Region
{
	public Settlement settlement;
	public Color color;
	public float value;

	public Region(string name, float v)
	{
		settlement = new Settlement(name, (int)v);
		value = v;
		color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}
}

