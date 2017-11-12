using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RiverDisplayHandler : MonoBehaviour
{
	public Material RiversMat;

	public Texture2D RiverStraight;
	public Texture2D RiverEnd;
	public Texture2D RiverBend;
	public Texture2D RiverFork;
	public Texture2D RiverCross;
	public Texture2D RiverLake;

	public Material GetRiversMat(MapModel Map)
	{
		Texture2D RiversTexture = new Texture2D(Map.Map.Width * 128, Map.Map.Height * 128);
		List<Color> colors = new List<Color>();
		for (int i = 0; i < Map.Map.Width * 128 * Map.Map.Height * 128; i++)
		{
			colors.Add(new Color(0, 0, 0, 0));
		}
		RiversTexture.SetPixels(colors.ToArray());
		RiversTexture.Apply();

		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
			{
				RiversTexture.SetPixels(point.X * 128, point.Y * 128, 128, 128, GetRiverTilePixels(Map,point));
			}
		}
		RiversTexture.Apply();
		RiversMat.mainTexture = RiversTexture;
		return RiversMat;
	}

	public Color[] GetRiverTilePixels(MapModel Map, Int2 tile)
	{
		bool topBorders = true;
		bool leftBorders = true;
		bool rightBorders = true;
		bool bottomBorders = true;
		foreach (Int2 adjacent in Map.Map.GetAdjacentPoints(tile))
		{
			if (adjacent.X == tile.X && adjacent.Y == tile.Y + 1 &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
				topBorders = false;
			if (adjacent.X == tile.X - 1 && adjacent.Y == tile.Y &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
				leftBorders = false;
			if (adjacent.X == tile.X + 1 && adjacent.Y == tile.Y &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
				rightBorders = false;
			if (adjacent.X == tile.X && adjacent.Y == tile.Y - 1 &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
				bottomBorders = false;
		}

		int riverWidth = RiverStraight.width;

		if (topBorders && bottomBorders && !leftBorders && !rightBorders)
			return RiverStraight.GetPixels();
		if (!topBorders && !bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(RiverStraight.GetPixels(), riverWidth, 1);

		if (!topBorders && bottomBorders && !leftBorders && !rightBorders)
			return RotateMatrix(RiverEnd.GetPixels(), riverWidth, 0);
		if (!topBorders && !bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(RiverEnd.GetPixels(), riverWidth, 1);
		if (topBorders && !bottomBorders && !leftBorders && !rightBorders)
			return RotateMatrix(RiverEnd.GetPixels(), riverWidth, 2);
		if (!topBorders && !bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(RiverEnd.GetPixels(), riverWidth, 3);

		if (!topBorders && bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(RiverBend.GetPixels(), riverWidth, 0);
		if (topBorders && !bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(RiverBend.GetPixels(), riverWidth, 1);
		if (topBorders && !bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(RiverBend.GetPixels(), riverWidth, 2);
		if (!topBorders && bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(RiverBend.GetPixels(), riverWidth, 3);

		if (topBorders && bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(RiverFork.GetPixels(), riverWidth, 0);
		if (topBorders && !bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(RiverFork.GetPixels(), riverWidth, 1);
		if (topBorders && bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(RiverFork.GetPixels(), riverWidth, 2);
		if (!topBorders && bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(RiverFork.GetPixels(), riverWidth, 3);

		if (topBorders && bottomBorders && leftBorders && rightBorders)
			return RiverCross.GetPixels();

		return RiverLake.GetPixels();
	}

	static Color[] RotateMatrix(Color[] matrix, int width, int numRotations)
	{
		for (int i = 0; i < numRotations; i++)
		{
			matrix = Rotate90(matrix, width);
		}
		return matrix;
	}

	static Color[] Rotate90(Color[] matrix, int width)
	{
		Color[] ret = new Color[width * width];

		for (int i = 0; i < width; ++i)
		{
			for (int j = 0; j < width; ++j)
			{
				ret[i * width + j] = matrix[(width - j - 1) * width + i];
			}
		}

		return ret;
	}

}
