using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MapBuilder : MonoBehaviour
{
	public InputField sizeX;
	public InputField sizeY;
	public Text displayText;

	public GameObject terrainMeshDisplay;
	public GameObject generatedMapInputDisplay;
	public GameObject generatedTerrainMapInputDisplay;

	public Material terrainMaterial;
	public Material regionsMaterial;

	public GameObject waterPlane;

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
			new CulturePrevelance(CultureDefinitions.Anglo, CulturePrevelance.Prevelance.Dominant),
			new CulturePrevelance(CultureDefinitions.Orc, CulturePrevelance.Prevelance.Occasional),
			new CulturePrevelance(CultureDefinitions.Dwarf, CulturePrevelance.Prevelance.Occasional),
		};

		List<Environment> environments = new List<Environment>()
		{
			new Environment("Midlands", new MidlandGenerator())
		};

		StartCoroutine(BuildMap(width, height, cultures, environments[0]));
	}

	public IEnumerator BuildMap(int width, int height, List<CulturePrevelance> cultures, Environment environment)
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

		MapGenerator mapGenerator = new MapGenerator(width, height, environment.HeightGenerator);
		
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

		generatedMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapGenerator.GetHeightMapTexture();
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = mapGenerator.GetTerrainTexture();


		terrainMaterial.SetTexture("_LookupTex", mapGenerator.GetTerrainTexture());
		terrainMaterial.SetFloat("_LookupWidth", mapGenerator.GetTerrainTexture().width);
		terrainMaterial.SetFloat("_TexSize", 512);
		
		Texture2D regions = WriteRegionsMap(regionsMap, meshBuilder);
		regionsMaterial.mainTexture = regions;
		regionsMaterial.SetFloat("_LookupWidth", mapGenerator.GetTerrainTexture().width);

		int meshNum = 0;
		foreach(Mesh m in meshBuilder.GetBuiltMeshes())
		{
			GameObject g = new GameObject("Mesh"+meshNum);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>();
			g.GetComponent<MeshRenderer>().materials = new Material[1] { terrainMaterial };
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

	private Texture2D WriteRegionsMap(RegionsGen map, MeshBuilder meshBuilder)
	{
		Texture2D mapOut = new Texture2D(map.Width, map.Height);
		mapOut.anisoLevel = 0;
		mapOut.filterMode = FilterMode.Point;
		mapOut.wrapMode = TextureWrapMode.Clamp;
		
		for (int i = 0; i < mapOut.width; i++)
		{
			for (int j = 0; j < mapOut.height; j++)
			{
				mapOut.SetPixel(i, j, map.GetTileColor(new Int2(i, j)));
			}
		}

		mapOut.Apply();

		return mapOut;
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

	public class Environment
	{
		public string displayName;
		public IMapGenerator HeightGenerator;

		public Environment(string name, IMapGenerator gen)
		{
			displayName = name;
			HeightGenerator = gen;
		}
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
