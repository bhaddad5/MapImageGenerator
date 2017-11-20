using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityStandardAssets.Water;

public class MapBuilder : MonoBehaviour
{
	public TMP_Dropdown EnvironmentSelection;
	public TextMeshProUGUI displayText;

	public GameObject TerrainMeshDisplay;

	public GameObject SettlementInfoPrefab;

	public Material TerrainTextMat;
	public Material OverlaysMat;
	public Material WaterMat;

	public GameObject Sphere;
	public static GameObject DebugSphere;

	private GameObject ObjectParent;
	private MapModel CurrentMap;

	// Use this for initialization
	void Start ()
	{
		DebugSphere = Sphere;

		WorldParser.LoadWorlds();
		RealmParser.LoadRealms();
		ModelsParser.LoadModels();
		CultureParser.LoadCultures();
		LocationParser.LoadLocations();
		TerrainParser.LoadTerrainTypes();
		OverlayParser.LoadOverlays();
		TextChunkParser.LoadTextChunks();
		SettlementTypeParser.ParseSettlementTypes();

		foreach (WorldModel environment in WorldParser.WorldData.Values)
		{
			EnvironmentSelection.options.Add(new TMP_Dropdown.OptionData(environment.Id));
		}
		EnvironmentSelection.value = 1;
	}

