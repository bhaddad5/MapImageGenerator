using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapBuilder : MonoBehaviour
{
	public Texture2D mapIn;

	//DEBUG
	public Material outMatToSet;
	public MeshFilter outMeshToSet;

	// Use this for initialization
	void Start () {
		int averagePixelsPerRegion = 60;

		StoredTerrainMap terrainMap = new StoredTerrainMap(mapIn);

		StoredRegionsMap regionsMap = new StoredRegionsMap(terrainMap, (mapIn.width * mapIn.height) / averagePixelsPerRegion);

		MeshBuilder meshBuilder = new MeshBuilder(terrainMap);

		outMeshToSet.mesh = meshBuilder.GetBuiltMesh();
		outMeshToSet.transform.localScale = new Vector3(10, 1, 10);
		outMeshToSet.transform.localPosition = new Vector3(5f, 1, 5f);
		outMeshToSet.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

		WriteRegionsMapToPng(regionsMap);

		Debug.Log("Done");
	}

	private void WriteRegionsMapToPng(StoredRegionsMap map)
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

		outMatToSet.mainTexture = mapOut;

		byte[] bytes = mapOut.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/MapTextures/SavedMap.png", bytes);
	}
}
