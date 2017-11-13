using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayDisplayHandler : MonoBehaviour
{
	public Material OverlaysMat;
	public Material WaterMat;

	public Texture2D RiverStraightOverlay;
	public Texture2D RiverEndOverlay;
	public Texture2D RiverBendOverlay;
	public Texture2D RiverForkOverlay;
	public Texture2D RiverCrossOverlay;
	public Texture2D RiverLakeOverlay;

	public Texture2D RiverStraightMask;
	public Texture2D RiverEndMask;
	public Texture2D RiverBendMask;
	public Texture2D RiverForkMask;
	public Texture2D RiverCrossMask;
	public Texture2D RiverLakeMask;

	public Texture2D CoastFlatMask;
	public Texture2D CoastBayMask;
	public Texture2D CoastBentMask;
	public Texture2D CoastLakeMask;
	public Texture2D CoastOceanMask;
	public Texture2D CoastStraightMask;

	public Texture2D CoastFlatOverlay;
	public Texture2D CoastBayOverlay;
	public Texture2D CoastBentOverlay;
	public Texture2D CoastLakeOverlay;
	public Texture2D CoastOceanOverlay;
	public Texture2D CoastStraightOverlay;

	private int textureSize = 128;

	public class OverlayTextures
	{
		public Material Overlays;
		public Material Water;

		public OverlayTextures(Material over, Material water)
		{
			Overlays = over;
			Water = water;
		}
	}

	public class ColorMap
	{
		public Color[] Colors;
		private int textureSize;
		private int width;
		private int height;

		public ColorMap(int w, int h, int ts)
		{
			textureSize = ts;
			width = w;
			height = h;
			Colors = new Color[width * textureSize * height * textureSize];
		}

		public void SetPixels(int x, int y, Color[] colorsToSet)
		{
			for (int i = 0; i < textureSize; i++)
			{
				for (int j = 0; j < textureSize; j++)
				{
					Color c = Colors[(y + i) * (width * textureSize) + (x + j)];
					Color n = colorsToSet[(i * textureSize) + j];
					if (c.a > n.a)
						n = c;
					Colors[(y + i) * (width * textureSize) + (x + j)] = n;
				}
			}
		}
	}

	public OverlayTextures GetOverlayMats(MapModel Map)
	{
		ColorMap WaterMask = new ColorMap(Map.Map.Width, Map.Map.Height, 128);
		ColorMap OverlaysTexture = new ColorMap(Map.Map.Width, Map.Map.Height, 128);

		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
			{
				OverlaysTexture.SetPixels(point.X * textureSize, point.Y * textureSize, 
					GetTilePixels(Map, point, TerrainModel.GroundTraits.Water, RiverLakeOverlay, RiverEndOverlay, RiverBendOverlay, RiverStraightOverlay, RiverForkOverlay, RiverCrossOverlay));
				WaterMask.SetPixels(point.X * textureSize, point.Y * textureSize,
					GetTilePixels(Map, point, TerrainModel.GroundTraits.Water, RiverLakeOverlay, RiverEndMask, RiverBendMask, RiverStraightMask, RiverForkMask, RiverCrossMask));
			}
			if (Map.Map.Get(point).Terrain().HasTrait(TerrainModel.GroundTraits.Ocean))
			{
				OverlaysTexture.SetPixels(point.X * textureSize, point.Y * textureSize,
					GetTilePixels(Map, point, TerrainModel.GroundTraits.Ocean, CoastLakeOverlay, CoastBayOverlay, CoastBentOverlay, CoastStraightOverlay, CoastFlatOverlay, CoastOceanOverlay));
				WaterMask.SetPixels(point.X * textureSize, point.Y * textureSize,
					GetTilePixels(Map, point, TerrainModel.GroundTraits.Ocean, CoastLakeMask, CoastBayMask, CoastBentMask, CoastStraightMask, CoastFlatMask, CoastOceanMask));
			}
		}

		Texture2D Overlays = new Texture2D(Map.Map.Width * textureSize, Map.Map.Height * textureSize);
		Overlays.SetPixels(OverlaysTexture.Colors);
		Overlays.Apply();
		OverlaysMat.mainTexture = Overlays;

		Texture2D Water = new Texture2D(Map.Map.Width * textureSize, Map.Map.Height * textureSize);
		Water.SetPixels(WaterMask.Colors);
		Water.Apply();
		WaterMat.SetTexture("_MaskTex", Water);

		return new OverlayTextures(OverlaysMat, WaterMat);
	}

	public Color[] GetTilePixels(MapModel Map, Int2 tile, TerrainModel.GroundTraits adjacentTrait,
		Texture2D noSides, Texture2D oneSide, Texture2D twoAdjSides, Texture2D twoOppSides, Texture2D threeSides, Texture2D fourSides)
	{
		bool topBorders = true;
		bool leftBorders = true;
		bool rightBorders = true;
		bool bottomBorders = true;
		foreach (Int2 adjacent in Map.Map.GetAdjacentPoints(tile))
		{
			if (adjacent.X == tile.X && adjacent.Y == tile.Y + 1 &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(adjacentTrait))
				topBorders = false;
			if (adjacent.X == tile.X - 1 && adjacent.Y == tile.Y &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(adjacentTrait))
				leftBorders = false;
			if (adjacent.X == tile.X + 1 && adjacent.Y == tile.Y &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(adjacentTrait))
				rightBorders = false;
			if (adjacent.X == tile.X && adjacent.Y == tile.Y - 1 &&
			    !Map.Map.Get(adjacent).Terrain().HasTrait(adjacentTrait))
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
