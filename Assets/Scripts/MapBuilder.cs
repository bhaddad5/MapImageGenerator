using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapBuilder : MonoBehaviour
{
	public Texture2D mapIn;
	public MapTextureLookup lookup;

	public GameObject terrainMeshDisplay;
	public GameObject regionsMeshDisplay;

	// Use this for initialization
	void Start () {
		int averagePixelsPerRegion = 60;

		StoredTerrainMap terrainMap = new StoredTerrainMap(mapIn);

		StoredRegionsMap regionsMap = new StoredRegionsMap(terrainMap, (mapIn.width * mapIn.height) / averagePixelsPerRegion);

		MeshBuilder meshBuilder = new MeshBuilder(terrainMap);

		MapTextureGenerator textureGenerator = new MapTextureGenerator(terrainMap, lookup);

		terrainMeshDisplay.GetComponent<MeshFilter>().mesh = meshBuilder.GetBuiltMesh();
		terrainMeshDisplay.transform.localScale = new Vector3(.3f, .06f, .3f);
		terrainMeshDisplay.transform.localPosition = new Vector3(5f, 1f, 5f);
		terrainMeshDisplay.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		terrainMeshDisplay.GetComponent<MeshRenderer>().material.mainTexture = textureGenerator.GetMapTexture();

		WriteRegionsMap(regionsMap, meshBuilder);

		Debug.Log("Done");
	}

	private void WriteRegionsMap(StoredRegionsMap map, MeshBuilder meshBuilder)
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
