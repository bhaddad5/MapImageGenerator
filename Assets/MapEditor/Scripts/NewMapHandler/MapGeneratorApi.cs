using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorApi
{
	protected MapModel Map;

	public MapModel GenerateMap(int width, int height, RealmModel realm)
	{
		Map = new MapModel(width, height);
		ExecuteApiCommands(realm.MapBuildingCommands);
		return Map;
	}

	public void ExecuteApiCommands(List<string> commands)
	{
		foreach (string command in commands)
		{
			ExecuteApiCommand(command);
		}
	}

	public class HeightLevel
	{
		public enum Comparison
		{
			Exact,
			Greater,
			Less
		};

		public float Height;
		private Comparison comp = Comparison.Exact;

		public HeightLevel(string height)
		{
			if (height.StartsWith(">"))
			{
				comp = Comparison.Greater;
				height = height.Substring(1);
			}
			else if (height.StartsWith("<"))
			{
				comp = Comparison.Less;
				height = height.Substring(1);
			}
			Height = Single.Parse(height);
		}

		public bool Compare(float level)
		{
			if (comp == Comparison.Exact)
				return level.Equals(Height);
			if (comp == Comparison.Greater)
				return Height < level;
			if (comp == Comparison.Less)
				return Height > level;
			return false;
		}
	}

	private void ExecuteApiCommand(string cmd)
	{
		string[] split = cmd.Split(' ');
		ReplaceWellKnownValues(split);
		
		if (split[0] == "HeightsDefaultFill")
			HeightsDefaultFill(new HeightLevel(split[1]));
		if (split[0] == "HeightRandomlyPlace")
			HeightRandomlyPlace(new HeightLevel(split[1]), Single.Parse(split[2]));
		if (split[0] == "HeightRandomlyPlaceNotInWater")
			HeightRandomlyPlaceNotInWater(new HeightLevel(split[1]), Single.Parse(split[2]));
		if (split[0] == "HeightRandomlyExpandLevel")
			HeightRandomlyExpandLevel(new HeightLevel(split[1]), Single.Parse(split[2]));
		if (split[0] == "HeightRandomizeLevelEdges")
			HeightRandomizeLevelEdges(new HeightLevel(split[1]), Int32.Parse(split[2]));
		if (split[0] == "HeightRandomlyPlaceAlongLine")
			HeightRandomlyPlaceAlongLine(new HeightLevel(split[1]), Single.Parse(split[2]), Single.Parse(split[3]), Single.Parse(split[4]), Single.Parse(split[5]));
		if (split[0] == "HeightRandomlyPlaceAlongEdges")
			HeightRandomlyPlaceAlongEdges(new HeightLevel(split[1]), Single.Parse(split[2]));
		if (split[0] == "HeightRandomlyExpandLevelFromItselfOrLevel")
			HeightRandomlyExpandLevelFromItselfOrLevel(new HeightLevel(split[1]), new HeightLevel(split[2]), Single.Parse(split[3]), Single.Parse(split[4]));
		if (split[0] == "HeightSetEdges")
			HeightSetEdges(Single.Parse(split[1]));
		if (split[0] == "HeightBlendUp")
			HeightBlendUp(Int32.Parse(split[1]));
		if (split[0] == "CreateRivers")
			CreateRivers(Int32.Parse(split[1]), Int32.Parse(split[2]));


		if (split[0] == "TerrainDefaultFill")
			TerrainDefaultFill(split[1]);
		if (split[0] == "TerrainFillInOceans")
			TerrainFillInOceans(split[1]);
		if (split[0] == "TerrainFillInRivers")
			TerrainFillInRivers(split[1]);
		if (split[0] == "TerrainFillInSeaLevel")
			TerrainFillInSeaLevel(split[1]);
		if (split[0] == "TerrainFillInMountains")
			TerrainFillInMountains(split[1]);
		if (split[0] == "TerrainEncourageStartAlongMountains")
			TerrainEncourageStartAlongMountains(split[1], Single.Parse(split[2]));
		if (split[0] == "TerrainEncourageStartAlongOcean")
			TerrainEncourageStartAlongOcean(split[1], Single.Parse(split[2]));
		if (split[0] == "TerrainRandomlyStart")
			TerrainRandomlyStart(split[1], Single.Parse(split[2]));
		if (split[0] == "TerrainExpandSimmilarTypes")
			TerrainExpandSimmilarTypes(Int32.Parse(split[1]), split[2]);
	}

	private void ReplaceWellKnownValues(string[] strs)
	{
		for (int i = 0; i < strs.Length; i++)
		{
			if(strs[i] == "MinGroundHeight")
				strs[i] = Globals.MinGroundHeight.ToString();
			if (strs[i] == "MountainHeight")
				strs[i] = Globals.MountainHeight.ToString();
		}
	}


	//HEIGHTS
	public void HeightsDefaultFill(HeightLevel height)
	{
		Map.FillMapWithHeight(height.Height);
	}

	public void HeightRandomlyPlace(HeightLevel height, float occurancesPer80Square)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width - 1), Random.Range(0, Map.Map.Height - 1));
			Map.Map.Get(randPos).Height = height.Height;
		}
	}

	public void HeightRandomlyPlaceNotInWater(HeightLevel height, float occurancesPer80Square)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width - 1), Random.Range(0, Map.Map.Height - 1));
			if(Map.Map.Get(randPos).Height > 0)
				Map.Map.Get(randPos).Height = height.Height;
		}
	}

	public void HeightRandomlyPlaceAlongEdges(HeightLevel height, float occurancesPer80Square)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos;
			if (Helpers.Odds(0.5f))
			{
				int randX = Random.Range(0, Map.Map.Width - 1);
				randPos = new Int2(randX, 0);
				if (Helpers.Odds(0.5f))
				{
					randPos = new Int2(randX, Map.Map.Height-1);
				}
			}
			else
			{
				int randY = Random.Range(0, Map.Map.Height - 1);
				randPos = new Int2(randY, 0);
				if (Helpers.Odds(0.5f))
				{
					randPos = new Int2(Map.Map.Width - 1, randY);
				}
			}

			if (Map.Map.Get(randPos).Height > 0)
				Map.Map.Get(randPos).Height = height.Height;
		}
	}

	public void HeightRandomlyPlaceAlongLine(HeightLevel height, float occurancesPer80Square, float minLength, float maxLength, float spacing)
	{
		int numPlacements = (int)Helpers.Randomize(occurancesPer80Square);
		for (int i = 0; i < numPlacements; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width - 1), Random.Range(0, Map.Map.Height - 1));
			PlaceHeightAlongVector(randPos, height, new Int2(Random.Range(-1, 1), Random.Range(-1, 1)), Random.Range(minLength, maxLength), spacing);
		}
	}

	private void PlaceHeightAlongVector(Int2 startPos, HeightLevel height, Int2 direction, float length, float spacing)
	{
		Int2 currPixel = startPos;
		for (int k = 0; k < (int)length; k++)
		{
			Map.Map.Get(currPixel).Height = height.Height;
			currPixel = GetNextPixelInDirection(currPixel, direction, (int)spacing);
			if (!Map.Map.PosInBounds(currPixel))
				return;
		}
	}

	protected Int2 GetNextPixelInDirection(Int2 currPoint, Int2 direction, int distBetweenPixles)
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

	public void HeightRandomlyExpandLevelFromItselfOrLevel(HeightLevel height, HeightLevel adjacentExpansionHeight, float minExpansion, float maxExpansion)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (height.Compare(Map.Map.Get(point).Height) || (HeightBordersLevel(point, adjacentExpansionHeight) && !adjacentExpansionHeight.Compare(Map.Map.Get(point).Height)))
				HeightExpandFromPoint(point, height, Random.Range(minExpansion, maxExpansion), adjacentExpansionHeight);
		}
	}

	public void HeightRandomlyExpandLevel(HeightLevel height, float avgExpansion)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (height.Compare(Map.Map.Get(point).Height))
				HeightExpandFromPoint(point, height, Helpers.Randomize(avgExpansion), new HeightLevel("-1"));
		}
	}

	//TODO: FIX!?!
	private void HeightExpandFromPoint(Int2 point, HeightLevel height, float numExpansionsLevels, HeightLevel ignoreLevel)
	{
		SortedDupList<Int2> HeightFrontier = new SortedDupList<Int2>();
		HeightFrontier.Insert(point, numExpansionsLevels);
		while (HeightFrontier.Count > 0)
		{
			Int2 currPos = HeightFrontier.TopValue();
			float currStrength = HeightFrontier.TopKey();
			HeightFrontier.Pop();
			Map.Map.Get(currPos).Height = height.Height;
			foreach (Int2 pos in Map.Map.GetAdjacentPoints(currPos))
			{
				if (!HeightFrontier.ContainsValue(pos) && !ignoreLevel.Compare(Map.Map.Get(pos).Height) && currStrength > 0)
				{
					HeightFrontier.Insert(pos, currStrength - 1);
				}
			}
		}
	}

	public void HeightRandomizeLevelEdges(HeightLevel height, int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Map.Map.GetMapPoints())
			{
				if (HeightBordersLevel(point, height))
				{
					if (Helpers.Odds(0.4f))
						Map.Map.Get(point).Height = height.Height;
				}
			}
		}
	}

	private bool HeightBordersLevel(Int2 tile, HeightLevel height)
	{
		foreach (Int2 neighbor in Map.Map.GetAdjacentPoints(tile))
		{
			if (height.Compare(Map.Map.Get(neighbor).Height))
				return true;
		}
		return false;
	}

	public void HeightBlendUp(int numPasses)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Map.Map.GetMapPoints())
			{
				float avg = NeighborAverageHeightAbove(point);
				if (Map.Map.Get(point).Height > 0 && (Map.Map.Get(point).Height > Globals.MinGroundHeight || avg > Globals.MinGroundHeight))
				{
					if(avg > Map.Map.Get(point).Height)
						Map.Map.Get(point).Height = (Map.Map.Get(point).Height + avg) / 2;
				}
			}
		}
	}

	private float NeighborAverageHeightAbove(Int2 pixle)
	{
		float sum = 0f;
		var points = Map.Map.GetAdjacentValues(pixle);
		int count = 0;
		foreach (MapTileModel pt in points)
		{
			if (pt.Height > Map.Map.Get(pixle).Height)
			{
				sum += pt.Height;
				count++;
			}
		}
		return sum / count;
	}

	public void HeightSetEdges(float edgeHeight)
	{
		for (int i = 0; i < Map.Map.Width; i++)
		{
			Map.Map.Get(new Int2(i, 0)).Height = edgeHeight;
			Map.Map.Get(new Int2(i, 1)).Height = edgeHeight;
			Map.Map.Get(new Int2(i, Map.Map.Height - 2)).Height = edgeHeight;
			Map.Map.Get(new Int2(i, Map.Map.Height - 1)).Height = edgeHeight;
		}
		for (int i = 0; i < Map.Map.Height; i++)
		{
			Map.Map.Get(new Int2(0, i)).Height =  edgeHeight;
			Map.Map.Get(new Int2(1, i)).Height =  edgeHeight;
			Map.Map.Get(new Int2(Map.Map.Width - 2, i)).Height =  edgeHeight;
			Map.Map.Get(new Int2(Map.Map.Width - 1, i)).Height =  edgeHeight;
		}
	}

	public void CreateRivers(int minNumRivers, int maxNumRivers)
	{
		int numOfRivers = Random.Range(minNumRivers, maxNumRivers);

		List<Int2> possibleRiverStarts = new List<Int2>();
		for (int i = 0; i < numOfRivers * 500; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width), Random.Range(0, Map.Map.Height));

			if (Map.Map.Get(randPos).Height < Globals.MountainHeight && Map.Map.Get(randPos).Height >= Globals.MinGroundHeight)
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
						Map.Map.Get(px).Height = Globals.MinGroundHeight - 0.05f;
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
		foreach (MapTileModel h in Map.Map.GetAdjacentValues(startPos))
		{
			if (h.Height >= Globals.MountainHeight)
				numMountains++;
			else if (h.Height == 0)
				numOceans++;
			else numOthers++;
		}

		if (numOceans == 0 && numMountains > 0 && numMountains < 4)
			return 1f;
		else return 0f;
	}

	private List<Int2> TryExpandRiver(Int2 pos, List<Int2> currPath)
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
				if (Map.Map.Get(neighbor).Height < Globals.MinGroundHeight)
				{
					endTile = shortestTile;
					break;
				}
				else if (Map.Map.Get(neighbor).Height <= Map.Map.Get(shortestTile).Height + 0.02f)
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
			if (checkedTiles.Get(neighbor) == 0 && Map.Map.Get(neighbor).Height < Globals.MountainHeight)
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




	//TERRAIN
	public void TerrainDefaultFill(string defaultTerrain)
	{
		Map.FillMapWithTerrain(defaultTerrain);
	}

	public void TerrainFillInOceans(string ocean)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).Height < Globals.MinGroundHeight)
				Map.Map.Get(point).TerrainId = ocean;
		}
	}

	public void TerrainFillInSeaLevel(string seaLevel)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (IsSeaLevel(point))
				Map.Map.Get(point).TerrainId = seaLevel;
		}
	}

	public void TerrainFillInMountains(string mountain)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).Height >= Globals.MountainHeight)
				Map.Map.Get(point).TerrainId = mountain;
		}
	}

	public void TerrainFillInRivers(string river)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (Map.Map.Get(point).Height < Globals.MinGroundHeight && NumBordersOfAtLeaseHeight(point, Globals.MinGroundHeight) >= 6)
				Map.Map.Get(point).TerrainId = river;
		}
	}

	private int NumBordersOfAtLeaseHeight(Int2 point, float height)
	{
		return Map.Map.GetAllNeighboringValues(point).Select((h) => h.Height >= height).Count();
	}

	public void TerrainEncourageStartAlongMountains(string terrain, float odds)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if(IsSeaLevel(point) && Helpers.Odds(odds) && BordersMountain(point))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	public void TerrainEncourageStartAlongOcean(string terrain, float odds)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(odds) && BordersOcean(point))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	public void TerrainRandomlyStart(string terrain, float odds)
	{
		foreach (Int2 point in Map.Map.GetMapPoints())
		{
			if (IsSeaLevel(point) && Helpers.Odds(odds))
				Map.Map.Get(point).TerrainId = terrain;
		}
	}

	//TODO: FIX!?!
	public void TerrainExpandSimmilarTypes(int numPasses, string typeToExpand)
	{
		for (int i = 0; i < numPasses; i++)
		{
			foreach (Int2 point in Map.Map.GetMapPoints())
			{
				if (IsSeaLevel(point))
				{
					int numAdjacent = GetAdjacentSeaLevelOfType(point, typeToExpand);
					if (Helpers.Odds(0.25f * numAdjacent))
						Map.Map.Get(point).TerrainId = typeToExpand;
				}
				
			}
		}
	}

	private int GetAdjacentSeaLevelOfType(Int2 point, string type)
	{
		int num = 0;
		foreach (Int2 adjacent in Map.Map.GetAdjacentPoints(point))
		{
			if (IsSeaLevel(adjacent) && Map.Map.Get(adjacent).TerrainId == type)
				num++;
		}
		return num;
	}

	private bool IsSeaLevel(Int2 point)
	{
		return Map.Map.Get(point).Height < Globals.MountainHeight && Map.Map.Get(point).Height >= Globals.MinGroundHeight;
	}

	protected bool BordersMountain(Int2 tile)
	{
		foreach (MapTileModel neighbor in Map.Map.GetAllNeighboringValues(tile))
		{
			if (neighbor.Height >= Globals.MountainHeight)
				return true;
		}
		return false;
	}

	protected bool BordersOcean(Int2 tile)
	{
		foreach (Int2 neighbor in Map.Map.GetAdjacentPoints(tile))
		{
			if (Map.Map.Get(neighbor).Height < Globals.MinGroundHeight)
				return true;
		}
		return false;
	}
}
