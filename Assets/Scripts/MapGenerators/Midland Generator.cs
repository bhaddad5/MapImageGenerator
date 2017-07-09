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
		HeightBlendUp(2);
		HeightRandomlyPlaceNotInWater(.2f, 500);
		HeightBlendUp(1);
		HeightSetEdges(0f);
		HeightRandomizeLevelEdges(0f, 2);
		CreateRivers(10, 20);
	}

	private void MakeTerrain()
	{
		TerrainDefaultFill("Wilderness");
		TerrainFillInOceans("Ocean");
		TerrainFillInMountains("Mountain");
		TerrainFillInRivers("River");
		TerrainEncourageStartAlongMountains("Forest", 0.15f);
		TerrainExpandSimmilarTypes(6, "Forest");
		TerrainRandomlyStart("Forest", 0.05f);
		TerrainExpandSimmilarTypes(3, "Forest");
		TerrainEncourageStartAlongOcean("Swamp", 0.05f);
		TerrainRandomlyStart("Swamp", 0.04f);
		TerrainExpandSimmilarTypes(3, "Swamp");
		TerrainEncourageStartAlongOcean("Fertile", 0.15f);
		TerrainRandomlyStart("Fertile", 0.05f);
		TerrainExpandSimmilarTypes(4, "Fertile");
	}
}
