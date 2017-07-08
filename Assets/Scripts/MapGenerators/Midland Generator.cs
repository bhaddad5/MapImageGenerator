using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidlandGenerator : InitialMapGenerator, IMapGenerator
{
	public Map GenerateMaps(int width, int height, MapEnvironment env)
	{
		MapGenerator.Environment = env;
		Heights = new Map2D<float>(width, height);
		MakeHeights();

		Terrain = new Map2D<GroundInfo>(width, height);
		MakeTerrain();
		
		return new Map(Heights, Terrain);
	}

	private void MakeHeights()
	{
		HeightsDefaultFill(0f);
		HeightRandomlyPlaceAlongLine(1f, 9, 4, 50, 3);
		HeightRandomlyExpandLevelFromItselfOrLevel(Globals.MinGroundHeight, 1f, 1, 10);
		HeightRandomlyPlaceNotInWater(.2f, 200);
		HeightBlendUp(2);
		HeightSetEdges(0f);
		HeightRandomizeLevelEdges(0f, 2);

		

		//GenerateMountainRanges(Random.Range(avgNumOfRanges / 2, avgNumOfRanges + avgNumOfRanges / 2));
		/*GenerateHills(Random.Range(avgNumHills / 2, avgNumHills + avgNumHills / 2));
		ZeroOutEdges();
		RandomizeCoastline(3, false);
		BlendHeightMap();
		CreateRivers();*/
	}

	private void GenerateMountainRanges(int numOfRanges)
	{
		for (int i = 0; i < numOfRanges; i++)
		{
			GenerateMountainRange();
		}
	}

	private void GenerateMountainRange()
	{
		Int2 startingPixel = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
		float startingStrength = Random.Range(.6f, .75f);
		Int2 mountainDirection = new Int2(Random.Range(-1, 1), Random.Range(-1, 1));
		int mountainsLength = Random.Range(4, 50);
		int distToCoast = Random.Range(5, 30);

		Int2 currPixel = startingPixel;
		for (int k = 0; k < mountainsLength; k++)
		{
			float strength = startingStrength + Random.Range(-.4f, .4f);
			TryMountainCenterPixel(currPixel, strength, distToCoast);
			currPixel = GetNextPixelInDirection(currPixel, mountainDirection, 3); ;
		}
	}

	private bool TryMountainCenterPixel(Int2 pixel, float height, int distanceToCoast)
	{
		if (!Heights.PosInBounds(pixel))
			return false;

		//Random mountain passes
		if (Helpers.Odds(0.2f))
			return true;

		Heights.Set(pixel, height);

		TrySpreadLandArea(pixel, distanceToCoast);
		foreach (Int2 point in Heights.GetAllNeighboringPoints(pixel))
		{
			if (Heights.PosInBounds(point) && Helpers.Odds(0.7f))
				Heights.Set(point, height * .6f);
		}

		return true;
	}

	private void TrySpreadLandArea(Int2 startPixel, int distanceToCoast)
	{
		SortedDupList<Int2> pixelsToSpreadTo = new SortedDupList<Int2>();
		pixelsToSpreadTo.Insert(startPixel, distanceToCoast);

		while (pixelsToSpreadTo.Count > 0)
		{
			if (pixelsToSpreadTo.TopKey() > 0)
			{
				foreach (Int2 neighbor in Heights.GetAdjacentPoints(pixelsToSpreadTo.TopValue()))
				{
					if (Heights.Get(neighbor) == 0)
					{
						Heights.Set(neighbor, Globals.MinGroundHeight);
						pixelsToSpreadTo.Insert(neighbor, pixelsToSpreadTo.TopKey() - 1);
					}
				}
			}
			pixelsToSpreadTo.Pop();
		}
	}

	private void ZeroOutEdges()
	{
		for (int i = 0; i < Heights.Width; i++)
		{
			Heights.Set(new Int2(i, 0), 0);
			Heights.Set(new Int2(i, 1), 0);
			Heights.Set(new Int2(i, Heights.Height - 2), 0);
			Heights.Set(new Int2(i, Heights.Height - 1), 0);
		}
		for (int i = 0; i < Heights.Height; i++)
		{
			Heights.Set(new Int2(0, i), 0);
			Heights.Set(new Int2(1, i), 0);
			Heights.Set(new Int2(Heights.Width - 2, i), 0);
			Heights.Set(new Int2(Heights.Width - 1, i), 0);
		}
	}
	
	private void GenerateHills(int numOfHills)
	{
		List<Int2> possibleHillSites = new List<Int2>();
		for (int i = 0; i < numOfHills * 100; i++)
		{
			Int2 pos = new Int2(Random.Range(0, Heights.Width - 1), Random.Range(0, Heights.Height - 1));
			if (Heights.Get(pos) == Globals.MinGroundHeight && !BordersWater(pos))
				possibleHillSites.Add(pos);
		}

		for (int i = 0; i < numOfHills && i < possibleHillSites.Count; i++)
		{
			float hillHeight = Random.Range(.25f, .3f);
			Heights.Set(possibleHillSites[i], hillHeight);

			foreach (Int2 point in Heights.GetAllNeighboringPoints(possibleHillSites[i]))
			{
				if (Heights.PosInBounds(point) && Heights.Get(point) == Globals.MinGroundHeight && Helpers.Odds(0.8f))
					Heights.Set(point, hillHeight * .85f);
			}

		}
	}

	private bool BordersWater(Int2 tile)
	{
		foreach (float neighbor in Heights.GetAllNeighboringValues(tile))
		{
			if (neighbor < Globals.MinGroundHeight)
				return true;
		}
		return false;
	}

	private void BlendHeightMap()
	{
		int passes = 1;
		for (int i = 0; i < passes; i++)
		{
			BlendHeightMapPass();
		}
	}

	private void BlendHeightMapPass()
	{
		foreach (Int2 pixle in Heights.GetMapPoints())
		{
			float avg = NeighborAverageHeight(pixle);
			if (Heights.Get(pixle) > 0 && (Heights.Get(pixle) > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				Heights.Set(pixle, (Heights.Get(pixle) + avg) / 2);
		}
	}

	private float NeighborAverageHeight(Int2 pixle)
	{
		float sum = 0f;
		var points = Heights.GetAdjacentValues(pixle);
		foreach (var pt in points)
		{
			sum += pt;
		}
		return sum / points.Count;
	}

	private void CreateRivers()
	{
		int mapPixelsPerRiver = 500;
		int numOfRivers = (Heights.Width * Heights.Height) / mapPixelsPerRiver;

		List<Int2> possibleRiverStarts = new List<Int2>();
		for (int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Heights.Width), Random.Range(0, Heights.Height));

			if (Heights.Get(randPos) < Globals.MountainHeight && Heights.Get(randPos) >= Globals.MinGroundHeight)
			{
				possibleRiverStarts.Add(randPos);
			}
		}

		int k = 0;
		while (numOfRivers > 0 && k < possibleRiverStarts.Count)
		{
			if (possibleRiverStarts.Count > k &&
			    RiverStartValue(possibleRiverStarts[k]) > 0)
			{
				var river = TryExpandRiver(possibleRiverStarts[k], new List<Int2>());

				if (river != null)
				{
					foreach (var px in river)
						Heights.Set(px, Globals.MinGroundHeight - 0.05f);
					numOfRivers--;
				}
			}
			k++;
		}
	}

	private float RiverStartValue(Int2 startPos)
	{
		int numMountains = 0;
		int numOceans = 0;
		int numOthers = 0;
		foreach (float h in Heights.GetAdjacentValues(startPos))
		{
			if (h >= Globals.MountainHeight)
				numMountains++;
			else if (h == 0)
				numOceans++;
			else numOthers++;
		}

		if (numOceans == 0 && numMountains > 0 && numMountains < 4)
			return 1f;
		else return 0f;
	}

	private List<Int2> TryExpandRiver(Int2 pos, List<Int2> currPath)
	{
		Map2D<int> checkedTiles = new Map2D<int>(Heights.Width, Heights.Height);
		SortedDupList<Int2> nextRiverTiles = new SortedDupList<Int2>();
		int maxRiverLength = int.MaxValue;

		nextRiverTiles.Insert(pos, 0);
		checkedTiles.Set(pos, maxRiverLength);
		Int2 endTile = null;

		while (nextRiverTiles.Count > 0 && endTile == null)
		{
			var shortestTile = nextRiverTiles.MinValue();
			nextRiverTiles.PopMin();
			foreach (var neighbor in GetAdjacentRiverExpansions(shortestTile, checkedTiles))
			{
				if (Heights.Get(neighbor) < Globals.MinGroundHeight)
				{
					endTile = shortestTile;
					break;
				}
				else if (Heights.Get(neighbor) <= Heights.Get(shortestTile) + 0.03f)
				{
					checkedTiles.Set(neighbor, checkedTiles.Get(shortestTile) - 1);
					nextRiverTiles.Insert(neighbor, checkedTiles.Get(shortestTile) - 1);
				}
			}

		}

		List<Int2> riverPath = new List<Int2>();
		if (endTile != null)
		{
			riverPath.Add(pos);
			riverPath.Add(endTile);
			BuildRiverBack(checkedTiles, riverPath);
		}
		return riverPath;
	}

	private List<Int2> GetAdjacentRiverExpansions(Int2 pos, Map2D<int> checkedTiles)
	{
		List<Int2> expansionTiles = new List<Int2>();
		foreach (Int2 neighbor in Heights.GetAdjacentPoints(pos))
		{
			if (checkedTiles.Get(neighbor) == 0 && Heights.Get(neighbor) < Globals.MountainHeight)
				expansionTiles.Add(neighbor);
		}
		return expansionTiles;
	}

	private void BuildRiverBack(Map2D<int> riverDistField, List<Int2> riverPath)
	{
		Int2 maxNeighbor = riverPath[riverPath.Count - 1];
		foreach (Int2 tile in riverDistField.GetAdjacentPoints(maxNeighbor).RandomEnumerate())
		{
			if (!riverPath.Contains(tile) && riverDistField.Get(tile) > riverDistField.Get(maxNeighbor))
			{
				if (maxNeighbor == riverPath[riverPath.Count - 1])
					maxNeighbor = tile;
				else if (Helpers.Odds(.5f))
					maxNeighbor = tile;
			}

		}

		if (maxNeighbor == riverPath[riverPath.Count - 1])
			return;
		else
		{
			riverPath.Add(maxNeighbor);
			BuildRiverBack(riverDistField, riverPath);
		}
	}

	private int NumLandBorders(Int2 point)
	{
		int landBorders = 0;
		foreach (var tile in Heights.GetAllNeighboringValues(point))
		{
			if (tile >= Globals.MinGroundHeight)
				landBorders++;
		}
		return landBorders;
	}








	private void MakeTerrain()
	{
		foreach (var point in Terrain.GetMapPoints())
		{
			if (Heights.Get(point) < Globals.MinGroundHeight)
			{
				int numLandBorders = NumLandBorders(point);
				if (numLandBorders >= 6)
					Terrain.Set(point, MapGenerator.Environment.groundTypes["River"]);
				else Terrain.Set(point, MapGenerator.Environment.groundTypes["Ocean"]);
			}
			else if (Heights.Get(point) >= Globals.MountainHeight)
				Terrain.Set(point, MapGenerator.Environment.groundTypes["Mountain"]);
			else Terrain.Set(point, MapGenerator.Environment.groundTypes["Wilderness"]);
		}

		FillInLandTextures();
	}

	private void FillInLandTextures()
	{
		int numPasses = 4;
		for (int i = 0; i < numPasses; i++)
		{
			FillInLandTexturesPass();
		}
	}

	private void FillInLandTexturesPass()
	{
		foreach (var tile in Terrain.GetMapPoints())
		{
			TryFillInTile(tile);
		}
	}

	private void TryFillInTile(Int2 tile)
	{
		float oddsOfForest = 0.01f +
		                     0.01f * NextToNumOfType(tile, MapGenerator.Environment.GetGround("Mountain")) +
		                     0.3f * NextToNumOfType(tile, MapGenerator.Environment.GetGround("Forest"));
		float oddsOfFertile = 0.01f +
		                      0.07f * NextToNumOfType(tile, MapGenerator.Environment.Ocean) +
		                      0.15f * NextToNumOfType(tile, MapGenerator.Environment.River) -
		                      0.01f * NextToNumOfType(tile, MapGenerator.Environment.GetGround("Mountain")) +
		                      0.3f * NextToNumOfType(tile, MapGenerator.Environment.GetGround("Fertile"));
		float oddsOfSwamp = 0.001f +
		                    0.01f * NextToNumOfType(tile, MapGenerator.Environment.Ocean) +
		                    0.01f * NextToNumOfType(tile, MapGenerator.Environment.River) -
		                    1f * NextToNumOfType(tile, MapGenerator.Environment.GetGround("Mountain")) +
		                    0.3f * NextToNumOfType(tile, MapGenerator.Environment.GetGround("Swamp"));

		if (Terrain.Get(tile) == MapGenerator.Environment.GetGround("Wilderness"))
		{
			if (Helpers.Odds(oddsOfFertile))
				Terrain.Set(tile, MapGenerator.Environment.GetGround("Fertile"));
			else if (Helpers.Odds(oddsOfForest))
				Terrain.Set(tile, MapGenerator.Environment.GetGround("Forest"));
			else if (Helpers.Odds(oddsOfSwamp))
				Terrain.Set(tile, MapGenerator.Environment.GetGround("Swamp"));
		}
	}

	private int NextToNumOfType(Int2 tile, GroundInfo type)
	{
		int numNextTo = 0;
		foreach (GroundInfo t in Terrain.GetAdjacentValues(tile))
		{
			if (t == type)
				numNextTo++;
		}
		return numNextTo;
	}
}
