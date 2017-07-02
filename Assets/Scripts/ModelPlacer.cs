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
			PlaceCityTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Forest)
			PlaceForestTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Swamp)
			PlaceSwampTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Grass)
			PlaceWildernessTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Fertile)
			PlaceFarmTile(tile);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Road)
			PlaceRoadTile(tile);
	}

	private void PlaceForestTile(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(7, 10), lookup.PineTree);
		if (Helpers.Odds(0.05f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
	}

	private void PlaceSwampTile(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(0, 4), lookup.Willow);
		if (Helpers.Odds(0.03f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
	}

	private void PlaceWildernessTile(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(0, 3), lookup.PineTree);
		if (Helpers.Odds(0.1f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
	}

	private void PlaceFarmTile(Int2 tile)
	{
		if (Helpers.Odds(0.4f))
			PlaceObjectsOnTile(tile, 1, lookup.Hovel);
		PlaceObjectsOnTile(tile, Random.Range(10, 15), lookup.WheatField);
	}

	private void PlaceRoadTile(Int2 tile)
	{
		if(heights.GetValueAt(tile) < Globals.MinGroundHeight)
		{
			foreach(var t in map.GetAdjacentPoints(tile))
			{
				if(map.GetValueAt(t).tileType == TerrainTile.TileType.Road ||
					map.GetValueAt(t).tileType == TerrainTile.TileType.City)
				{
					if (heights.GetValueAt(t) >= Globals.MinGroundHeight)
					{
						Vector3 rot = new Vector3();
						if (t.Y != tile.Y)
							rot = new Vector3(0, 90f, 0);
						SpawnObjectAtPos(GetCenterPlacementTrans(tile, rot, true), lookup.Bridge);
						break;
					}
				}
			}
		}
	}

	private void PlaceCityTile(Int2 tile)
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

	private void PlaceTurretsOnCorners(Int2 tile)
	{
		foreach(var t in map.GetDiagonalPoints(tile))
			TryPlaceTurret(tile, t);
	}

	private void TryPlaceTurret(Int2 tile, Int2 diagTile)
	{
		if (TileIsCityBorder(diagTile) || 
			TileIsCityBorder(tile + new Int2(diagTile.X - tile.X, 0)) || 
			TileIsCityBorder(tile + new Int2(0, diagTile.Y - tile.Y)))
		{
			SpawnObjectAtPos(GetPlacementBetweenTileCenters(tile, diagTile, true), lookup.Turret);
		}
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

			int layerMask = ~(1 << 40);
			if(forcePlacement)
				layerMask = ~(1 << LayerMask.NameToLayer("Ocean") | (1 << LayerMask.NameToLayer("PlacedModel")));
			if (Physics.Raycast(new Ray(p, Vector3.down), out hit, 10f, layerMask))
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

	private PlacementTrans GetPlacementBetweenTileCenters(Int2 tile1, Int2 tile2, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(tile1.X + ((tile2.X - tile1.X) / 2f) + .5f, 2f, tile1.Y + ((tile2.Y - tile1.Y) / 2f) + .5f), Vector3.zero, forcePlacement);
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

	private PlacementTrans GetCenterPlacementTrans(Int2 myTile, Vector3 rot, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 0.5f), rot);
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
}
