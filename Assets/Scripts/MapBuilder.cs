using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MapBuilder : MonoBehaviour
{
	public MapTextureLookup lookup;
	public InputField sizeX;
	public InputField sizeY;
	public Text displayText;

	public GameObject generatedMapInputDisplay;
	public GameObject terrainMeshDisplay;
	public GameObject regionsMeshDisplay;
	public GameObject generatedTerrainMapInputDisplay;

	// Use this for initialization
	void Start ()
	{
		RebuildMap();
	}

	public void RebuildMap()
	{
		int width = 36;
		if(sizeX.text != "")
			width = int.Parse(sizeX.text);
		int height = 36;
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

		int averagePixelsPerRegion = 40;

		HeightMapGenerator heightGenerator = new HeightMapGenerator(width, height);
		generatedMapInputDisplay.GetComponent<MeshRenderer>().material.mainTexture = heightGenerator.GetHeightMapTexture();

		displayText.text = "Generating Ground Types";
		yield return null;

		TerrainMapGenerator terrainMapGenerator = new TerrainMapGenerator(heightGenerator.GetHeightMap());
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().material.mainTexture = terrainMapGenerator.GetTerrainTexture();

		StoredTerrainMap terrainMap = new StoredTerrainMap(terrainMapGenerator.GetTerrainMap());

		displayText.text = "Generating Regions";
		yield return null;

		RegionsMapGenerator regionsMap = new RegionsMapGenerator(terrainMap, terrainMap.LandPixelCount() / averagePixelsPerRegion);

		displayText.text = "Building Terrain Mesh";
		yield return null;

		MeshBuilder meshBuilder = new MeshBuilder(terrainMap, heightGenerator.GetHeightMap());

		displayText.text = "Displaying Map";
		yield return null;

		terrainMeshDisplay.GetComponent<MeshRenderer>().material.SetTexture("_LookupTex", terrainMapGenerator.GetTerrainTexture());
		terrainMeshDisplay.GetComponent<MeshRenderer>().material.SetFloat("_LookupWidth", terrainMapGenerator.GetTerrainTexture().width);
		terrainMeshDisplay.GetComponent<MeshRenderer>().material.SetFloat("_TexSize", lookup.Fertile.width);

		int meshNum = 0;
		foreach(Mesh m in meshBuilder.GetBuiltMeshes())
		{
			GameObject g = new GameObject("Mesh"+meshNum);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>().material = terrainMeshDisplay.GetComponent<MeshRenderer>().material;
			meshNum++;
		}

		Texture2D regions = WriteRegionsMap(regionsMap, meshBuilder);

		int meshNum2 = 0;
		foreach (Mesh m in meshBuilder.GetBuiltMeshes())
		{
			GameObject g = new GameObject("Mesh" + meshNum2);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>().sharedMaterial = regionsMeshDisplay.GetComponent<MeshRenderer>().material;
			g.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = regions;
			g.transform.localPosition += new Vector3(0f, 0.01f, 0f);
			meshNum2++;
		}

		terrainMeshDisplay.transform.localPosition -= new Vector3(width / 10, 0f, height / 10);

		displayText.enabled = false;

		Debug.Log("Done");
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
}
