using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer
{
	Map2D<TerrainTile> map;
	Map2D<float> heights;
	ModelLookup lookup;
	Transform objectParent;

	public void PlaceModels(Map2D<TerrainTile> terrainMap, Map2D<float> heightsMap, ModelLookup lu, Transform par)
	{
		map = terrainMap;
		heights = heightsMap;
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
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Swamp)
			PlaceSwampOnTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Grass)
			PlaceWildernessOnTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Fertile)
			PlaceFarmsOnTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Road)
			PlaceBridgesOnTile(tile);
	}

	private void PlaceForestOnTile(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(7, 10), lookup.PineTree, true);
		if (Helpers.Odds(0.05f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
	}

	private void PlaceSwampOnTile(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(0, 4), lookup.Willow);
		if (Helpers.Odds(0.03f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
	}

	private void PlaceWildernessOnTile(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(0, 3), lookup.PineTree);
		if (Helpers.Odds(0.1f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
	}

	private void PlaceFarmsOnTile(Int2 tile)
	{
		if (Helpers.Odds(0.4f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
		PlaceObjectsOnTile(tile, Random.Range(10, 15), lookup.WheatField);
	}

	private void PlaceBridgesOnTile(Int2 tile)
	{
		if(heights.GetValueAt(tile) < Globals.MinGroundHeight)
		{
			var bridge = GameObject.Instantiate(lookup.Bridge, objectParent);
			bridge.transform.position = new Vector3(tile.X + 0.5f, Globals.MinGroundHeight*2f, tile.Y + 0.5f);

			if ((map.GetValueAt(tile + new Int2(-1, 0)).tileType == TerrainTile.TileType.Road ||
				map.GetValueAt(tile + new Int2(-1, 0)).tileType == TerrainTile.TileType.City) &&
				(map.GetValueAt(tile + new Int2(1, 0)).tileType == TerrainTile.TileType.Road ||
				map.GetValueAt(tile + new Int2(1, 0)).tileType == TerrainTile.TileType.City))
			{
				bridge.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			}
			else if ((map.GetValueAt(tile + new Int2(0, -1)).tileType == TerrainTile.TileType.Road ||
				map.GetValueAt(tile + new Int2(0, -1)).tileType == TerrainTile.TileType.City) &&
				(map.GetValueAt(tile + new Int2(0, 1)).tileType == TerrainTile.TileType.Road ||
				map.GetValueAt(tile + new Int2(0, 1)).tileType == TerrainTile.TileType.City))
			{
				bridge.transform.eulerAngles = new Vector3(0f, 90f, 0f);
			}
			else GameObject.Destroy(bridge);
		}
	}

	private void PlaceCityOnTile(Int2 tile)
	{
		foreach(Int2 pt in map.GetAdjacentPoints(tile))
		{
			if (TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.Gates);
			}
			else if(TileIsCityBorder(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.Wall);
			}
		}
		PlaceTurretsOnCorners(tile);

		PlaceObjectsOnTile(tile, Random.Range(10, 15), lookup.Hovel, true);
	}

	private bool TileIsRoad(Int2 tile)
	{
		return map.PosInBounds(tile) && map.GetValueAt(tile).tileType == TerrainTile.TileType.Road;
	}

	private bool TileIsCityBorder(Int2 tile)
	{
		return map.PosInBounds(tile) &&
			map.GetValueAt(tile).tileType != TerrainTile.TileType.City &&
			map.GetValueAt(tile).tileType != TerrainTile.TileType.Ocean &&
			map.GetValueAt(tile).tileType != TerrainTile.TileType.River;
	}




	public class PlacementTrans
	{
		public Vector3 pos;
		public Vector3 rot;
		public bool valid = false;

		public PlacementTrans(Vector3 p, Vector3 r, bool forcePlacement = false)
		{
			RaycastHit hit;

			//int layerMask = ~(1 << LayerMask.NameToLayer("Ocean") | (1 << LayerMask.NameToLayer("PlacedModel")));
			if (Physics.Raycast(new Ray(p, Vector3.down), out hit))
			{
				if (forcePlacement ||
					(hit.collider.gameObject.layer != LayerMask.NameToLayer("Ocean") &&
					hit.collider.gameObject.layer != LayerMask.NameToLayer("PlacedModel")))
				{
					pos = hit.point;
					rot = r;
					valid = true;
				}
			}
		}
	}

	private void PlaceObjectsOnTile(Int2 tile, int num, GameObject objToPlace, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetRandomPlacementTrans(tile, forcePlacement), objToPlace);
	}

	private PlacementTrans GetEdgePlacementTrans(Int2 myTile, Int2 edgeTile, bool forcePlacement = false)
	{
		if ((edgeTile - myTile).Equals(new Int2(0, -1)))
			return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y), new Vector3(0, 90f, 0), forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(0, 1)))
			return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 1), new Vector3(0, 90f, 0), forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(1, 0)))
			return new PlacementTrans(new Vector3(myTile.X + 1, 2f, myTile.Y + .5f), new Vector3(0, 0, 0), forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(-1, 0)))
			return new PlacementTrans(new Vector3(myTile.X, 2f, myTile.Y + .5f), new Vector3(0, 0, 0), forcePlacement);
		return null;
	}

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 360f), 0);
		return GetRandomPlacementTrans(myTile, rot, forcePlacement);
	}

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, Vector3 rot, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(Random.Range(myTile.X, myTile.X + 1f), 3f, Random.Range(myTile.Y, myTile.Y + 1f)), rot, forcePlacement);
	}

	private void SpawnObjectAtPos(PlacementTrans trans, GameObject obj)
	{
		if (trans.valid)
		{
			GameObject newObj = GameObject.Instantiate(obj, objectParent);
			newObj.transform.position = trans.pos;
			newObj.transform.eulerAngles = trans.rot;
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
}
