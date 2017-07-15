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

	private Vector3 lastGoodVec = Vector3.zero;
	private Vector3 TestGetNewPos(Vector3 targetPos)
	{
		Vector3 desiredDir = transform.position.FromTo(targetPos);
		if (TryDir(desiredDir))
		{
			lastGoodVec = desiredDir;
			return GetNewPos(desiredDir);
		}

		SortedDupList<Vector3> dirsToTry = new SortedDupList<Vector3>();
		dirsToTry.Insert(Vector3.forward, Vector3.Dot(lastGoodVec, Vector3.forward));
		dirsToTry.Insert(-Vector3.forward, Vector3.Dot(lastGoodVec, -Vector3.forward));
		dirsToTry.Insert(Vector3.right, Vector3.Dot(lastGoodVec, Vector3.right));
		dirsToTry.Insert(-Vector3.right, Vector3.Dot(lastGoodVec, -Vector3.right));

		foreach (var pair in dirsToTry.GetList())
		{
			if (TryDir(pair.Value))
			{
				lastGoodVec = pair.Value;
				return GetNewPos(pair.Value);
			}
		}


		/*for (int i = 0; i < 8; i = i + 1)
		{
			Vector3 newDir = Quaternion.Euler(0, i * 45, 0) * desiredDir;
			//Vector3 newPos = GetNewPos(newDir);

			int layerMask = (1 << LayerMask.NameToLayer("Terrain")) | (1 << LayerMask.NameToLayer("Unit"));
			Collider[] coll = Physics.OverlapSphere(newPos + new Vector3(0, 0.05f, 0), radius, layerMask);
			if (SceneGraph.PosIsPassable(newPos))
			{
				return newPos;
			}
				
		}*/
		lastGoodVec = desiredDir;
		return GetNewPos(desiredDir);
	}

	private bool TryDir(Vector3 dir)
	{
		Vector3 newPos = GetNewPos(dir);
		return SceneGraph.PosIsPassable(newPos);


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
