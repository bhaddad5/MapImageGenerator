using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayDisplayHandler : MonoBehaviour
{
	public class OverlayTextures
	{
		public Map2D<Color> Overlays;
		public Map2D<Color> Water;

		public OverlayTextures(Map2D<Color> over, Map2D<Color> water)
		{
			Overlays = over;
			Water = water;
		}
	}

	public class ColorMap
	{
		public Map2D<Color> Colors;
		private int textureSize;
		private int width;
		private int height;
		private Map2D<int> stackedColorsPerTile;

		public ColorMap(int w, int h, int ts)
		{
			textureSize = ts;
			width = w;
			height = h;
			Colors = new Map2D<Color>(width * textureSize, height * textureSize);
			stackedColorsPerTile = new Map2D<int>(width * textureSize, height * textureSize);
		}

		public void SetPixels(int x, int y, Color[] colorsToSet)
		{
			for (int i = 0; i < textureSize; i++)
			{
				for (int j = 0; j < textureSize; j++)
				{
					Color c = Colors.Get(new Int2(x + j, y + i));
					Color n = colorsToSet[(i * textureSize) + j];
					if (c.a > n.a)
						n = c;
					Colors.Set(new Int2(x + j, y + i), n);
				}
			}
		}
	}

	public OverlayTextures GetOverlayMats(Map2D<MapTileModel> Map, int tileSize)
	{
		ColorMap WaterMask = new ColorMap(Map.Width, Map.Height, tileSize);
		ColorMap OverlaysTexture = new ColorMap(Map.Width, Map.Height, tileSize);

		foreach (Int2 point in Map.GetMapPoints())
		{
			foreach (string over in Map.Get(point).GetOverlays())
			{
				OverlayPlacementModel overlay = OverlayParser.OverlayData[over];
				var pixels = GetTilePixels(Map, point, overlay.GroundTrait, tileSize,
					overlay.NoSides.GetTexture(), overlay.OneSide.GetTexture(), overlay.TwoAdjSides.GetTexture(),
					overlay.TwoOppSides.GetTexture(), overlay.ThreeSides.GetTexture(), overlay.FourSides.GetTexture());

				if(overlay.OverlayLayer == "Overlay")
					OverlaysTexture.SetPixels(point.X * tileSize, point.Y * tileSize, pixels);
				if (overlay.OverlayLayer == "Water")
					WaterMask.SetPixels(point.X * tileSize, point.Y * tileSize, pixels);

			}
		}
		return new OverlayTextures(OverlaysTexture.Colors, WaterMask.Colors);
	}

	public Color[] GetTilePixels(Map2D<MapTileModel> Map, Int2 tile, string adjacentTrait, int textureSize,
		Texture2D noSides, Texture2D oneSide, Texture2D twoAdjSides, Texture2D twoOppSides, Texture2D threeSides, Texture2D fourSides)
	{
		bool topBorders = true;
		bool leftBorders = true;
		bool rightBorders = true;
		bool bottomBorders = true;
		foreach (Int2 adjacent in Map.GetAdjacentPoints(tile))
		{
			if (adjacent.X == tile.X && adjacent.Y == tile.Y + 1 &&
			    !Map.Get(adjacent).HasTrait(adjacentTrait))
				topBorders = false;
			if (adjacent.X == tile.X - 1 && adjacent.Y == tile.Y &&
			    !Map.Get(adjacent).HasTrait(adjacentTrait))
				leftBorders = false;
			if (adjacent.X == tile.X + 1 && adjacent.Y == tile.Y &&
			    !Map.Get(adjacent).HasTrait(adjacentTrait))
				rightBorders = false;
			if (adjacent.X == tile.X && adjacent.Y == tile.Y - 1 &&
			    !Map.Get(adjacent).HasTrait(adjacentTrait))
				bottomBorders = false;
		}

		if (topBorders && bottomBorders && !leftBorders && !rightBorders)
			return RotateMatrix(twoOppSides.GetPixels(), textureSize, 0);
		if (!topBorders && !bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(twoOppSides.GetPixels(), textureSize, 1);

		if (!topBorders && bottomBorders && !leftBorders && !rightBorders)
			return RotateMatrix(oneSide.GetPixels(), textureSize, 0);
		if (!topBorders && !bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(oneSide.GetPixels(), textureSize, 1);
		if (topBorders && !bottomBorders && !leftBorders && !rightBorders)
			return RotateMatrix(oneSide.GetPixels(), textureSize, 2);
		if (!topBorders && !bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(oneSide.GetPixels(), textureSize, 3);

		if (!topBorders && bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(twoAdjSides.GetPixels(), textureSize, 0);
		if (topBorders && !bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(twoAdjSides.GetPixels(), textureSize, 1);
		if (topBorders && !bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(twoAdjSides.GetPixels(), textureSize, 2);
		if (!topBorders && bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(twoAdjSides.GetPixels(), textureSize, 3);

		if (topBorders && bottomBorders && !leftBorders && rightBorders)
			return RotateMatrix(threeSides.GetPixels(), textureSize, 0);
		if (topBorders && !bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(threeSides.GetPixels(), textureSize, 1);
		if (topBorders && bottomBorders && leftBorders && !rightBorders)
			return RotateMatrix(threeSides.GetPixels(), textureSize, 2);
		if (!topBorders && bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(threeSides.GetPixels(), textureSize, 3);

		if (topBorders && bottomBorders && leftBorders && rightBorders)
			return RotateMatrix(fourSides.GetPixels(), textureSize, 0);

		return RotateMatrix(noSides.GetPixels(), textureSize, 0);
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
