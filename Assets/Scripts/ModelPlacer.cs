using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ModelPlacer
{
	Transform objectParent;

	public void PlaceModels(Transform par)
	{
		objectParent = par;
		foreach (Int2 tile in MapGenerator.Terrain.GetMapPoints())
		{
			if (RegionsGen.Map.Get(tile) != null)
				PlaceCultureObjectsOnTile(tile, RegionsGen.Map.Get(tile).settlement.kingdom.culture, MapGenerator.Terrain.Get(tile).traits);
		}
		foreach (Int2 tile in MapGenerator.Terrain.GetMapPoints())
		{
			PlaceEnvironmentObjectsOnTile(tile, MapGenerator.Terrain.Get(tile).placementInfos);
		}

		CombineAllMeshes();
	}

	private void PlaceEnvironmentObjectsOnTile(Int2 tile, List<ModelPlacementInfo> infos)
	{
		foreach (var info in infos)
		{
			PlaceModels(tile, info.Model, info.Mode, info.NumToPlace);
		}
	}

	private void PlaceCultureObjectsOnTile(Int2 tile, Culture culture, List<GroundInfo.GroundTraits> traits)
	{
		foreach (GroundInfo.GroundTraits trait in traits)
		{
			if (culture.tileModelPlacement.ContainsKey(trait))
			{
				List<ModelPlacementInfo> placers = culture.tileModelPlacement[trait];
				foreach (ModelPlacementInfo placer in placers)
				{
					PlaceModels(tile, placer.Model, placer.Mode, placer.NumToPlace);
				}
			}
		}
	}

	private void PlaceModels(Int2 tile, GameObject obj, ModelPlacementInfo.PlacementMode mode, int num)
	{
		if (mode == ModelPlacementInfo.PlacementMode.Scattered)
			PlaceObjectsOnTile(tile, num, obj);
		if (mode == ModelPlacementInfo.PlacementMode.ScatteredBordered)
			PlaceObjectsOnTileWithBorder(tile, num, obj);
		if(mode == ModelPlacementInfo.PlacementMode.Corners)
			PlaceTurretsOnCorners(tile, obj);
		if(mode == ModelPlacementInfo.PlacementMode.CityWalls)
			PlaceWallsOnEdges(tile, obj);
		if(mode == ModelPlacementInfo.PlacementMode.CityGates)
			PlaceGatesOnEdges(tile, obj);
		if (mode == ModelPlacementInfo.PlacementMode.Bridge)
			PlaceBridgeOnTile(tile, obj);
	}



	private void PlaceWallsOnEdges(Int2 tile, GameObject wall)
	{
		foreach (Int2 pt in MapGenerator.Terrain.GetAdjacentPoints(tile))
		{
			if (TileIsCityBorder(pt) && !TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), wall);
			}
		}
	}

	private void PlaceGatesOnEdges(Int2 tile, GameObject gate)
	{
		foreach (Int2 pt in MapGenerator.Terrain.GetAdjacentPoints(tile))
		{
			if (TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, true), gate);
			}
		}
	}

	private bool TileIsRoad(Int2 tile)
	{
		return MapGenerator.Terrain.PosInBounds(tile) && MapGenerator.Terrain.Get(tile).HasTrait(GroundInfo.GroundTraits.Road);
	}

	private bool TileIsCityBorder(Int2 tile)
	{
		return MapGenerator.Terrain.PosInBounds(tile) &&
		       !MapGenerator.Terrain.Get(tile).HasTrait(GroundInfo.GroundTraits.Impassable) &&
		       !MapGenerator.Terrain.Get(tile).HasTrait(GroundInfo.GroundTraits.City);
	}

	private void PlaceTurretsOnCorners(Int2 tile, GameObject turret)
	{
		foreach (var t in MapGenerator.Terrain.GetDiagonalPoints(tile))
		{
			if (TileIsCityBorder(t) ||
			TileIsCityBorder(tile + new Int2(t.X - tile.X, 0)) ||
			TileIsCityBorder(tile + new Int2(0, t.Y - tile.Y)))
			{
				SpawnObjectAtPos(GetPlacementBetweenTileCenters(tile, t, true), turret);
			}
		}
	}

	private void PlaceBridgeOnTile(Int2 tile, GameObject bridge)
	{
		if (MapGenerator.Heights.Get(tile) < Globals.MinGroundHeight)
		{
			foreach (var t in MapGenerator.Terrain.GetAdjacentPoints(tile))
			{
				if (MapGenerator.Terrain.Get(t).HasTrait(GroundInfo.GroundTraits.Road) ||
				    MapGenerator.Terrain.Get(t).HasTrait(GroundInfo.GroundTraits.City))
				{
					if (MapGenerator.Heights.Get(t) >= Globals.MinGroundHeight)
					{
						Vector3 rot = new Vector3();
						if (t.Y != tile.Y)
							rot = new Vector3(0, 90f, 0);
						SpawnObjectAtPos(GetCenterPlacementTrans(tile, rot, true), bridge);
						break;
					}
				}
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
			p.y = 3f;
			if (Physics.Raycast(new Ray(p, Vector3.down), out hit, 10f, layerMask))
			{
				if (Vector3.Angle(hit.normal, Vector3.up) > 60f)
					return;

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
			newObj.SetActive(true);
			newObj.transform.position = trans.pos;
			newObj.transform.eulerAngles = trans.rot;
		}
	}





	private void CombineAllMeshes()
	{
		MeshFilter[] meshFilters = objectParent.GetComponentsInChildren<MeshFilter>();

		List<MeshCombine> combines = new List<MeshCombine>();
		int i = 0;
		while (i < meshFilters.Length)
		{
			bool foundMeshCombine = false;
			foreach (MeshCombine combine in combines)
			{
				if (combine.CanAddVerts(meshFilters[i]))
				{
					combine.AddVerts(meshFilters[i]);
					foundMeshCombine = true;
					break;
				}
			}
			if(!foundMeshCombine)
				combines.Add(new MeshCombine(meshFilters[i]));

			GameObject.Destroy(meshFilters[i].gameObject);
			i++;
		}

		foreach (MeshCombine combine in combines)
		{
			GameObject newMeshPar = new GameObject("NewMeshPar");
			newMeshPar.AddComponent<MeshFilter>().mesh = new Mesh();
			newMeshPar.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.combines.ToArray());
			newMeshPar.AddComponent<MeshRenderer>().materials = combine.mats;
			newMeshPar.transform.SetParent(objectParent);
		}

	}

	private class MeshCombine
	{
		public Material[] mats;
		public List<CombineInstance> combines = new List<CombineInstance>();
		private int verts = 0;

		public bool CanAddVerts(MeshFilter mf)
		{
			return MatsEquivilant(mf.GetComponent<MeshRenderer>().sharedMaterials) && verts + mf.sharedMesh.vertices.Length < 60000;
		}

		private bool MatsEquivilant(Material[] newMats)
		{
			if (mats.Length != newMats.Length)
				return false;
			int i = 0;
			foreach (Material mat in mats)
			{
				if (mat != newMats[i])
					return false;
				i++;
			}
			return true;
		}

		public void AddVerts(MeshFilter mf)
		{
			CombineInstance i = new CombineInstance();
			i.mesh = mf.sharedMesh;
			i.transform = mf.transform.localToWorldMatrix;
			combines.Add(i);
			verts += i.mesh.vertexCount;
		}

		public MeshCombine(MeshFilter mf)
		{
			mats = mf.GetComponent<MeshRenderer>().sharedMaterials;
			AddVerts(mf);
		}
	}
}
