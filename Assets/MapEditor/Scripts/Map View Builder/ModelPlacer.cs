using System.Collections;
using System.Collections.Generic;
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
			PlaceEnvironmentObjectsOnTile(tile, Map.Map.Get(tile).GetEntites());
		}
		CombineAllMeshes();
	}
	
	private void PlaceEnvironmentObjectsOnTile(Int2 tile, List<EntityPlacementModel> infos)
	{
		foreach (var info in infos)
		{
			if(info.model == null)
				Debug.Log("Hit!");
			PlaceModels(tile, info.Model(), info.Mode(), info.NumToPlace());
		}
	}

	private void PlaceModels(Int2 tile, GameObject obj, EntityPlacementModel.PlacementMode mode, int num)
	{
		if (mode == EntityPlacementModel.PlacementMode.Scattered)
			PlaceObjectsOnTile(tile, num, obj);
		if (mode == EntityPlacementModel.PlacementMode.Center)
			PlaceObjectsOnTileCenter(tile, num, obj, true);
		if (mode == EntityPlacementModel.PlacementMode.Rot0)
			PlaceObjectsOnTileCenterWithRot(tile, num, obj, 0, true);
		if (mode == EntityPlacementModel.PlacementMode.Rot90)
			PlaceObjectsOnTileCenterWithRot(tile, num, obj, 90, true);
		if (mode == EntityPlacementModel.PlacementMode.Rot180)
			PlaceObjectsOnTileCenterWithRot(tile, num, obj, 180, true);
		if (mode == EntityPlacementModel.PlacementMode.Rot270)
			PlaceObjectsOnTileCenterWithRot(tile, num, obj, 270, true);
		if (mode == EntityPlacementModel.PlacementMode.Bridge)
			PlaceBridgeOnTile(tile, obj);
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
				if (!forcePlacement && tex.GetPixel((int) (hit.textureCoord.x * tex.width), (int) (hit.textureCoord.y * tex.height)).a > .2f)
					return;

				pos = hit.point;
				rot = r;
				valid = true;
			}
		}
	}

	private void PlaceObjectsOnTile(Int2 tile, int num, GameObject objToPlace, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetRandomPlacementTrans(tile, objToPlace, forcePlacement), objToPlace);
	}

	private void PlaceObjectsOnTileCenter(Int2 tile, int num, GameObject objToPlace, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetCenterPlacementTrans(tile, objToPlace, forcePlacement), objToPlace);
	}

	private void PlaceObjectsOnTileCenterWithRot(Int2 tile, int num, GameObject objToPlace, float rot, bool forcePlacement = false)
	{
		for (int i = 0; i < num; i++)
			SpawnObjectAtPos(GetCenterPlacementTransWithRot(tile, objToPlace, rot, forcePlacement), objToPlace);
	}

	private PlacementTrans GetCenterPlacementTransWithRot(Int2 myTile, GameObject g, float rot, bool forcePlacement = false)
	{
		return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 0.5f), new Vector3(0, rot, 0), g, forcePlacement);
	}

	private PlacementTrans GetCenterPlacementTrans(Int2 myTile, GameObject g, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 3) * 90f, 0);
		return new PlacementTrans(new Vector3(myTile.X + 0.5f, 2f, myTile.Y + 0.5f), rot, g, forcePlacement);
	}

	private PlacementTrans GetRandomPlacementTrans(Int2 myTile, GameObject g, bool forcePlacement = false)
	{
		Vector3 rot = new Vector3(0, Random.Range(0, 360f), 0);
		return new PlacementTrans(new Vector3(Random.Range(myTile.X, myTile.X + 1f), 3f, Random.Range(myTile.Y, myTile.Y + 1f)), rot, g, forcePlacement);
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
