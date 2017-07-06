using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class MapBuilder : MonoBehaviour
{
	public InputField sizeX;
	public InputField sizeY;
	public Text displayText;

	public GameObject terrainMeshDisplay;
	public GameObject waterPlane;
	public GameObject generatedMapInputDisplay;
	public GameObject generatedTerrainMapInputDisplay;

	public GameObject SettlementInfoPrefab;

	public ModelLookup ModelLookup;

	// Use this for initialization
	void Start ()
	{
		RebuildMap();
	}

	public void RebuildMap()
	{
		int width = 80;
		if(sizeX.text != "")
			width = int.Parse(sizeX.text);
		int height = 80;
		if(sizeY.text != "")
			height = int.Parse(sizeY.text);

		List<CulturePrevelance> cultures = new List<CulturePrevelance>()
		{
			//new CulturePrevelance(CultureDefinitions.Anglo, CulturePrevelance.Prevelance.Dominant),
			new CulturePrevelance(CultureDefinitions.Orc, CulturePrevelance.Prevelance.Occasional),
			new CulturePrevelance(CultureDefinitions.Dwarf, CulturePrevelance.Prevelance.Dominant),
		};

		var environments = EnvironmentParser.LoadEnvironments();

		StartCoroutine(BuildMap(width, height, cultures, environments[0]));
	}

	public IEnumerator BuildMap(int width, int height, List<CulturePrevelance> cultures, MapEnvironment mapEnvironment)
	{
		terrainMeshDisplay.transform.localPosition = Vector3.zero;
		for (int i = 0; i < terrainMeshDisplay.transform.childCount; i++)
		{
			Destroy(terrainMeshDisplay.transform.GetChild(i).gameObject);
		}
		transform.localPosition = Vector3.zero;

		waterPlane.SetActive(true);
		waterPlane.transform.localScale = new Vector3(width / 10, 1, height / 10);
		waterPlane.transform.parent = transform;
		waterPlane.transform.localPosition = new Vector3(width / 2, Globals.MinGroundHeight * 2f - 0.01f, height / 2);

		displayText.enabled = true;

		displayText.text = "Raising Mountains";
		yield return null;

		MapGenerator mapGenerator = new MapGenerator(width, height, mapEnvironment.HeightGenerator, mapEnvironment);
		
		displayText.text = "Forging Kingdoms";
		yield return null;

		int landPixelCount = mapGenerator.LandPixelCount();
		foreach (var culture in cultures)
			culture.SetNumSettlements(landPixelCount);

		RegionsGen regionsMap = new RegionsGen(cultures);

		displayText.text = "Artificing Lands";
		yield return null;

		MeshBuilder meshBuilder = new MeshBuilder();

		displayText.text = "Presenting World";
		yield return null;

		generatedMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapGenerator.GetHeightMapTexture();
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapGenerator.GetTerrainTexture();

		List<Material> mapMats = GetMapMaterials(mapEnvironment.groundTypes.Values.ToList());

		int meshNum = 0;
		foreach(Mesh m in meshBuilder.GetBuiltMeshes())
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
		mp.PlaceModels(ModelLookup, terrainMeshDisplay.transform);

		displayText.text = "Displaying Heraldry";
		yield return null;

		AddSettlementInfoPanels(regionsMap);

		displayText.text = "Done";
		yield return null;

		transform.localPosition -= new Vector3(width / 2f, 0f, height / 2f);

		displayText.enabled = false;
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
				tag.GetComponent<SettlementInfoController>().settlement = sett;
			}
		}
	}

	private List<Material> GetMapMaterials(List<GroundDisplayInfo> groundTypes)
	{
		var mats = new List<Material>() {FlushGroundInfoToMat(groundTypes)};
		return mats;
	}

	private Material FlushGroundInfoToMat(List<GroundDisplayInfo> groundInfo)
	{
		Material mat = new Material(Shader.Find("Custom/GroundShader"));
		mat.SetTexture("_LookupTex", MapGenerator.GetTerrainTexture());
		mat.SetFloat("_LookupWidth", MapGenerator.GetTerrainTexture().width);
		for (int i = 0; i < 10 && i < groundInfo.Count; i++)
		{
			mat.SetVector("_Color" + i, groundInfo[i].lookupColor);
			mat.SetTexture("_Tex" + i, groundInfo[i].texture);
		}
		return mat;
	}

	

	public class CulturePrevelance
	{
		public enum Prevelance
		{
			SingleColony,
			Scattered,
			Occasional,
			Dominant
		}

		public Culture culture;
		public int numSettlements;
		Prevelance prevelance;

		public CulturePrevelance(Culture c, Prevelance p)
		{
			culture = c;
			prevelance = p;
		}

		public void SetNumSettlements(int landPixleCount)
		{
			if (prevelance == Prevelance.SingleColony)
				numSettlements = 1;
			if (prevelance == Prevelance.Scattered)
			{
				numSettlements = landPixleCount / 380;
			}
			if (prevelance == Prevelance.Occasional)
			{
				numSettlements = landPixleCount / 240;
			}
			if (prevelance == Prevelance.Dominant)
			{
				numSettlements = landPixleCount / 180;
			}
		}
	}
}
