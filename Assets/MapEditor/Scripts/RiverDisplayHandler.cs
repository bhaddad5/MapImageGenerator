using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RiverDisplayHandler : MonoBehaviour
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

	public OverlayTextures GetOverlayMats(MapModel Map)
	{
		Texture2D WaterMask = new Texture2D(Map.Map.Width * 128, Map.Map.Height * 128);
		Texture2D OverlaysTexture = new Texture2D(Map.Map.Width * 128, Map.Map.Height * 128);
		List<Color> colors = new List<Color>();
		for (int i = 0; i < Map.Map.Width * 128 * Map.Map.Height * 128; i++)
		{
			colors.Add(new Color(0, 0, 0, 0));
		}
		OverlaysTexture.SetPixels(colors.ToArray());
		OverlaysTexture.Apply();
		WaterMask.SetPixels(colors.ToArray());
		WaterMask.Apply();

		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).Terrain().HasTrait(TerrainModel.GroundTraits.Water))
			{
				OverlayAndMaskPixels px = GetRiverTilePixels(Map, point);
				OverlaysTexture.SetPixels(point.X * 128, point.Y * 128, 128, 128, px.OverlayPixels);
				WaterMask.SetPixels(point.X * 128, point.Y * 128, 128, 128, px.MaskPixels);
			}
		}
		OverlaysTexture.Apply();
		WaterMask.Apply();
		OverlaysMat.mainTexture = OverlaysTexture;
		WaterMat.SetTexture("_MaskTex", WaterMask);

		return new OverlayTextures(OverlaysMat, WaterMat);
	}

	public class OverlayAndMaskPixels
	{
		public Color[] OverlayPixels;
		public Color[] MaskPixels;

		private int textureSize = 128;

		public OverlayAndMaskPixels(Texture2D overTex, Texture2D maskTex, int numRotations)
		{
			OverlayPixels = RotateMatrix(overTex.GetPixels(), textureSize, numRotations);
			MaskPixels = RotateMatrix(maskTex.GetPixels(), textureSize, numRotations);
		}
	}

	public OverlayAndMaskPixels GetRiverTilePixels(MapModel Map, Int2 tile)
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

		int riverWidth = RiverStraightOverlay.width;

		if (topBorders && bottomBorders && !leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverStraightOverlay, RiverStraightMask, 0);
		if (!topBorders && !bottomBorders && leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverStraightOverlay, RiverStraightMask, 1);

		if (!topBorders && bottomBorders && !leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverEndOverlay, RiverEndMask, 0);
		if (!topBorders && !bottomBorders && !leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverEndOverlay, RiverEndMask, 1);
		if (topBorders && !bottomBorders && !leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverEndOverlay, RiverEndMask, 2);
		if (!topBorders && !bottomBorders && leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverEndOverlay, RiverEndMask, 3);

		if (!topBorders && bottomBorders && !leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverBendOverlay, RiverBendMask, 0);
		if (topBorders && !bottomBorders && !leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverBendOverlay, RiverBendMask, 1);
		if (topBorders && !bottomBorders && leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverBendOverlay, RiverBendMask, 2);
		if (!topBorders && bottomBorders && leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverBendOverlay, RiverBendMask, 3);

		if (topBorders && bottomBorders && !leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverForkOverlay, RiverForkMask, 0);
		if (topBorders && !bottomBorders && leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverForkOverlay, RiverForkMask, 1);
		if (topBorders && bottomBorders && leftBorders && !rightBorders)
			return new OverlayAndMaskPixels(RiverForkOverlay, RiverForkMask, 2);
		if (!topBorders && bottomBorders && leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverForkOverlay, RiverForkMask, 3);

		if (topBorders && bottomBorders && leftBorders && rightBorders)
			return new OverlayAndMaskPixels(RiverCrossOverlay, RiverCrossMask, 0);

		return new OverlayAndMaskPixels(RiverLakeOverlay, RiverLakeMask, 0);
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
