using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

class Program
{
	static void Main(string[] args)
	{
		Bitmap mapIn = new Bitmap("mapIn.png");

		StoredTerrainMap tileMap = new StoredTerrainMap(mapIn);

		Console.WriteLine(tileMap.TileAreaValue(15, 15));

		Console.ReadLine();
	}
}

