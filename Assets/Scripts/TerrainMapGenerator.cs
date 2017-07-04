using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMapGenerator
{
	public static Map2D<TerrainTile> TerrainMap;
	Texture2D terrainMapImage;

	public TerrainMapGenerator()
	{
		TerrainMap = new Map2D<TerrainTile>(HeightMapGenerator.HeightMap.Width, HeightMapGenerator.HeightMap.Height);

		foreach(var point in TerrainMap.GetMapPoints())
		{
			if (HeightMapGenerator.HeightMap.GetValueAt(point) < Globals.MinGroundHeight)
			{
				int numLandBorders = NumLandBorders(point);
				if(numLandBorders >= 6)
					TerrainMap.SetPoint(point, new TerrainTile(TerrainTile.TileType.River));
				else TerrainMap.SetPoint(point, new TerrainTile(TerrainTile.TileType.Ocean));
			}
			else if (HeightMapGenerator.HeightMap.GetValueAt(point) >= Globals.MountainHeight)
				TerrainMap.SetPoint(point, new TerrainTile(TerrainTile.TileType.Mountain));
			else TerrainMap.SetPoint(point, new TerrainTile(TerrainTile.TileType.Grass));
		}

		FillInLandTextures();
	}

	private int NumLandBorders(Int2 point)
	{
		int landBorders = 0;
		foreach(var tile in HeightMapGenerator.HeightMap.GetAllNeighboringValues(point))
		{
			if (tile >= Globals.MinGroundHeight)
				landBorders++;
		}
		return landBorders;
	}

	public void RebuildTerrainTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (var tile in TerrainMap.GetMapValuesFlipped())
		{
			pixels.Add(tile.GetTileColor());
		}
		terrainMapImage = new Texture2D(TerrainMap.Width, TerrainMap.Height, TextureFormat.ARGB32, true, true);
		terrainMapImage.filterMode = FilterMode.Point;
		terrainMapImage.anisoLevel = 0;
		terrainMapImage.SetPixels(pixels.ToArray());
		terrainMapImage.Apply();
	}

	public void FillInLandTextures()
	{
		int numPasses = 4;
		for(int i = 0; i < numPasses; i++)
		{
			FillInLandTexturesPass();
		}
	}

	private void FillInLandTexturesPass()
	{
		foreach(var tile in TerrainMap.GetMapPoints())
		{
			TryFillInTile(tile);
		}
	}

	private void TryFillInTile(Int2 tile)
	{
		float oddsOfForest = 0.01f +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) + 
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Forest);
		float oddsOfFertile = 0.01f +
			0.07f * NextToNumOfType(tile, TerrainTile.TileType.Ocean)+
			0.15f * NextToNumOfType(tile, TerrainTile.TileType.River) -
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) +
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Fertile);
		float oddsOfSwamp = 0.001f +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.Ocean) +
			0.01f * NextToNumOfType(tile, TerrainTile.TileType.River) -
			1f * NextToNumOfType(tile, TerrainTile.TileType.Mountain) +
			0.3f * NextToNumOfType(tile, TerrainTile.TileType.Swamp);

		if (GetTile(tile).tileType == TerrainTile.TileType.Grass)
		{
			if (Helpers.Odds(oddsOfFertile))
				TerrainMap.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Fertile));
			else if(Helpers.Odds(oddsOfForest))
				TerrainMap.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Forest));
			else if (Helpers.Odds(oddsOfSwamp))
				TerrainMap.SetPoint(tile, new TerrainTile(TerrainTile.TileType.Swamp));
		}
	}

	private int NextToNumOfType(Int2 tile, TerrainTile.TileType type)
	{
		int numNextTo = 0;
		foreach(TerrainTile t in GetNeighborTiles(tile))
		{
			if (t.tileType == type)
				numNextTo++;
		}
		return numNextTo;
	}

	private List<TerrainTile> GetNeighborTiles(Int2 pos)
	{
		List<TerrainTile> neighbors = new List<TerrainTile>();
		TryAddTile(pos + new Int2(1, 0), neighbors);
		TryAddTile(pos + new Int2(-1, 0), neighbors);
		TryAddTile(pos + new Int2(0, 1), neighbors);
		TryAddTile(pos + new Int2(0, -1), neighbors);
		return neighbors;
	}

	public void TryAddTile(Int2 pos, List<TerrainTile> neighbors)
	{
		if (pixelInBounds(pos))
			neighbors.Add(GetTile(pos));
	}

	private bool pixelInBounds(Int2 pixel)
	{
		return !(pixel.X < 0 || pixel.X >= TerrainMap.Width || pixel.Y < 0 || pixel.Y >= TerrainMap.Height);
	}

	public TerrainTile GetTile(Int2 pos)
	{
		return TerrainMap.GetValueAt(pos);
	}

	public Map2D<TerrainTile> GetTerrainMap()
	{
		return TerrainMap;
	}

	public Texture2D GetTerrainTexture()
	{
		RebuildTerrainTexture();
		return terrainMapImage;
	}

	public static float TileAreaValue(Culture culture, Int2 pos, bool includeDiag = false)
	{
		float value = culture.GetTileValue(pos, TerrainMap) * 2;

		float oneWaterBorderValue = 3f;
		float someWaterValue = 2f;
		float allWaterValue = -1f;

		int numWaterBorders = 0;
		foreach (Int2 t in TerrainMap.GetAdjacentPoints(pos))
		{
			if (TerrainMap.GetValueAt(t).tileType == TerrainTile.TileType.Ocean || TerrainMap.GetValueAt(t).tileType == TerrainTile.TileType.River)
				numWaterBorders++;
			value += culture.GetTileValue(t, TerrainMap);
		}

		if (includeDiag)
		{
			foreach (Int2 t in TerrainMap.GetDiagonalPoints(pos))
				value += culture.GetTileValue(t, TerrainMap) * .8f;
		}

		if (numWaterBorders == 1)
			value += oneWaterBorderValue;
		else if (numWaterBorders > 1 && numWaterBorders < 4)
			value += someWaterValue;
		else if (numWaterBorders == 4)
			value += allWaterValue;

		return value;
	}

	public int LandPixelCount()
	{
		int numTiles = 0;
		foreach (var tile in TerrainMap.GetMapValues())
		{
			if (tile.tileType != TerrainTile.TileType.Ocean &&
				tile.tileType != TerrainTile.TileType.River)
				numTiles++;
		}
		return numTiles;
	}
}
