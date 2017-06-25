using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer
{
	Map2D<TerrainTile> map;
	ModelLookup lookup;

	int numTrees = 0;
	public void PlaceModels(Map2D<TerrainTile> terrainMap, ModelLookup lu)
	{
		map = terrainMap;
		lookup = lu;
		foreach (Int2 tile in map.GetMapPoints())
		{
			PlaceModelsOnPoint(tile);
		}

		Debug.Log(numTrees);
	}

	private void PlaceModelsOnPoint(Int2 tile)
	{
		if(map.GetValueAt(tile).tileType == TerrainTile.TileType.Forest)
		{
			PlaceForestOnTile(tile);
		}

		if(map.GetValueAt(tile).tileType == TerrainTile.TileType.Grass)
		{
			PlaceWildernessOnTile(tile);
		}

		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Fertile)
		{
			PlaceFarmsOnTile(tile);
		}
	}

	private void PlaceForestOnTile(Int2 tile)
	{
		PlaceTreesOnTile(tile, Random.Range(7, 10));
	}

	private void PlaceWildernessOnTile(Int2 tile)
	{
		PlaceTreesOnTile(tile, Random.Range(1, 3));
	}

	private void PlaceFarmsOnTile(Int2 tile)
	{
	}

	private void PlaceTreesOnTile(Int2 tile, int numOfTrees)
	{
		for (int i = 0; i < numOfTrees; i++)
			PlaceTreeOnTile(tile);
	}

	private void PlaceTreeOnTile(Int2 tile)
	{
		Vector3 raycastPos = new Vector3(Random.Range(tile.X, tile.X + 1f), 3f, Random.Range(tile.Y, tile.Y + 1f));
		RaycastHit hit;
		if(Physics.Raycast(new Ray(raycastPos, Vector3.down), out hit))
		{
			if(hit.collider.gameObject.layer != LayerMask.NameToLayer("PlacedModel") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Water"))
			{
				GameObject tree = GameObject.Instantiate(lookup.PineTree);
				tree.transform.position = hit.point;
				tree.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
				numTrees++;
			}
		}
	}
}
