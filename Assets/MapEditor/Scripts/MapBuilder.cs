using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

public class MapBuilder : MonoBehaviour
{
	public Dropdown EnvironmentSelection;
	public Text displayText;

	public GameObject terrainMeshDisplay;
	public GameObject waterPlane;
	public GameObject generatedMapInputDisplay;
	public GameObject generatedTerrainMapInputDisplay;

	public GameObject SettlementInfoPrefab;
	public GameObject LocationInfoPrefab;

	private GameObject objectParent;

	// Use this for initialization
	void Start ()
	{
		RealmParser.LoadRealms();
		ModelsParser.LoadModels();
		CultureParser.LoadCultures();
		LocationParser.LoadLocations();
		TerrainParser.LoadTerrainTypes();

		foreach (RealmModel environment in RealmParser.RealmsData.Values)
		{
			EnvironmentSelection.options.Add(new Dropdown.OptionData(environment.DisplayName));
		}
		EnvironmentSelection.value = 1;
	}

	public void RebuildMap()
	{
		int width = 80;
		int height = 80;
		MapModel NewMap = GenerateMap(width, height, RealmParser.RealmsData[EnvironmentSelection.options[EnvironmentSelection.value].text]);
		StartCoroutine(DisplayMap(NewMap));
	}

	public MapModel GenerateMap(int width, int height, RealmModel realmCreationInfo)
	{
		displayText.enabled = true;

		MapGeneratorApi generator = new MapGeneratorApi();
		MapModel Map = generator.GenerateMap(width, height, realmCreationInfo);
		
		RegionsGenerator regionsGen = new RegionsGenerator();
		regionsGen.GenerateRegions(Map, realmCreationInfo);

		return Map;
	}

	private IEnumerator DisplayMap(MapModel Map)
	{
		terrainMeshDisplay.transform.localPosition = Vector3.zero;
		for (int i = 0; i < terrainMeshDisplay.transform.childCount; i++)
		{
			Destroy(terrainMeshDisplay.transform.GetChild(i).gameObject);
		}
		transform.localPosition = Vector3.zero;

		if (objectParent != null)
			Destroy(objectParent);
		objectParent = new GameObject("objectParent");
		objectParent.transform.SetParent(transform);

		waterPlane.SetActive(true);
		waterPlane.transform.localScale = new Vector3(Map.Map.Width / 10, 1, Map.Map.Height / 10);
		waterPlane.transform.parent = transform;
		waterPlane.transform.localPosition = new Vector3(Map.Map.Width / 2, Globals.MinGroundHeight * 2f - 0.01f, Map.Map.Height / 2);

		displayText.text = "Artificing Lands";
		yield return null;

		MapMeshBuilder meshConstructor = new MapMeshBuilder();
		List<Mesh> mapMeshes = meshConstructor.BuildMapMeshes(Map);

		displayText.text = "Presenting World";
		yield return null;

		generatedMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapTextureHelpers.GetHeightMapTexture(Map);
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapTextureHelpers.GetTerrainTexture(Map);

		List<Material> mapMats = GetMapMaterials(TerrainParser.TerrainData.Values.ToList(), Map);

		int meshNum = 0;
		foreach (Mesh m in mapMeshes)
		{
			GameObject g = new GameObject("Mesh" + meshNum);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>();
			g.GetComponent<MeshRenderer>().materials = mapMats.ToArray();
			if (m.vertices.Length > 1)
				g.AddComponent<MeshCollider>();
			meshNum++;
		}

		displayText.text = "Seeding Forests";
		yield return null;

		ModelPlacer mp = new ModelPlacer();
		mp.PlaceModels(objectParent.transform, Map);

		displayText.text = "Displaying Heraldry";
		yield return null;

		AddSettlementInfoPanels(Map);

		displayText.text = "Done";
		yield return null;

		transform.localPosition -= new Vector3(Map.Map.Width / 2f, 0f, Map.Map.Height / 2f);

		displayText.enabled = false;
	}


	private void AddSettlementInfoPanels(MapModel Map)
	{
		float tileWidth = 1f;
		foreach(KingdomModel r in Map.Kingdoms.Values)
		{
			/*foreach(var sett in r.settlements)
			{
				GameObject tag = GameObject.Instantiate(SettlementInfoPrefab);
				tag.transform.SetParent(terrainMeshDisplay.transform);
				Int2 placementPos = sett.GetInfoPlacementPos();
				tag.transform.localPosition = new Vector3(placementPos.X * tileWidth, .5f, placementPos.Y * tileWidth);
				tag.GetComponent<SettlementInfoDisplay>().settlement = sett;
			}*/
		}
	}

	private List<Material> GetMapMaterials(List<TerrainModel> groundTypes, MapModel Map)
	{
		var mats = new List<Material>() {new Material(Shader.Find("Standard"))};
		
		mats[0].color = Color.black;
		mats[0].SetFloat("_Glossiness", 0f);

		List<TerrainModel> gtToFlush = new List<TerrainModel>();
		for (int i = 0; i < groundTypes.Count; i++)
		{
			gtToFlush.Add(groundTypes[i]);
			if (gtToFlush.Count >= 5)
			{
				mats.Add(FlushGroundInfoToMat(gtToFlush, Map));
				gtToFlush.Clear();
			}
		}
		if(gtToFlush.Count > 0)
			mats.Add(FlushGroundInfoToMat(gtToFlush, Map));

		return mats;
	}

	private Material FlushGroundInfoToMat(List<TerrainModel> groundInfo, MapModel Map)
	{
		var mat = new Material(Shader.Find("Custom/GroundShader"));

		mat.SetTexture("_LookupTex", MapTextureHelpers.GetTerrainTexture(Map));
		mat.SetFloat("_LookupWidth", MapTextureHelpers.GetTerrainTexture(Map).width);
		for (int i = 0; i < 5 && i < groundInfo.Count; i++)
		{
			mat.SetVector("_Color" + i, groundInfo[i].LookupColor);
			mat.SetTexture("_Tex" + i, groundInfo[i].GetTerrainTexture());
		}
		return mat;
	}

	public void QuitApp()
	{
		Application.Quit();
	}
}
