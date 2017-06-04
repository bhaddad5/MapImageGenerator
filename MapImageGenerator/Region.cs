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
	public Settlement settlement;
	public Color color;
	public float value;

	public Region(string name, float v)
	{
		settlement = new Settlement(name, (int)v);
		value = v;
		Random r = new Random();
		color = Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
	}
}

