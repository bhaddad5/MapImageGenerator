using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer
{
	Map2D<TerrainTile> map;
	Map2D<float> heights;
	Map2D<RegionTile> regions;
	ModelLookup lookup;
	Transform objectParent;
	List<Kingdom> kingdoms;

	public void PlaceModels(Map2D<TerrainTile> terrainMap, Map2D<float> heightsMap, Map2D<RegionTile> regionsMap, List<Kingdom> ks, ModelLookup lu, Transform par)
	{
		map = terrainMap;
		heights = heightsMap;
		kingdoms = ks;
		regions = regionsMap;
		lookup = lu;
		objectParent = par;
		foreach (Int2 tile in map.GetMapPoints())
		{
			HandleTile(tile);
		}
	}

	private void HandleTile(Int2 tile)
	{
		Culture culture = regions.GetValueAt(tile).settlement.kingdom.culture;
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.City)
			PlaceCityTile(tile, culture);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Forest)
			PlaceForestTile(tile, culture);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Swamp)
			PlaceSwampTile(tile, culture);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Grass)
			PlaceWildernessTile(tile, culture);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Fertile)
			PlaceFarmTile(tile, culture);
		if (map.GetValueAt(tile).tileType == TerrainTile.TileType.Road)
			PlaceRoadTile(tile, culture);
	}

	private void PlaceForestTile(Int2 tile, Culture culture)
	{
		PlaceObjectsOnTile(tile, Random.Range(7, 10), lookup.PineTree);
		if (culture == CultureDefinitions.Anglo)
		{
			if (Helpers.Odds(0.05f))
				PlaceObjectsOnTile(tile, 1, lookup.Hovel);
		}
		else if (culture == CultureDefinitions.Dwarf)
		{
			if (Helpers.Odds(0.05f))
				PlaceObjectsOnTile(tile, 1, lookup.DwarfHouse);
		}
		else if (culture == CultureDefinitions.Orc)
		{
			if (Helpers.Odds(0.1f))
				PlaceObjectsOnTile(tile, 1, lookup.OrcHut);
		}
	}

	private void PlaceSwampTile(Int2 tile, Culture culture)
	{
		PlaceObjectsOnTile(tile, Random.Range(0, 4), lookup.Willow);
		PlaceObjectsOnTile(tile, Random.Range(5, 9), lookup.Rushes);
		if (culture == CultureDefinitions.Anglo)
		{
			if (Helpers.Odds(0.03f))
				PlaceObjectsOnTile(tile, 1, lookup.Hovel);
		}
		else if (culture == CultureDefinitions.Dwarf)
		{

		}
		if (culture == CultureDefinitions.Orc)
		{
			if (Helpers.Odds(0.1f))
				PlaceObjectsOnTile(tile, 1, lookup.OrcHut);
		}
	}

	private void PlaceWildernessTile(Int2 tile, Culture culture)
	{
		PlaceObjectsOnTile(tile, Random.Range(0, 3), lookup.PineTree);
		if (culture == CultureDefinitions.Anglo)
		{
			if (Helpers.Odds(0.1f))
				PlaceObjectsOnTile(tile, 1, lookup.Hovel);
		}
		else if (culture == CultureDefinitions.Dwarf)
		{
			if (Helpers.Odds(0.15f))
				PlaceObjectsOnTile(tile, 1, lookup.DwarfHouse);
		}
		else if (culture == CultureDefinitions.Orc)
		{
			if (Helpers.Odds(0.1f))
				PlaceObjectsOnTile(tile, 1, lookup.OrcHut);
		}
	}

	private void PlaceFarmTile(Int2 tile, Culture culture)
	{
		if (culture == CultureDefinitions.Anglo)
		{
			PlaceObjectsOnTile(tile, Random.Range(10, 15), lookup.WheatField);
			if (Helpers.Odds(0.4f))
				PlaceObjectsOnTile(tile, 1, lookup.Hovel);
		}
		else if(culture == CultureDefinitions.Dwarf)
		{
			PlaceObjectsOnTile(tile, Random.Range(10, 15), lookup.WheatField);
			if (Helpers.Odds(0.15f))
				PlaceObjectsOnTile(tile, 1, lookup.DwarfHouse);
		}
		else if (culture == CultureDefinitions.Orc)
		{
			if (Helpers.Odds(0.1f))
				PlaceObjectsOnTile(tile, 1, lookup.OrcHut);
		}
	}

	private void PlaceRoadTile(Int2 tile, Culture culture)
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

	private void PlaceCityTile(Int2 tile, Culture culture)
	{
		Settlement sett = GetSettlementFromTile(tile);
		List<Settlement.CityTrait> traits = sett.GetCityTraits(map);
		if (culture == CultureDefinitions.Anglo)
		{
			if (traits.Contains(Settlement.CityTrait.Small))
				PlaceSmallAngloCity(tile);
			if (traits.Contains(Settlement.CityTrait.Medium))
				PlaceMediumAngloCity(tile);
			if (traits.Contains(Settlement.CityTrait.Large))
				PlaceLargeAngloCity(tile);
		}
		else if (culture == CultureDefinitions.Dwarf)
		{
			if (traits.Contains(Settlement.CityTrait.Small))
				PlaceSmallDwarfCity(tile);
			if (traits.Contains(Settlement.CityTrait.Medium))
				PlaceMediumDwarfCity(tile);
			if (traits.Contains(Settlement.CityTrait.Large))
				PlaceLargeDwarfCity(tile);
		}
		else if (culture == CultureDefinitions.Orc)
		{
			if (traits.Contains(Settlement.CityTrait.Small))
				PlaceSmallOrcCity(tile);
			if (traits.Contains(Settlement.CityTrait.Medium))
				PlaceMediumOrcCity(tile);
			if (traits.Contains(Settlement.CityTrait.Large))
				PlaceLargeOrcCity(tile);
		}
	}

	private Settlement GetSettlementFromTile(Int2 tile)
	{
		foreach(var kingdom in kingdoms)
		{
			foreach(var sett in kingdom.settlements)
			{
				foreach(Int2 t in sett.cityTiles)
				{
					if (t.Equals(tile))
						return sett;
				}
			}
		}
		return null;
	}

	private void PlaceSmallAngloCity(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(20, 25), lookup.Hovel);
	}

	private void PlaceMediumAngloCity(Int2 tile)
	{
		foreach (Int2 pt in map.GetAdjacentPoints(tile))
		{
			if (TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.WoodenGate);
			}
			else if (TileIsCityBorder(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.WoodenWall);
			}
		}

		PlaceObjectsOnTileWithBorder(tile, Random.Range(10, 13), lookup.Hovel);
		PlaceObjectsOnTileWithBorder(tile, Random.Range(5, 8), lookup.TownHouse);
	}

	private void PlaceLargeAngloCity(Int2 tile)
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
		PlaceTurretsOnCorners(tile, lookup.Turret);

		PlaceObjectsOnTileWithBorder(tile, Random.Range(20, 25), lookup.TownHouse);
	}

	private void PlaceSmallDwarfCity(Int2 tile)
	{
		PlaceObjectsOnTileWithBorder(tile, Random.Range(20, 25), lookup.DwarfHouse);
	}

	private void PlaceMediumDwarfCity(Int2 tile)
	{
		PlaceLargeDwarfCity(tile);
	}

	private void PlaceLargeDwarfCity(Int2 tile)
	{
		foreach (Int2 pt in map.GetAdjacentPoints(tile))
		{
			if (TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.DwarfGates);
			}
			else if (TileIsCityBorder(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.DwarfWall);
			}
		}
		PlaceTurretsOnCorners(tile, lookup.DwarfTower);

		PlaceObjectsOnTileWithBorder(tile, Random.Range(20, 25), lookup.DwarfHouse);
	}

	private void PlaceSmallOrcCity(Int2 tile)
	{
		PlaceMediumOrcCity(tile);
	}

	private void PlaceMediumOrcCity(Int2 tile)
	{
		PlaceObjectsOnTile(tile, Random.Range(20, 25), lookup.OrcHut);

		foreach (Int2 pt in map.GetAdjacentPoints(tile))
		{
			if (TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.OrcGate);
			}
			else if (TileIsCityBorder(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), lookup.OrcWall);
			}
		}

		PlaceTurretsOnCorners(tile, lookup.OrcTower);
	}

	private void PlaceLargeOrcCity(Int2 tile)
	{
		PlaceMediumOrcCity(tile);
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

	private void PlaceTurretsOnCorners(Int2 tile, GameObject turret)
	{
		foreach (var t in map.GetDiagonalPoints(tile))
		{
			if (TileIsCityBorder(t) ||
			TileIsCityBorder(tile + new Int2(t.X - tile.X, 0)) ||
			TileIsCityBorder(tile + new Int2(0, t.Y - tile.Y)))
			{
				SpawnObjectAtPos(GetPlacementBetweenTileCenters(tile, t, true), turret);
			}
		}
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

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 360f), 0);
		return GetRandomPlacementTrans(myTile, rot, forcePlacement);
	}

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, Vector3 rot, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(Random.Range(myTile.X, myTile.X + 1f), 3f, Random.Range(myTile.Y, myTile.Y + 1f)), rot, forcePlacement);
	}

	private void PlaceObjectsOnTileWithBorder(Int2 tile, int num, GameObject objToPlace, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetRandomPlacementTransWithBorder(tile, forcePlacement), objToPlace);
	}

	private PlacementTrans GetRandomPlacementTransWithBorder(Int2 myTile, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 360f), 0);
		return GetRandomPlacementTransWithBorder(myTile, rot, forcePlacement);
	}

	private PlacementTrans GetRandomPlacementTransWithBorder(Int2 myTile, Vector3 rot, bool forcePlacement = false)
	{
		float border = 0.15f;
		return new PlacementTrans(new Vector3(Random.Range(myTile.X + border, myTile.X + 1f - border), 3f, Random.Range(myTile.Y + border, myTile.Y + 1f - border)), rot, forcePlacement);
	}

	private PlacementTrans GetEdgePlacementTrans(Int2 myTile, Int2 edgeTile, bool forcePlacement = false)
	{
		if ((edgeTile - myTile).Equals(new Int2(0, -1)))
			return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y), new Vector3(0, -90f, 0), forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(0, 1)))
			return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 1), new Vector3(0, 90f, 0), forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(1, 0)))
			return new PlacementTrans(new Vector3(myTile.X + 1, 2f, myTile.Y + .5f), new Vector3(0, 180, 0), forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(-1, 0)))
			return new PlacementTrans(new Vector3(myTile.X, 2f, myTile.Y + .5f), new Vector3(0, 0, 0), forcePlacement);
		return null;
	}

	private PlacementTrans GetCenterPlacementTrans(Int2 myTile, Vector3 rot, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 0.5f), rot);
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
