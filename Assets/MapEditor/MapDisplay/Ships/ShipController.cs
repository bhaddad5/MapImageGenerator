using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private MapModel Map;

	private List<Int2> currentCourse = new List<Int2>();

	public void SetupShipController(Int2 startPos, MapModel map)
	{
		Map = map;
		currentCourse.Add(startPos);
		transform.position = new Vector3(startPos.X + 0.5f, 0, startPos.Y + 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		Int2 currentTile = new Int2((int)(transform.position.x - 0.5f), (int)(transform.position.z - 0.5f));

		if (currentTile.Equals(currentCourse.Last()))
		{
			currentCourse = Map.Map.Get(currentTile).Port.GetSeaLane();
		}
		else
		{
			Int2 nextTile = currentTile;
			for(int i = 0; i < currentCourse.Count; i++)
			{
				if (currentCourse[i].Equals(currentTile))
					nextTile = currentCourse[i + 1];
			}
			Vector3 target = new Vector3(nextTile.X + 0.5f, 0, nextTile.Y + 0.5f);
			transform.position = Vector3.MoveTowards(transform.position, target, 0.05f);
			transform.LookAt(target);
		}
	}
}
