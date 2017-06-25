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

	public GameObject SettlementInfoPrefab;

	public ModelLookup ModelLookup;

	// Use this for initialization
	void Start ()
	{
		RebuildMap();
	}

	public void RebuildMap()
	{
		int width = 100;
		if(sizeX.text != "")
			width = int.Parse(sizeX.text);
		int height = 100;
		if(sizeY.text != "")
			height = int.Parse(sizeY.text);
		StartCoroutine(BuildMap(width, height));
	}

	public IEnumerator BuildMap(int width, int height)
	{
		terrainMeshDisplay.transform.localPosition = Vector3.zero;
		for (int i = 0; i < terrainMeshDisplay.transform.childCount; i++)
		{
			Destroy(terrainMeshDisplay.transform.GetChild(i).gameObject);
		}

		displayText.enabled = true;

		displayText.text = "Generating Height Map";
		yield return null;

		int averagePixelsPerRegion = 120;

		HeightMapGenerator heightGenerator = new HeightMapGenerator(width, height);
		
		displayText.text = "Generating Ground Types";
		yield return null;

		TerrainMapGenerator terrainMapGenerator = new TerrainMapGenerator(heightGenerator.GetHeightMap());
		
		displayText.text = "Generating Regions";
		yield return null;

		RegionsMapGenerator regionsMap = new RegionsMapGenerator(terrainMapGenerator, terrainMapGenerator.LandPixelCount() / averagePixelsPerRegion);

		displayText.text = "Building Terrain Mesh";
		yield return null;

		MeshBuilder meshBuilder = new MeshBuilder(terrainMapGenerator, heightGenerator.GetHeightMap());

		displayText.text = "Displaying Map";
		yield return null;

		generatedMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = heightGenerator.GetHeightMapTexture();
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = terrainMapGenerator.GetTerrainTexture();


		terrainMaterial.SetTexture("_LookupTex", terrainMapGenerator.GetTerrainTexture());
		terrainMaterial.SetFloat("_LookupWidth", terrainMapGenerator.GetTerrainTexture().width);
		terrainMaterial.SetFloat("_TexSize", 512);
		
		Texture2D regions = WriteRegionsMap(regionsMap, meshBuilder);
		regionsMaterial.mainTexture = regions;
		regionsMaterial.SetFloat("_LookupWidth", terrainMapGenerator.GetTerrainTexture().width);

		int meshNum = 0;
		foreach(Mesh m in meshBuilder.GetBuiltMeshes())
		{
			GameObject g = new GameObject("Mesh"+meshNum);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>();
			g.GetComponent<MeshRenderer>().materials = new Material[2] { terrainMaterial, regionsMaterial };
			if(m.vertices.Length > 1)
				g.AddComponent<MeshCollider>();
			meshNum++;
		}

		displayText.text = "Setting Up Settlements";
		yield return null;

		AddSettlementInfoPanels(regionsMap);

		displayText.text = "Done";
		yield return null;

		//terrainMeshDisplay.transform.localPosition -= new Vector3(width / 2.5f, 0f, height / 2.5f);

		displayText.enabled = false;
	}

	private Texture2D WriteRegionsMap(RegionsMapGenerator map, MeshBuilder meshBuilder)
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

	private void AddSettlementInfoPanels(RegionsMapGenerator regionsMap)
	{
		float tileWidth = .8f;
		foreach(Kingdom r in regionsMap.GetRegions())
		{
			if (r.settlement == null)
				continue;
			GameObject tag = GameObject.Instantiate(SettlementInfoPrefab);
			tag.transform.SetParent(terrainMeshDisplay.transform);
			Int2 placementPos = r.settlement.cityTiles[0];
			tag.transform.localPosition = new Vector3(placementPos.X * tileWidth, .5f, placementPos.Y * tileWidth);
			tag.GetComponent<SettlementInfoController>().settlement = r.settlement;
		}
	}
}
