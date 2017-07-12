using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
	public int numTroops;
	public GameObject troopPrefab;

	private List<TroopController> troops = new List<TroopController>();

	private int numLines = 3;
	private float spacing = 0.2f;

	void Start()
	{
		int i = 0;
		while (i < numTroops)
		{
			troops.Add(Instantiate(troopPrefab).GetComponent<TroopController>());
			troops[i].unit = this;
			i++;
		}
	}

	private Vector3 GetDesiredPos(int index)
	{
		int numTroopsInLine = troops.Count / numLines;
		int myLine = index / numTroopsInLine;
		int myColumn = index % numTroopsInLine;

		Vector3 offset = new Vector3((numTroopsInLine/2f) * spacing, 0, ((troops.Count%numTroopsInLine)/2f) * spacing);

		Vector3 localOffset = new Vector3(myColumn * spacing - offset.x, 0, myLine * spacing - offset.z);

		return transform.position - Quaternion.Euler(transform.eulerAngles) * localOffset;
	}
	
	// Update is called once per frame
	void Update ()
	{
		int i = 0;
		foreach (TroopController troop in troops)
		{
			troop.MoveTowardsDesiredPos(GetDesiredPos(i), transform.eulerAngles);

			i++;
		}
	}
}
