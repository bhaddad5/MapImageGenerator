using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VR.WSA.Persistence;

public class UnitController : MonoBehaviour
{
	public int numTroops;
	public GameObject troopPrefab;
	public GameObject troopMarkerPrefab;

	public float attack = 1f;
	public float defense = 1f;
	

	private List<TroopController> troops = new List<TroopController>();
	private List<GameObject> troopMarkers = new List<GameObject>();

	public int numColumns { get; set; }
	public float speed { get; set; }
	public float spacing { get; set; }

	void Start()
	{
		numColumns = 20;
		speed = 2f;
		spacing = 2f;

		int i = 0;
		while (i < numTroops)
		{
			troops.Add(Instantiate(troopPrefab, transform.position, transform.rotation).GetComponent<TroopController>());
			troopMarkers.Add(Instantiate(troopMarkerPrefab));
			troops[i].unit = this;
			i++;
		}
	}

	private Vector3 GetDesiredPos(int index)
	{
		if (numColumns == 0)
			numColumns = 1;

		int numTroopsInColumn = troops.Count / numColumns;
		int myColumn = index / numTroopsInColumn;
		int myLine = index % numTroopsInColumn;

		Vector3 offset = new Vector3(((numTroopsInColumn) / 2f) * spacing, 0, ((troops.Count % numTroopsInColumn) /2f) * spacing);

		Vector3 localOffset = new Vector3(myColumn * spacing - offset.x, 0, myLine * spacing - offset.z);

		Vector3 desiredPos = transform.position - Quaternion.Euler(transform.eulerAngles) * localOffset;

		if (!SceneGraph.PosIsPassable(desiredPos))
			desiredPos = SceneGraph.ClosestPassable(desiredPos);

		return SceneGraph.HeightAdjustedPos(desiredPos);
	}
	
	// Update is called once per frame
	void Update ()
	{
		int i = 0;
		foreach (TroopController troop in troops)
		{
			Vector3 pos = GetDesiredPos(i);
			troop.MoveTowardsDesiredPos(pos, transform.eulerAngles);
			troopMarkers[i].transform.position = pos;
			troopMarkers[i].transform.eulerAngles = transform.eulerAngles;

			i++;
		}
	}

	public void LoseTroop(TroopController troop)
	{
		troops.Remove(troop);
		troopMarkers.RemoveAt(0);
	}
}
