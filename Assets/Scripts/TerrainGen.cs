using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen
{
	public static Map2D<GroundTypes.Type> Map;
	Texture2D terrainMapImage;

	public TerrainGen()
	{
		Map = new Map2D<GroundTypes.Type>(HeightsGen.Map.Width, HeightsGen.Map.Height);

		foreach(var point in Map.GetMapPoints())
		{
			if (HeightsGen.Map.GetValueAt(point) < Globals.MinGroundHeight)
			{
				int numLandBorders = NumLandBorders(point);
				if(numLandBorders >= 6)
					Map.SetPoint(point, GroundTypes.Type.River);
				else Map.SetPoint(point, GroundTypes.Type.Ocean);
			}
			else if (HeightsGen.Map.GetValueAt(point) >= Globals.MountainHeight)
				Map.SetPoint(point, GroundTypes.Type.Mountain);
			else Map.SetPoint(point, GroundTypes.Type.Grass);
		}

		FillInLandTextures();
	}

	private int NumLandBorders(Int2 point)
	{
		int landBorders = 0;
		foreach(var tile in HeightsGen.Map.GetAllNeighboringValues(point))
		{
			if (tile >= Globals.MinGroundHeight)
				landBorders++;
		}
		return landBorders;
	}

	public void RebuildTerrainTexture()
	{
		List<Color> pixels = new List<Color>();
		foreach (var tile in Map.GetMapValuesFlipped())
		{
			pixels.Add(GroundTypes.tileColors[tile]);
		}
		terrainMapImage = new Texture2D(Map.Width, Map.Height, TextureFormat.ARGB32, true, true);
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
		foreach(var tile in Map.GetMapPoints())
		{
			TryFillInTile(tile);
		}
	}

	private void TryFillInTile(Int2 tile)
	{
		float oddsOfForest = 0.01f +
			0.01f * NextToNumOfType(tile, GroundTypes.Type.Mountain) + 
			0.3f * NextToNumOfType(tile, GroundTypes.Type.Forest);
		float oddsOfFertile = 0.01f +
			0.07f * NextToNumOfType(tile, GroundTypes.Type.Ocean)+
			0.15f * NextToNumOfType(tile, GroundTypes.Type.River) -
			0.01f * NextToNumOfType(tile, GroundTypes.Type.Mountain) +
			0.3f * NextToNumOfType(tile, GroundTypes.Type.Fertile);
		float oddsOfSwamp = 0.001f +
			0.01f * NextToNumOfType(tile, GroundTypes.Type.Ocean) +
			0.01f * NextToNumOfType(tile, GroundTypes.Type.River) -
			1f * NextToNumOfType(tile, GroundTypes.Type.Mountain) +
			0.3f * NextToNumOfType(tile, GroundTypes.Type.Swamp);

		if (Map.GetValueAt(tile) == GroundTypes.Type.Grass)
		{
			if (Helpers.Odds(oddsOfFertile))
				Map.SetPoint(tile,GroundTypes.Type.Fertile);
			else if(Helpers.Odds(oddsOfForest))
				Map.SetPoint(tile, GroundTypes.Type.Forest);
			else if (Helpers.Odds(oddsOfSwamp))
				Map.SetPoint(tile, GroundTypes.Type.Swamp);
		}
	}

	private int NextToNumOfType(Int2 tile, GroundTypes.Type type)
	{
		int numNextTo = 0;
		foreach(GroundTypes.Type t in GetNeighborTiles(tile))
		{
			if (t == type)
				numNextTo++;
		}
		return numNextTo;
	}

	private List<GroundTypes.Type> GetNeighborTiles(Int2 pos)
	{
		List<GroundTypes.Type> neighbors = new List<GroundTypes.Type>();
		TryAddTile(pos + new Int2(1, 0), neighbors);
		TryAddTile(pos + new Int2(-1, 0), neighbors);
		TryAddTile(pos + new Int2(0, 1), neighbors);
		TryAddTile(pos + new Int2(0, -1), neighbors);
		return neighbors;
	}

	public void TryAddTile(Int2 pos, List<GroundTypes.Type> neighbors)
	{
		if (pixelInBounds(pos))
			neighbors.Add(Map.GetValueAt(pos));
	}

	private bool pixelInBounds(Int2 pixel)
	{
		return !(pixel.X < 0 || pixel.X >= Map.Width || pixel.Y < 0 || pixel.Y >= Map.Height);
	}

	public Texture2D GetTerrainTexture()
	{
		RebuildTerrainTexture();
		return terrainMapImage;
	}

	public int LandPixelCount()
	{
		int numTiles = 0;
		foreach (var tile in Map.GetMapValues())
		{
			if (tile != GroundTypes.Type.Ocean &&
				tile != GroundTypes.Type.River)
				numTiles++;
		}
		return numTiles;
	}
}