	public void SaveMap()
	{
		string json = CurrentMap.ToJson();
		Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "SavedMaps"));
		string path = Path.Combine(Application.streamingAssetsPath, "SavedMaps/tempSave.realm");
		if(File.Exists(path))
			File.Delete(path);
		StreamWriter sw = File.CreateText(path);
		sw.WriteLine(json);
		sw.Close();
	}

	public void LoadMap()
	{
		string path = Path.Combine(Application.streamingAssetsPath, "SavedMaps/tempSave.realm");
		if (File.Exists(path))
		{
			CurrentMap = MapModel.FromJson(File.ReadAllText(path));
			StartCoroutine(DisplayMap());
		}
	}

	public void RebuildMap()
	{
		WorldModel world = WorldParser.WorldData[EnvironmentSelection.options[EnvironmentSelection.value].text];
		CurrentMap = new MapModel(world.MapWidth, world.MapHeight);
		GenerateMap(world);
		StartCoroutine(DisplayMap());
	}

	public void GenerateMap(WorldModel WorldCreationInfo)
	{
		MapGeneratorApi generator = new MapGeneratorApi();
		generator.GenerateMap(CurrentMap, WorldCreationInfo);

		CulturesGenerator culturesGen = new CulturesGenerator();
		culturesGen.GenerateMap(CurrentMap, WorldCreationInfo);
	}

	private IEnumerator DisplayMap()
	{
		displayText.enabled = true;

		TerrainMeshDisplay.transform.localPosition = Vector3.zero;
		for (int i = 0; i < TerrainMeshDisplay.transform.childCount; i++)
		{
			Destroy(TerrainMeshDisplay.transform.GetChild(i).gameObject);
		}
		transform.localPosition = Vector3.zero;

		if (ObjectParent != null)
			Destroy(ObjectParent);
		ObjectParent = new GameObject("objectParent");
		ObjectParent.transform.SetParent(transform);

		displayText.text = "Artificing Lands";
		yield return null;
		
		int vertsPerTile = 5;
		Map2D<float> vertHeights = MapMeshBuilder.BuildVertHeights(CurrentMap, vertsPerTile);
		int overlayTexSize = 64;
		int terrainMatTexSize = 36;
		Map2D<Color> mapTex = new Map2D<Color>(CurrentMap.Map.Width * terrainMatTexSize, CurrentMap.Map.Height * terrainMatTexSize);
		yield return StartCoroutine(MapTextureHelpers.GetTerrainTexture(CurrentMap.Map, terrainMatTexSize, mapTex, displayText));
		OverlayDisplayHandler.OverlayTextures overlays = OverlayDisplayHandler.GetOverlayMats(CurrentMap.Map, overlayTexSize);

		int mapChunkSize = 20;
		Map2D<int> mapChunks = GetMapChunks(mapChunkSize);

		int currChunk = 0;
		foreach (Int2 mapChunk in mapChunks.GetMapPoints())
		{
			displayText.text = "Presenting World " + (currChunk/(float)mapChunks.Size * 100f) + "%";
			currChunk++;
			yield return null;

			Int2 MapScaleStartingPoint = mapChunk * mapChunkSize;
			int MapScaleWidth = Math.Min(mapChunkSize, CurrentMap.Map.Width - MapScaleStartingPoint.X);
			int MapScaleHeight = Math.Min(mapChunkSize, CurrentMap.Map.Height - MapScaleStartingPoint.Y);

			Mesh mapMesh = MeshConstructor.BuildMeshes(vertHeights.GetMapBlock(MapScaleStartingPoint * vertsPerTile, MapScaleWidth * vertsPerTile + 1, MapScaleHeight * vertsPerTile + 1), vertsPerTile);

			List<Material> mapMats = new List<Material>();
			TerrainTextMat.mainTexture = MapTextureHelpers.ColorMapToMaterial(mapTex.GetMapBlock(MapScaleStartingPoint * terrainMatTexSize, MapScaleWidth * terrainMatTexSize, MapScaleHeight * terrainMatTexSize));
			mapMats.Add(TerrainTextMat);
			OverlaysMat.mainTexture = MapTextureHelpers.ColorMapToMaterial(overlays.Overlays.GetMapBlock(MapScaleStartingPoint * overlayTexSize, MapScaleWidth * overlayTexSize, MapScaleHeight * overlayTexSize));
			mapMats.Add(OverlaysMat);
			WaterMat.SetTexture("_MaskTex", MapTextureHelpers.ColorMapToMaterial(overlays.Water.GetMapBlock(MapScaleStartingPoint * overlayTexSize, MapScaleWidth * overlayTexSize, MapScaleHeight * overlayTexSize)));
			mapMats.Add(WaterMat);

			GameObject g = new GameObject("Mesh");
			g.transform.SetParent(TerrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = mapMesh;
			g.AddComponent<MeshRenderer>();
			g.GetComponent<MeshRenderer>().materials = mapMats.ToArray();
			g.AddComponent<WaterBasic>();
			g.GetComponent<WaterBasic>().matNumber = mapMats.Count - 1;
			if (mapMesh.vertices.Length > 1)
				g.AddComponent<MeshCollider>();
			g.transform.localPosition += new Vector3(MapScaleStartingPoint.X, 0, MapScaleStartingPoint.Y);
		}

		displayText.text = "Seeding Forests";
		yield return null;

		ModelPlacer mp = new ModelPlacer();
		mp.PlaceModels(ObjectParent.transform, CurrentMap);

		displayText.text = "Displaying Heraldry";
		yield return null;

		AddSettlementInfoPanels(CurrentMap);

		displayText.text = "Done";
		yield return null;

		transform.localPosition -= new Vector3(CurrentMap.Map.Width / 2f, 0f, CurrentMap.Map.Height / 2f);

		displayText.enabled = false;
	}

	private Map2D<int> GetMapChunks(int mapChunkSize)
	{
		int mapChunkWidth = CurrentMap.Map.Width / mapChunkSize;
		if (CurrentMap.Map.Width % mapChunkSize > 0)
			mapChunkWidth++;
		int mapChunkHeight = CurrentMap.Map.Height / mapChunkSize;
		if (CurrentMap.Map.Height % mapChunkSize > 0)
			mapChunkHeight++;
		Map2D<int> mapChunks = new Map2D<int>(mapChunkWidth, mapChunkHeight);
		return mapChunks;
	}


	private void AddSettlementInfoPanels(MapModel Map)
	{
		foreach(KingdomModel r in Map.Kingdoms.Values)
		{
			/*foreach(var sett in r.settlements)
			{
				GameObject tag = GameObject.Instantiate(SettlementInfoPrefab);
				tag.transform.SetParent(terrainMeshDisplay.transform);
				Int2 placementPos = sett.GetInfoPlacementPos();
				tag.transform.localPosition = new Vector3(placementPos.X, .5f, placementPos.Y);
				tag.GetComponent<SettlementInfoDisplay>().settlement = sett;
			}*/
		}
	}

	public void QuitApp()
	{
		Application.Quit();
	}
}
