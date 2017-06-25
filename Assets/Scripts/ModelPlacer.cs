using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer
{
	Map2D<TerrainTile> map;
	ModelLookup lookup;
	Transform objectParent;

	public void PlaceModels(Map2D<TerrainTile> terrainMap, ModelLookup lu, Transform par)
	{
		map = terrainMap;
		lookup = lu;
		objectParent = par;
		foreach (Int2 tile in map.GetMapPoints())
		{
			PlaceModelsOnPoint(tile);
		}
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
		if (Helpers.Odds(0.05f))
			PlaceHovelOnTile(tile);
		PlaceTreesOnTile(tile, Random.Range(7, 10));
	}

	private void PlaceWildernessOnTile(Int2 tile)
	{
		PlaceTreesOnTile(tile, Random.Range(0, 3));
		if (Helpers.Odds(0.1f))
			PlaceHovelOnTile(tile);
	}

	private void PlaceTreesOnTile(Int2 tile, int numOfTrees)
	{
		for (int i = 0; i < numOfTrees; i++)
			PlaceTreeOnTile(tile);
	}

	private void PlaceTreeOnTile(Int2 tile)
	{
		var pos = GetModelPlacementPos(tile);
		if (pos != null)
		{
			GameObject tree = GameObject.Instantiate(lookup.PineTree, objectParent);
			tree.transform.position = pos.Value;
			tree.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
		}
	}

	private void PlaceFarmsOnTile(Int2 tile)
	{
		if (Helpers.Odds(0.4f))
			PlaceHovelOnTile(tile);
		PlaceFieldsOnTile(tile, Random.Range(10, 15));
	}

	private void PlaceFieldsOnTile(Int2 tile, int numOfFarms)
	{
		for (int i = 0; i < numOfFarms; i++)
			PlaceFieldOnTile(tile);
	}

	private void PlaceFieldOnTile(Int2 tile)
	{
		var pos = GetModelPlacementPos(tile);
		if (pos != null)
		{
			GameObject field;
			field = GameObject.Instantiate(lookup.WheatField, objectParent);
			field.transform.position = pos.Value;
			field.transform.eulerAngles = new Vector3(0, Random.Range(60, 120), 0);
		}
	}

	private void PlaceHovelOnTile(Int2 tile)
	{
		var pos = GetModelPlacementPos(tile);
		if (pos != null)
		{
			GameObject hovel = GameObject.Instantiate(lookup.Hovel, objectParent);
			hovel.transform.position = pos.Value;
			hovel.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
		}
	}

	private Vector3? GetModelPlacementPos(Int2 tile)
	{
		Vector3 raycastPos = new Vector3(Random.Range(tile.X, tile.X + 1f), 3f, Random.Range(tile.Y, tile.Y + 1f));
		RaycastHit hit;
		if (Physics.Raycast(new Ray(raycastPos, Vector3.down), out hit))
		{
			if (hit.collider.gameObject.layer != LayerMask.NameToLayer("PlacedModel") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Water"))
			{
				return hit.point;
			}
		}
		return null;
	}
}
