using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundGenerator : InitialMapGenerator, IMapGenerator
{
	public Map GenerateMaps(int width, int height, MapEnvironment env)
	{
		MapGenerator.Environment = env;
		Heights = new Map2D<float>(width, height);
		Terrain = new Map2D<GroundInfo>(width, height);
		ExecuteApiCommands(env.MapBuildingCommands);
		//MakeHeights();

		//MakeTerrain();

		return new Map(Heights, Terrain);
	}

	private void MakeHeights()
	{
		/*HeightsDefaultFill(1f);
		HeightRandomlyPlace(0f, 21f);
		HeightRandomlyExpandLevel(0f, 2);
		HeightRandomizeLevelEdges(0f, 2);
		HeightRandomlyPlaceAlongLine(Globals.MinGroundHeight, 25, 5, 15, 3);
		HeightRandomlyExpandLevelFromItselfOrLevel(Globals.MinGroundHeight, 0, 1, 3);
		HeightRandomlyPlace(1f, 150f);
		HeightSetEdges(1f);*/
	}
	
	private void MakeTerrain()
	{
		/*TerrainDefaultFill("CaveWall");
		TerrainFillInOceans("Ocean");
		TerrainFillInSeaLevel("CaveFloor");
		TerrainEncourageStartAlongMountains("MushroomForest", 0.1f);
		TerrainRandomlyStart("MushroomForest", 0.02f);
		TerrainExpandSimmilarTypes(2, "MushroomForest");

		TerrainRandomlyStart("MushroomForest", 0.02f);*/
	}
}
