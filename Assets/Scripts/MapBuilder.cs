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

		displayText.text = "Building Ground Texture";
		yield return null;

		MapTextureGenerator textureGenerator = new MapTextureGenerator(terrainMap, lookup);

		displayText.text = "Displaying Map";
		yield return null;

		int meshNum = 0;
		foreach(Mesh m in meshBuilder.GetBuiltMeshes())
		{
			GameObject g = new GameObject("Mesh"+meshNum);
			g.transform.SetParent(terrainMeshDisplay.transform);
			g.AddComponent<MeshFilter>().mesh = m;
			g.AddComponent<MeshRenderer>().material = terrainMeshDisplay.GetComponent<MeshRenderer>().material;
			g.GetComponent<MeshRenderer>().material.mainTexture = textureGenerator.GetMapTexture();
			meshNum++;
		}


		terrainMeshDisplay.transform.localPosition -= new Vector3(width/10, 0f, height/10);
		//WriteRegionsMap(regionsMap, meshBuilder);

		displayText.enabled = false;

		Debug.Log("Done");
	}

	/*private void WriteRegionsMap(RegionsMapGenerator map, MeshBuilder meshBuilder)
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

		regionsMeshDisplay.GetComponent<MeshFilter>().mesh = meshBuilder.GetBuiltMesh();
		regionsMeshDisplay.transform.localScale = new Vector3(.3f, .06f, .3f);
		regionsMeshDisplay.transform.localPosition = new Vector3(5f, 1.01f, 5f);
		regionsMeshDisplay.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		regionsMeshDisplay.GetComponent<MeshRenderer>().material.mainTexture = mapOut;
	}*/
}
