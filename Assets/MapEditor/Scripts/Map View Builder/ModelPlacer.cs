using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class ModelPlacer
{
	Transform objectParent;
	private MapModel Map;

	public void PlaceModels(Transform par, MapModel map)
	{
		Map = map; 
		objectParent = par;
		foreach (Int2 tile in Map.Map.GetMapPoints())
		{
			PlaceEnvironmentObjectsOnTile(tile, Map.Map.Get(tile).Terrain().EntityPlacements.ToList());
		}
		foreach (Int2 tile in Map.Map.GetMapPoints())
		{
			//PlaceCultureObjectsOnTile(tile, Map.Kingdoms[Map.Map.Get(tile).KingdomId].Culture, Map.Map.Get(tile).Terrain.Traits);
		}
		CombineAllMeshes();
	}
	
	private void PlaceEnvironmentObjectsOnTile(Int2 tile, List<EntityPlacementModel> infos)
	{
		foreach (var info in infos)
		{
			PlaceModels(tile, info.Model, info.Mode, info.NumToPlace);
		}
	}

	private void PlaceModels(Int2 tile, GameObject obj, EntityPlacementModel.PlacementMode mode, int num)
	{
		if (mode == EntityPlacementModel.PlacementMode.Scattered)
			PlaceObjectsOnTile(tile, num, obj);
		if (mode == EntityPlacementModel.PlacementMode.ScatteredBordered)
			PlaceObjectsOnTileWithBorder(tile, num, obj);
		if (mode == EntityPlacementModel.PlacementMode.Corners)
			PlaceTurretsOnCorners(tile, obj);
		if (mode == EntityPlacementModel.PlacementMode.CityWalls)
			PlaceWallsOnEdges(tile, obj);
		if (mode == EntityPlacementModel.PlacementMode.CityGates)
			PlaceGatesOnEdges(tile, obj);
		if (mode == EntityPlacementModel.PlacementMode.Bridge)
			PlaceBridgeOnTile(tile, obj);
	}

	/*private void PlaceCultureObjectsOnTile(Int2 tile, CultureModel culture, List<string> traits)
	{
		foreach (string trait in traits)
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
	}*/

	private void PlaceWallsOnEdges(Int2 tile, GameObject wall)
	{
		foreach (Int2 pt in Map.Map.GetAdjacentPoints(tile))
		{
			if (TileIsCityBorder(pt) && !TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, wall, true), wall);
			}
		}
	}

	private void PlaceGatesOnEdges(Int2 tile, GameObject gate)
	{
		foreach (Int2 pt in Map.Map.GetAdjacentPoints(tile))
		{
			if (TileIsRoad(pt))
			{
				SpawnObjectAtPos(GetEdgePlacementTrans(tile, pt, gate, true), gate);
			}
		}
	}

	private bool TileIsRoad(Int2 tile)
	{
		return false;
		//return Map.Map.PosInBounds(tile) && Map.Map.Get(tile).Terrain().HasTrait(TerrainInfo.GroundTraits.Road);
	}

	private bool TileIsCityBorder(Int2 tile)
	{
		/*return Map.Map.PosInBounds(tile) &&
		       !Map.Map.Get(tile).Terrain().HasTrait(TerrainModel.GroundTraits.Impassable) &&
		       !Map.Map.Get(tile).Terrain().HasTrait(TerrainInfo.GroundTraits.City);*/
		return false;
	}

	private void PlaceTurretsOnCorners(Int2 tile, GameObject turret)
	{
		foreach (var t in Map.Map.GetDiagonalPoints(tile))
		{
			if (TileIsCityBorder(t) ||
			TileIsCityBorder(tile + new Int2(t.X - tile.X, 0)) ||
			TileIsCityBorder(tile + new Int2(0, t.Y - tile.Y)))
			{
				SpawnObjectAtPos(GetPlacementBetweenTileCenters(tile, t, turret, true), turret);
			}
		}
	}

	private void PlaceBridgeOnTile(Int2 tile, GameObject bridge)
	{
		if (Map.Map.Get(tile).HasTrait(MapTileModel.TileTraits.Water))
		{
			foreach (var t in Map.Map.GetAdjacentPoints(tile))
			{
				/*if (Map.Map.Get(t).HasTrait(TerrainInfo.GroundTraits.Road) ||
				    Map.Map.Get(t).HasTrait(TerrainInfo.GroundTraits.City))
				{
					if (MapTextureHelpers.Heights.Get(t) >= Globals.MinGroundHeight)
					{
						Vector3 rot = new Vector3();
						if (t.Y != tile.Y)
							rot = new Vector3(0, 90f, 0);
						SpawnObjectAtPos(GetCenterPlacementTrans(tile, rot, bridge, true), bridge);
						break;
					}
				}*/
			}
		}
	}





	public class PlacementTrans
	{
		public Vector3 pos;
		public Vector3 rot;
		public bool valid = false;

		public PlacementTrans(Vector3 p, Vector3 r, GameObject g, bool forcePlacement = false)
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

				var rend = hit.collider.gameObject.GetComponent<Renderer>();
				Texture2D tex = rend.materials[rend.materials.Length - 1].GetTexture("_MaskTex") as Texture2D;
				if (tex.GetPixel((int) (hit.textureCoord.x * tex.width), (int) (hit.textureCoord.y * tex.height)).a > .2f)
					return;

				if (!forcePlacement && !ClearDownwardRaycasts(g, p))
					return;

				pos = hit.point;
				rot = r;
				valid = true;
			}
		}

		private bool ClearDownwardRaycasts(GameObject g, Vector3 pos)
		{
			Bounds b = GetHierarchyBounds(g);
			RaycastHit hit;
			if (Physics.Raycast(new Ray(pos, Vector3.down), out hit))
			{
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
					return false;
			}
			if (Physics.Raycast(new Ray(pos + b.extents, Vector3.down), out hit))
			{
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
					return false;
			}
			if (Physics.Raycast(new Ray(pos - b.extents, Vector3.down), out hit))
			{
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
					return false;
			}
			if (Physics.Raycast(new Ray(pos + new Vector3(-b.extents.x, b.extents.y, b.extents.z), Vector3.down), out hit))
			{
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
					return false;
			}
			if (Physics.Raycast(new Ray(pos + new Vector3(b.extents.x, b.extents.y, -b.extents.z), Vector3.down), out hit))
			{
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
					return false;
			}

			return true;
		}

		private Bounds GetHierarchyBounds(GameObject g)
		{
			Bounds b = new Bounds();
			foreach (MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>())
			{
				b.Encapsulate(mr.bounds);
			}
			return b;
		}
	}

	private PlacementTrans GetPlacementBetweenTileCenters(Int2 tile1, Int2 tile2, GameObject g, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(tile1.X + ((tile2.X - tile1.X) / 2f) + .5f, 2f, tile1.Y + ((tile2.Y - tile1.Y) / 2f) + .5f), Vector3.zero, g, forcePlacement);
	}

	private void PlaceObjectsOnTile(Int2 tile, int num, GameObject objToPlace, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetRandomPlacementTrans(tile, objToPlace, forcePlacement), objToPlace);
	}

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, GameObject g, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 360f), 0);
		return GetRandomPlacementTrans(myTile, rot, g, forcePlacement);
	}

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, Vector3 rot, GameObject g, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(Random.Range(myTile.X, myTile.X + 1f), 3f, Random.Range(myTile.Y, myTile.Y + 1f)), rot, g, forcePlacement);
	}

	private void PlaceObjectsOnTileWithBorder(Int2 tile, int num, GameObject objToPlace, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetRandomPlacementTransWithBorder(tile, objToPlace, forcePlacement), objToPlace);
	}

	private PlacementTrans GetRandomPlacementTransWithBorder(Int2 myTile, GameObject g, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 360f), 0);
		return GetRandomPlacementTransWithBorder(myTile, rot, g, forcePlacement);
	}

	private PlacementTrans GetRandomPlacementTransWithBorder(Int2 myTile, Vector3 rot, GameObject g, bool forcePlacement = false)
	{
		float border = 0.15f;
		return new PlacementTrans(new Vector3(Random.Range(myTile.X + border, myTile.X + 1f - border), 3f, Random.Range(myTile.Y + border, myTile.Y + 1f - border)), rot, g, forcePlacement);
	}

	private PlacementTrans GetEdgePlacementTrans(Int2 myTile, Int2 edgeTile, GameObject g, bool forcePlacement = false)
	{
		if ((edgeTile - myTile).Equals(new Int2(0, -1)))
			return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y), new Vector3(0, -90f, 0), g, forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(0, 1)))
			return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 1), new Vector3(0, 90f, 0), g, forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(1, 0)))
			return new PlacementTrans(new Vector3(myTile.X + 1, 2f, myTile.Y + .5f), new Vector3(0, 180, 0), g, forcePlacement);
		if ((edgeTile - myTile).Equals(new Int2(-1, 0)))
			return new PlacementTrans(new Vector3(myTile.X, 2f, myTile.Y + .5f), new Vector3(0, 0, 0), g, forcePlacement);
		return null;
	}

	private PlacementTrans GetCenterPlacementTrans(Int2 myTile, Vector3 rot, GameObject g, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 0.5f), rot, g, forcePlacement);
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
