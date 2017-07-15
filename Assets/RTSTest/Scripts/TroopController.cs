using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopController : MonoBehaviour
{
	private float speed = .2f;
	public UnitController unit { get; set; }

	private float radius = 0.01f;

	public void MoveTowardsDesiredPos(Vector3 pos, Vector3 rot)
	{
		if (transform.position.FromTo(pos).magnitude < speed)
		{
			transform.position = SceneGraph.HeightAdjustedPos(pos);
			transform.eulerAngles = rot;
		}
		else
		{
			transform.position = SceneGraph.HeightAdjustedPos(TestGetNewPos(pos));
			transform.LookAt(pos);
		}
		
	}

	private Vector3 TestGetNewPos(Vector3 targetPos)
	{
		Vector3 desiredDir = transform.position.FromTo(targetPos);
		for (int i = 0; i < 8; i = i + 1)
		{
			Vector3 newDir = Quaternion.Euler(0, i * 45, 0) * desiredDir;
			Vector3 newPos = GetNewPos(newDir);

			int layerMask = (1 << LayerMask.NameToLayer("Terrain")) | (1 << LayerMask.NameToLayer("Unit"));
			Collider[] coll = Physics.OverlapSphere(newPos + new Vector3(0, 0.05f, 0), radius, layerMask);
			/*var enemy = OverlapEnemy(coll);
			if (enemy != null)
			{
				return transform.position;
			}*/
			if (SceneGraph.PosIsPassable(newPos))
			{
				return newPos;
			}
				
		}
		return GetNewPos(desiredDir);
	}

	private TroopController OverlapEnemy(Collider[] coll)
	{
		foreach (Collider collider in coll)
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
			{
				var tc = collider.gameObject.GetComponentInParent<TroopController>();
				if (tc.unit != unit)
					return tc;
			}
		}
		return null;
	}

	private bool OverlapTerrain(Collider[] coll)
	{
		foreach (Collider collider in coll)
		{
			int x = LayerMask.NameToLayer("Terrain");
			if (collider.gameObject.layer == x)
				return true;
		}
		return false;
	}

	private Vector3 GetNewPos(Vector3 dir)
	{
		return transform.position + dir.normalized * speed;
	}
}
