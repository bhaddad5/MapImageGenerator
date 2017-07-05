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
	public GameObject waterPlane;
	public GameObject generatedMapInputDisplay;
	public GameObject generatedTerrainMapInputDisplay;

	public GameObject SettlementInfoPrefab;

	public ModelLookup ModelLookup;
	public GroundTypes GroundTypes;

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

		List<Environment> environments = new List<Environment>()
		{
			new Environment("The Midlands", new MidlandGenerator()),
			new Environment("The Under Realms", new UndergroundGenerator()),
		};

		StartCoroutine(BuildMap(width, height, cultures, environments[1]));
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

		generatedMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapGenerator.GetHeightMapTexture();
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = MapGenerator.GetTerrainTexture(GroundTypes);

		List<Material> mapMats = GetMapMaterials();

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

	private List<Material> GetMapMaterials()
	{
		List<GroundTypes.Type> groundTypesUsed = new List<GroundTypes.Type>();
		foreach (GroundTypes.Type type in MapGenerator.Terrain.GetMapValues())
		{
			if(!groundTypesUsed.Contains(type))
				groundTypesUsed.Add(type);
		}

		List<Material> matsToUse = new List<Material>();
		List<GroundTypes.GroundDisplayInfo> groundInfo = new List<GroundTypes.GroundDisplayInfo>();
		//TODO: CAN ONLY SUPPORT 10 TEXTURES AT A TIME!!!
		foreach (GroundTypes.Type type in groundTypesUsed)
		{
			groundInfo.Add(GroundTypes.GetDisplayInfo(type));
			if (groundInfo.Count >= 10)
			{
				matsToUse.Add(FlushGroundInfoToMat(groundInfo));
				break;
			}
		}
		if (groundInfo.Count > 0)
			matsToUse.Add(FlushGroundInfoToMat(groundInfo));
		return matsToUse;
	}

	private Material FlushGroundInfoToMat(List<GroundTypes.GroundDisplayInfo> groundInfo)
	{
		Material mat = new Material(Shader.Find("Custom/GroundShader"));
		mat.SetTexture("_LookupTex", MapGenerator.GetTerrainTexture(GroundTypes));
		mat.SetFloat("_LookupWidth", MapGenerator.GetTerrainTexture(GroundTypes).width);
		for (int i = 0; i < 10 && i < groundInfo.Count; i++)
		{
			mat.SetVector("_Color" + i, groundInfo[i].lookupColor);
			mat.SetTexture("_Tex" + i, groundInfo[i].texture);
		}
		return mat;
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
