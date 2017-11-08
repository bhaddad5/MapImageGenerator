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
		StartCoroutine(BuildMap(width, height, RealmParser.RealmsData[EnvironmentSelection.options[EnvironmentSelection.value].text]));
	}

	public IEnumerator BuildMap(int width, int height, RealmModel realmCreationInfo)
	{
		MapModel Map = new MapModel(width, height);

		

		displayText.text = "Raising Mountains";
		yield return null;

		MapGenerator.SetUpMapGenerator(width, height, realmCreationInfo);
		
		displayText.text = "Forging Kingdoms";
		yield return null;

		RegionsGen regionsMap = new RegionsGen(realmCreationInfo.Cultures);

		displayText.text = "Artificing Lands";
		yield return null;

		MapMeshBuilder meshConstructor = new MapMeshBuilder();
		List<Mesh> mapMeshes = meshConstructor.BuildMapMeshes();

		displayText.text = "Presenting World";
		yield return null;

		generatedMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapGenerator.GetHeightMapTexture();
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapGenerator.GetTerrainTexture();

		List<Material> mapMats = GetMapMaterials(realmCreationInfo.groundTypes.Values.ToList());

		int meshNum = 0;
		foreach(Mesh m in mapMeshes)
		{
			GameObject g = new GameObject("Mesh"+meshNum);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>();
			g.GetComponent<MeshRenderer>().materials = mapMats.ToArray();
			if(m.vertices.Length > 1)
				g.AddComponent<MeshCollider>();
			meshNum++;
		}

		displayText.text = "Seeding Forests";
		yield return null;

		ModelPlacer mp = new ModelPlacer();
		mp.PlaceModels(objectParent.transform);

		displayText.text = "Displaying Heraldry";
		yield return null;

		AddSettlementInfoPanels(regionsMap);

		displayText.text = "Done";
		yield return null;

		transform.localPosition -= new Vector3(width / 2f, 0f, height / 2f);

		displayText.enabled = false;
	}

	private void DisplayMap(MapModel mapToDisplay)
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
		waterPlane.transform.localScale = new Vector3(mapToDisplay.Map.Width / 10, 1, mapToDisplay.Map.Height / 10);
		waterPlane.transform.parent = transform;
		waterPlane.transform.localPosition = new Vector3(mapToDisplay.Map.Width / 2, Globals.MinGroundHeight * 2f - 0.01f, mapToDisplay.Map.Height / 2);

		displayText.enabled = true;
	}


	private void AddSettlementInfoPanels(RegionsGen regionsMap)
	{
		float tileWidth = 1f;
		foreach(Kingdom r in regionsMap.GetKingdoms())
		{
			foreach(var sett in r.settlements)
			{
				GameObject tag = GameObject.Instantiate(SettlementInfoPrefab);
				tag.transform.SetParent(terrainMeshDisplay.transform);
				Int2 placementPos = sett.GetInfoPlacementPos();
				tag.transform.localPosition = new Vector3(placementPos.X * tileWidth, .5f, placementPos.Y * tileWidth);
				tag.GetComponent<SettlementInfoDisplay>().settlement = sett;
			}
		}
	}

	private List<Material> GetMapMaterials(List<TerrainInfo> groundTypes)
	{
		var mats = new List<Material>() {new Material(Shader.Find("Standard"))};
		
		mats[0].color = Color.black;
		mats[0].SetFloat("_Glossiness", 0f);

		List<TerrainInfo> gtToFlush = new List<TerrainInfo>();
		for (int i = 0; i < groundTypes.Count; i++)
		{
			gtToFlush.Add(groundTypes[i]);
			if (gtToFlush.Count >= 5)
			{
				mats.Add(FlushGroundInfoToMat(gtToFlush));
				gtToFlush.Clear();
			}
		}
		if(gtToFlush.Count > 0)
			mats.Add(FlushGroundInfoToMat(gtToFlush));

		return mats;
	}

	private Material FlushGroundInfoToMat(List<TerrainInfo> groundInfo)
	{
		var mat = new Material(Shader.Find("Custom/GroundShader"));

		mat.SetTexture("_LookupTex", MapGenerator.GetTerrainTexture());
		mat.SetFloat("_LookupWidth", MapGenerator.GetTerrainTexture().width);
		for (int i = 0; i < 5 && i < groundInfo.Count; i++)
		{
			mat.SetVector("_Color" + i, groundInfo[i].lookupColor);
			mat.SetTexture("_Tex" + i, groundInfo[i].texture);
		}
		return mat;
	}

	public void QuitApp()
	{
		Application.Quit();
	}
}
