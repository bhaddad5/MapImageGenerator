﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorApi
{
	protected MapModel Map;

	public MapModel GenerateMap(MapModel map, WorldModel world)
	{
		Map = map;
		TerrainDefaultFill("Ocean");
		foreach (RealmPlacementModel realm in world.Realms)
			ExecuteApiCommands(realm.Realm.PreRiverCommands, realm.MinLatitude, realm.MaxLatitude);

		SetEdgesToOcean();
		TerrainRandomizeEdges("Ocean", 2);

		foreach (RealmPlacementModel realm in world.Realms)
			ExecuteApiCommands(realm.Realm.RiverCommands, realm.MinLatitude, realm.MaxLatitude);
		foreach (RealmPlacementModel realm in world.Realms)
			ExecuteApiCommands(realm.Realm.PostRiverCommands, realm.MinLatitude, realm.MaxLatitude);
		ReplaceOceanCoastTiles();
		return Map;
	}

	private void ExecuteApiCommands(List<string> commands, float minLatitude, float maxLatitude)
	{
		foreach (string command in commands)
		{
			ExecuteApiCommand(command, (int)(Map.Map.Height * minLatitude), (int)(Map.Map.Height * maxLatitude));
		}
	}

	private void ExecuteApiCommand(string cmd, int minH, int maxH)
	{
		string[] split = cmd.Split(' ');
		
		if (split[0] == "TerrainRandomizeEdges")
			TerrainRandomizeEdges(split[1], Int32.Parse(split[2]));
		if (split[0] == "TerrainRandomlyPlaceAlongLine")
			TerrainRandomlyPlaceAlongLine(split[1], Single.Parse(split[2]), Single.Parse(split[3]), Single.Parse(split[4]), Single.Parse(split[5]), minH, maxH);
		if (split[0] == "TerrainRandomlyExpandFromTypes")
		{
			List<string> splits = new List<string>();
			for (int j = 4; j < split.Length; j++)
				splits.Add(split[j]);
			TerrainRandomlyExpandFromTypes(split[1], Single.Parse(split[2]), Single.Parse(split[3]), minH, maxH, splits.ToArray());
		}

		if (split[0] == "CreateRivers")
			CreateRivers(Int32.Parse(split[1]), Int32.Parse(split[2]), minH, maxH);

		if (split[0] == "TerrainEncourageStartAlongMountains")
			TerrainEncourageStartAlongMountains(split[1], Single.Parse(split[2]), minH, maxH);
		if (split[0] == "TerrainEncourageStartAlongWater")
			TerrainEncourageStartAlongWater(split[1], Single.Parse(split[2]), minH, maxH);
		if (split[0] == "TerrainEncourageStartAlongOcean")
			TerrainEncourageStartAlongOcean(split[1], Single.Parse(split[2]), minH, maxH);
		if (split[0] == "TerrainEncourageStartAlongRiver")
			TerrainEncourageStartAlongRiver(split[1], Single.Parse(split[2]), minH, maxH);
		if (split[0] == "TerrainForceSetRivers")
			TerrainForceSetRivers(split[1], minH, maxH);
		if (split[0] == "TerrainRandomlyStart")
			TerrainRandomlyStart(split[1], Single.Parse(split[2]), minH, maxH);
		if (split[0] == "TerrainExpandSimmilarTypes")
			TerrainExpandSimmilarTypes(Int32.Parse(split[1]), split[2], minH, maxH);
	}

	private void TerrainRandomlyPlaceAlongLine(string terrain, float occurancesPer20Square, float minLength, float maxLength, float spacing, int minH, int maxH)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer20Square * Map.OccurancesPer20Scaler(minH, maxH));
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width - 1), Random.Range(minH, maxH - 1));
			PlaceTerrainAlongVector(randPos, terrain, new Int2(Random.Range(-1, 1), Random.Range(-1, 1)), Random.Range(minLength, maxLength), spacing, minH, maxH);
		}
	}

	private void PlaceTerrainAlongVector(Int2 startPos, string terrain, Int2 direction, float length, float spacing, int minH, int maxH)
	{
		Int2 currPixel = startPos;
		for (int k = 0; k < (int)length; k++)
		{
			Map.Map.Get(currPixel).TerrainId = terrain;
			currPixel = GetNextPixelInDirection(currPixel, direction, (int)(spacing + Random.Range(-2, 2)));
			if (!Map.Map.PosInBounds(currPixel) || currPixel.Y >= maxH || currPixel.Y < minH)
				return;
		}
	}

	private Int2 GetNextPixelInDirection(Int2 currPoint, Int2 direction, int distBetweenPixles)
	{
		Int2[] potentials = new Int2[10];
		potentials[0] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[1] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[2] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[3] = currPoint + nextDirection(direction, 0) * distBetweenPixles;
		potentials[4] = currPoint + nextDirection(direction, 1) * distBetweenPixles;
		potentials[5] = currPoint + nextDirection(direction, 1) * distBetweenPixles;
		potentials[6] = currPoint + nextDirection(direction, 2) * distBetweenPixles;
		potentials[7] = currPoint + nextDirection(direction, -1) * distBetweenPixles;
		potentials[8] = currPoint + nextDirection(direction, -1) * distBetweenPixles;
		potentials[9] = currPoint + nextDirection(direction, -2) * distBetweenPixles;

		return potentials[Random.Range(0, 9)];
	}

	private Int2 nextDirection(Int2 dir, int forwardOrBack)
	{
		List<Int2> dirs = new List<Int2> { new Int2(-1, -1), new Int2(-1, 0), new Int2(-1, 1), new Int2(0, 1), new Int2(1, 1), new Int2(1, 0), new Int2(1, -1), new Int2(0, -1), };
		int index = dirs.IndexOf(dir) + forwardOrBack;
		if (index >= dirs.Count)
			index = index - dirs.Count;
		if (index < 0)
			index = dirs.Count + index;
		return dirs[index];
	}

	private void TerrainRandomlyExpandFromTypes(string terrain, float minExpansion, float maxExpansion, int minH, int maxH, params string[] terrainTypes)
	{
		Map2D<bool> ModifiedTiles = new Map2D<bool>(Map.Map.Width, Map.Map.Height);
		List<string> ignoreTypes = terrainTypes.ToList();
		ignoreTypes.Add(terrain);
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if (ignoreTypes.Contains(Map.Map.Get(point).TerrainId))
				continue;

			foreach (string terrainType in terrainTypes)
			{
				if (BordersTerrainType(point, terrainType))
				{
					TerrainExpandFromPoint(point, terrain, Random.Range(minExpansion, maxExpansion), ModifiedTiles, ignoreTypes.ToArray());
					break;
				}
			}
		}
	}

	private void TerrainExpandFromPoint(Int2 point, string terrain, float numExpansionsLevels, Map2D<bool> modifiedTiles, params string[] ignoredTypes)
	{
		SortedDupList<Int2> TerrainFrontier = new SortedDupList<Int2>();
		TerrainFrontier.Insert(point, numExpansionsLevels);
		while (TerrainFrontier.Count > 0)
		{
			Int2 currPos = TerrainFrontier.TopValue();
			float currStrength = TerrainFrontier.TopKey();
			TerrainFrontier.Pop();
			Map.Map.Get(currPos).TerrainId = terrain;
			modifiedTiles.Set(currPos, true);
			foreach (Int2 pos in Map.Map.GetAdjacentPoints(currPos))
			{
				if (!modifiedTiles.Get(pos) && !TerrainFrontier.ContainsValue(pos) && !ignoredTypes.Contains(Map.Map.Get(pos).TerrainId) && currStrength > 0)
				{
					TerrainFrontier.Insert(pos, currStrength - 1);
				}
			}
		}
	}

	private void TerrainRandomizeEdges(string terrain, int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Map.Map.GetMapPoints())
			{
				if (BordersTerrainType(point, terrain))
				{
					if (Helpers.Odds(0.4f))
						Map.Map.Get(point).TerrainId = terrain;
				}
			}
		}
	}

	private bool BordersTerrainTrait(Int2 tile, MapTileModel.TileTraits trait)
	{
		foreach (Int2 neighbor in Map.Map.GetAdjacentPoints(tile))
		{
			if (Map.Map.Get(neighbor).HasTrait(trait))
				return true;
		}
		return false;
	}

	private bool BordersTerrainType(Int2 tile, string terrain)
	{
		foreach (Int2 neighbor in Map.Map.GetAdjacentPoints(tile))
		{
			if (Map.Map.Get(neighbor).TerrainId == terrain)
				return true;
		}
		return false;
	}

	private void CreateRivers(int minNumRivers, int maxNumRivers, int minH, int maxH)
	{
		int numOfRivers = (int)(Random.Range(minNumRivers, maxNumRivers) * Map.OccurancesPer20Scaler(minH, maxH));

		List<Int2> possibleRiverStarts = new List<Int2>();
		for (int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width), Random.Range(minH, maxH));
			if (!OceanOrMountain(randPos))
				possibleRiverStarts.Add(randPos);
		}

		int k = 0;
		while (numOfRivers > 0 && k < possibleRiverStarts.Count)
		{
			if (possibleRiverStarts.Count > k &&
			    RiverStartValue(possibleRiverStarts[k]) > 0)
			{
				var river = TryExpandRiver(possibleRiverStarts[k]);

				if (river != null)
				{
					foreach (var px in river)
					{
						Map.Map.Get(px).Traits.Add(MapTileModel.TileTraits.River.ToString());
						Map.Map.Get(px).Traits.Add(MapTileModel.TileTraits.Water.ToString());
						Map.Map.Get(px).Overlays.Add("RiverWater");
						Map.Map.Get(px).Overlays.Add("RiverBanks");
					}
					numOfRivers--;
				}
			}
			k++;
		}
	}

	private float RiverStartValue(Int2 startPos)
	{
		int numMountains = GetAdjacentNumWithTrait(startPos, MapTileModel.TileTraits.Mountain);
		int numOceans = GetAdjacentNumWithTrait(startPos, MapTileModel.TileTraits.Ocean);

		if (numOceans == 0 && numMountains > 0 && numMountains < 4)
			return 1f;
		else return 0f;
	}

	private List<Int2> TryExpandRiver(Int2 pos)
	{
		Map2D<int> checkedTiles = new Map2D<int>(Map.Map.Width, Map.Map.Height);
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
				if (Map.Map.Get(neighbor).HasTrait(MapTileModel.TileTraits.Water))
				{
					endTile = shortestTile;
					break;
				}
				else if (Map.Map.Get(neighbor).Height <= Map.Map.Get(shortestTile).Height)
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
		foreach (Int2 neighbor in Map.Map.GetAdjacentPoints(pos))
		{
			if (checkedTiles.Get(neighbor) == 0 && Map.Map.Get(neighbor).Height <= Map.Map.Get(pos).Height)
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

	private void TerrainDefaultFill(string defaultTerrain)
	{
		Map.FillMapWithTerrain(defaultTerrain);
	}

	private void TerrainEncourageStartAlongMountains(string terrain, float odds, int minH, int maxH)
	{
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if(!OceanOrMountain(point) && Helpers.Odds(odds) && BordersTerrainTrait(point, MapTileModel.TileTraits.Mountain))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	private void TerrainEncourageStartAlongWater(string terrain, float odds, int minH, int maxH)
	{
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if (!OceanOrMountain(point) && Helpers.Odds(odds) && BordersTerrainTrait(point, MapTileModel.TileTraits.Water))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	private void TerrainEncourageStartAlongOcean(string terrain, float odds, int minH, int maxH)
	{
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if (!OceanOrMountain(point) && Helpers.Odds(odds) && BordersTerrainTrait(point, MapTileModel.TileTraits.Ocean))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	private void TerrainForceSetRivers(string terrain, int minH, int maxH)
	{
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if (Map.Map.Get(point).HasTrait(MapTileModel.TileTraits.River))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	private void TerrainEncourageStartAlongRiver(string terrain, float odds, int minH, int maxH)
	{
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if (!OceanOrMountain(point) && Helpers.Odds(odds) && BordersTerrainTrait(point, MapTileModel.TileTraits.River))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	private void TerrainRandomlyStart(string terrain, float odds, int minH, int maxH)
	{
		foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
		{
			if (!OceanOrMountain(point) && Helpers.Odds(odds))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	private void TerrainExpandSimmilarTypes(int numPasses, string typeToExpand, int minH, int maxH)
	{
		for (int i = 0; i < numPasses; i++)
		{
			Map2D<bool> ChangedTiles = new Map2D<bool>(Map.Map.Width, Map.Map.Height);
			foreach (Int2 point in Map.Map.GetMapPoints(minH, maxH))
			{
				if (!OceanOrMountain(point) && Map.Map.Get(point).TerrainId != typeToExpand)
				{
					int numAdjacent = GetAdjacentNumOfType(point, typeToExpand, ChangedTiles);
					if (Helpers.Odds(0.25f * numAdjacent))
					{
						ChangedTiles.Set(point, true);
						Map.Map.Get(point).TerrainId = typeToExpand;
					}
				}
				
			}
		}
	}

	private int GetAdjacentNumWithTrait(Int2 point, MapTileModel.TileTraits trait)
	{
		int num = 0;
		foreach (Int2 adjacent in Map.Map.GetAdjacentPoints(point))
		{
			if (Map.Map.Get(adjacent).HasTrait(trait))
				num++;
		}
		return num;
	}

	private int GetAdjacentNumOfType(Int2 point, string type, Map2D<bool> invalidTiles)
	{
		int num = 0;
		foreach (Int2 adjacent in Map.Map.GetAdjacentPoints(point))
		{
			if (!invalidTiles.Get(adjacent) && !OceanOrMountain(adjacent) && Map.Map.Get(adjacent).TerrainId == type)
				num++;
		}
		return num;
	}

	private bool OceanOrMountain(Int2 point)
	{
		return Map.Map.Get(point).HasTrait(MapTileModel.TileTraits.Ocean) || Map.Map.Get(point).HasTrait(MapTileModel.TileTraits.Mountain);
	}


	private void SetEdgesToOcean()
	{
		for(int i = 0; i < Map.Map.Width; i++)
		{
			for (int tilesIn = 0; tilesIn < 3; tilesIn++)
			{
				if(Helpers.Odds((4f - tilesIn)/3f))
					Map.Map.Get(new Int2(i, tilesIn)).TerrainId = "Ocean";
				if (Helpers.Odds((4f - tilesIn) / 3f))
					Map.Map.Get(new Int2(i, Map.Map.Height - tilesIn - 1)).TerrainId = "Ocean";
			}
		}
		for (int i = 0; i < Map.Map.Height; i++)
		{
			for (int tilesIn = 0; tilesIn < 3; tilesIn++)
			{
				if (Helpers.Odds((4f - tilesIn) / 3f))
					Map.Map.Get(new Int2(tilesIn, i)).TerrainId = "Ocean";
				if (Helpers.Odds((4f - tilesIn) / 3f))
					Map.Map.Get(new Int2(Map.Map.Width - tilesIn - 1, i)).TerrainId = "Ocean";
			}
		}
	}


	private void ReplaceOceanCoastTiles()
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).TerrainId == "Ocean")
			{
				List<string> neighborTerrains = new List<string>();
				foreach (MapTileModel neighbor in Map.Map.GetAllNeighboringValues(point))
				{
					if(!neighbor.HasTrait(MapTileModel.TileTraits.Ocean))
						neighborTerrains.Add(neighbor.TerrainId);
				}
				if (neighborTerrains.Count == 0)
					continue;

				string newTerrain = neighborTerrains.GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
				Map.Map.Get(point).TerrainId = newTerrain;
				Map.Map.Get(point).Traits.Add(MapTileModel.TileTraits.Ocean.ToString());
				Map.Map.Get(point).Traits.Add(MapTileModel.TileTraits.Water.ToString());
				Map.Map.Get(point).Traits.Add(MapTileModel.TileTraits.Impassable.ToString());

				Map.Map.Get(point).Overlays.Add("RiverWater");
				Map.Map.Get(point).Overlays.Add("RiverBanks");
				Map.Map.Get(point).Overlays.Add("OceanWater");
				Map.Map.Get(point).Overlays.Add("OceanShore");

				Map.Map.Get(point).SetMaxHeight(TerrainParser.TerrainData["Ocean"].Height);
			}
			
		}
	}
}
