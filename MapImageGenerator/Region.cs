using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
	public static Random rand = new Random();
	public Settlement settlement;
	public Color color;
	public float value;

	public Region(string name, float v)
	{
		settlement = new Settlement(name, (int)v);
		value = v;
		color = Color.FromArgb(255, rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
	}
}

