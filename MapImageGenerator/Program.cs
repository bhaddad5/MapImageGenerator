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
		int averagePixelsPerRegion = 60;

		StoredTerrainMap terrainMap = new StoredTerrainMap(mapIn);

		StoredRegionsMap regionsMap = new StoredRegionsMap(terrainMap, (mapIn.Width * mapIn.Height) / averagePixelsPerRegion);

		WriteRegionsMapToPng(regionsMap);
		Console.WriteLine("Done");
		Console.ReadLine();
	}

	static void WriteRegionsMapToPng(StoredRegionsMap map)
	{
		Bitmap mapOut = new Bitmap(map.Width, map.Height);

		for(int i = 0; i < mapOut.Width; i++)
		{
			for(int j = 0; j < mapOut.Height; j++)
			{
				mapOut.SetPixel(i, j, map.GetTileColor(new Int2(i, j)));
			}
		}

		mapOut.Save("mapOut.png");
	}
}

