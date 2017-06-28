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
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.City)
			PlaceCityOnTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Forest)
			PlaceForestOnTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Grass)
			PlaceWildernessOnTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Fertile)
			PlaceFarmsOnTile(tile);
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

	private void PlaceHovelsOnTile(Int2 tile, int numHovels)
	{
		for (int i = 0; i < numHovels; i++)
			PlaceHovelOnTile(tile);
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

	private void PlaceCityOnTile(Int2 tile)
	{
		foreach(Int2 pt in map.GetAdjacentPoints(tile))
		{
			if(TileIsCityBorder(pt))
			{
				PlaceWallOnEdge(tile, pt);
			}
		}
		PlaceTurretsOnCorners(tile);

		PlaceHovelsOnTile(tile, Random.Range(20, 25));
	}

	private void PlaceWallOnEdge(Int2 cityTile, Int2 nonCityTile)
	{
		if((nonCityTile - cityTile).Equals(new Int2(0, -1)))
			SpawnWall(new Vector3(cityTile.X + 0.5f, 2f, cityTile.Y), 90f);
		if ((nonCityTile - cityTile).Equals(new Int2(0, 1)))
			SpawnWall(new Vector3(cityTile.X + 0.5f, 2f, cityTile.Y+1), 90f);
		if ((nonCityTile - cityTile).Equals(new Int2(1, 0)))
			SpawnWall(new Vector3(cityTile.X+1, 2f, cityTile.Y + .5f), 0);
		if ((nonCityTile - cityTile).Equals(new Int2(-1, 0)))
			SpawnWall(new Vector3(cityTile.X, 2f, cityTile.Y + .5f), 0);
	}

	private void SpawnWall(Vector3 pos, float yRot)
	{
		RaycastHit hit;
		if(Physics.Raycast(new Ray(pos, Vector3.down), out hit))
		{
			GameObject wall = GameObject.Instantiate(lookup.Wall, objectParent);
			wall.transform.position = hit.point;
			wall.transform.eulerAngles = new Vector3(0, yRot, 0);
		}
	}

	private void PlaceTurretsOnCorners(Int2 tile)
	{
		TryPlaceTurret(new Vector3(tile.X, 2f, tile.Y), tile + new Int2(-1, -1), tile + new Int2(0, -1), tile + new Int2(-1, 0));
		TryPlaceTurret(new Vector3(tile.X+1, 2f, tile.Y), tile + new Int2(1, -1), tile + new Int2(0, -1), tile + new Int2(1, 0));
		TryPlaceTurret(new Vector3(tile.X, 2f, tile.Y+1), tile + new Int2(-1, 1), tile + new Int2(0, 1), tile + new Int2(-1, 0));
		TryPlaceTurret(new Vector3(tile.X+1, 2f, tile.Y+1), tile + new Int2(1, 1), tile + new Int2(0, 1), tile + new Int2(1, 0));
	}

	private void TryPlaceTurret(Vector3 pos, Int2 check1, Int2 check2, Int2 check3)
	{
		if(TileIsCityBorder(check1) || TileIsCityBorder(check2) || TileIsCityBorder(check3))
		{
			RaycastHit hit;
			if(Physics.Raycast(new Ray(pos, Vector3.down), out hit))
			{
				GameObject turret = GameObject.Instantiate(lookup.Turret, objectParent);
				turret.transform.position = hit.point;
			}
		}
	}

	private bool TileIsCityBorder(Int2 tile)
	{
		return map.PosInBounds(tile) &&
			map.GetValueAt(tile).tileType != TerrainTile.TileType.City &&
			map.GetValueAt(tile).tileType != TerrainTile.TileType.Ocean &&
			map.GetValueAt(tile).tileType != TerrainTile.TileType.River;
	}

	private Vector3? GetModelPlacementPos(Int2 tile)
	{
		Vector3 raycastPos = new Vector3(Random.Range(tile.X, tile.X + 1f), 3f, Random.Range(tile.Y, tile.Y + 1f));
		RaycastHit hit;
		if (Physics.Raycast(new Ray(raycastPos, Vector3.down), out hit))
		{
			if (hit.collider.gameObject.layer != LayerMask.NameToLayer("PlacedModel") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Ocean"))
			{
				return hit.point;
			}
		}
		return null;
	}
}
