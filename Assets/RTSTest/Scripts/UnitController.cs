using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
	public int numTroops;
	public GameObject troopPrefab;
	public GameObject troopMarkerPrefab;

	public float attack = 1f;
	public float defense = 1f;
	public float speed = 2f;
	public float spacing = 2f;


	private List<TroopController> troops = new List<TroopController>();
	private List<GameObject> troopMarkers = new List<GameObject>();

	public float desiredWidth { get; set; }
	private Vector3 desiredPos;
	private Vector3 desiredRot;
	private UnitController targetUnit = null;

	public float currMoveSpeed { get; set; }

	void Start()
	{
		desiredPos = transform.position;

		desiredWidth = 10;

		int i = 0;
		while (i < numTroops)
		{
			troops.Add(Instantiate(troopPrefab, transform.position, transform.rotation).GetComponent<TroopController>());
			troopMarkers.Add(Instantiate(troopMarkerPrefab));
			troops[i].unit = this;
			i++;
		}
	}

	private Vector3 GetDesiredPos(int index, Vector3 unitPos, Vector3 unitRot)
	{
		int numColumns = Mathf.Max(4, (int)(desiredWidth / spacing));

		int numTroopsInColumn = troops.Count / numColumns;
		if (numTroopsInColumn == 0)
			numTroopsInColumn = troops.Count;
		int myColumn = index / numTroopsInColumn;
		int myLine = index % numTroopsInColumn;

		Vector3 offset = new Vector3(((numTroopsInColumn) / 2f) * spacing, 0, ((troops.Count % numTroopsInColumn) /2f) * spacing);

		Vector3 localOffset = new Vector3(myColumn * spacing - offset.x, 0, myLine * spacing - offset.z);

		Vector3 desiredPos = unitPos - Quaternion.Euler(unitRot) * localOffset;

		if (!SceneGraph.PosIsPassable(desiredPos))
			desiredPos = SceneGraph.ClosestPassable(desiredPos);

		return SceneGraph.HeightAdjustedPos(desiredPos);
	}

	public void SetDesiredUnitPos(Vector3 givenPos, Vector3 givenRot, bool target = false)
	{
		desiredPos = SceneGraph.HeightAdjustedPos(SceneGraph.ClosestPassable(givenPos));
		desiredRot = givenRot;

		int i = 0;
		foreach (GameObject marker in troopMarkers)
		{
			Vector3 pos = GetDesiredPos(i, desiredPos, givenRot);
			troopMarkers[i].transform.position = pos;
			troopMarkers[i].transform.eulerAngles = givenRot;

			i++;
		}

		if (!target)
		{
			targetUnit = null;
		}
	}

	public void SetTarget(UnitController enemyUnit)
	{
		targetUnit = enemyUnit;
	}

	public bool HasTarget()
	{
		return targetUnit != null;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (targetUnit != null)
		{
			SetDesiredUnitPos(targetUnit.transform.position, transform.eulerAngles, true);
		}

		

		Vector3 startPos = transform.position;
		transform.position = Vector3.MoveTowards(transform.position, desiredPos, speed);
		transform.LookAt(desiredPos);
		currMoveSpeed = startPos.FromTo(transform.position).magnitude;
		if (currMoveSpeed < 0.001f)
		{
			transform.eulerAngles = desiredRot;
		}

		int i = 0;
		foreach (TroopController troop in troops)
		{
			Vector3 pos = GetDesiredPos(i, transform.position, transform.eulerAngles);

			troop.MoveTowardsDesiredPos(pos, transform.eulerAngles);
			i++;
		}
	}

	public void LoseTroop(TroopController troop)
	{
		troops.Remove(troop);
	}
}
