using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopController : MonoBehaviour
{
	public float speed = 0.02f;

	private float radius = 0.01f;

	public void MoveTowardsDesiredPos(Vector3 pos, Vector3 rot)
	{
		if (transform.position.FromTo(pos).magnitude < speed)
		{
			transform.position = pos;
			transform.eulerAngles = rot;
		}
		else
		{
			transform.position = TestGetNewPos(pos);
			transform.LookAt(pos);
		}
		
	}

	private Vector3 TestGetNewPos(Vector3 targetPos)
	{
		Vector3 desiredDir = transform.position.FromTo(targetPos);
		for (int i = 0; i < 8; i++)
		{
			Vector3 newDir = Quaternion.Euler(0, i * 45, 0) * desiredDir;
			Vector3 newPos = GetNewPos(newDir);

			Collider[] coll = Physics.OverlapSphere(newPos + new Vector3(0, 0.05f, 0), radius);
			if (coll.Length == 0)
			{
				return newPos;
			}
				
		}
		return GetNewPos(desiredDir);
	}

	private Vector3 GetNewPos(Vector3 dir)
	{
		return transform.position + dir.normalized * speed;
	}
}
