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
		BuildMap(width, height);
	}

	public void BuildMap(int width, int height)
	{
		int averagePixelsPerRegion = 40;

		HeightMapGenerator heightGenerator = new HeightMapGenerator(width, height);
		generatedMapInputDisplay.GetComponent<MeshRenderer>().material.mainTexture = heightGenerator.GetHeightMapTexture();

		TerrainMapGenerator terrainMapGenerator = new TerrainMapGenerator(heightGenerator.GetHeightMap());
		generatedTerrainMapInputDisplay.GetComponent<MeshRenderer>().material.mainTexture = terrainMapGenerator.GetTerrainTexture();

		StoredTerrainMap terrainMap = new StoredTerrainMap(terrainMapGenerator.GetTerrainMap());

		RegionsMapGenerator regionsMap = new RegionsMapGenerator(terrainMap, terrainMap.LandPixelCount() / averagePixelsPerRegion);

		MeshBuilder meshBuilder = new MeshBuilder(terrainMap, heightGenerator.GetHeightMap());

		MapTextureGenerator textureGenerator = new MapTextureGenerator(terrainMap, lookup);

		terrainMeshDisplay.GetComponent<MeshFilter>().mesh = meshBuilder.GetBuiltMesh();
		terrainMeshDisplay.transform.localScale = new Vector3(.3f, .06f, .3f);
		terrainMeshDisplay.transform.localPosition = new Vector3(5f, 1f, 5f);
		terrainMeshDisplay.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		terrainMeshDisplay.GetComponent<MeshRenderer>().material.mainTexture = textureGenerator.GetMapTexture();

		WriteRegionsMap(regionsMap, meshBuilder);

		Debug.Log("Done");
	}

	private void WriteRegionsMap(RegionsMapGenerator map, MeshBuilder meshBuilder)
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
	}
}
