using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopController : MonoBehaviour
{
	public float speed = 0.02f;

	public void MoveTowardsDesiredPos(Vector3 pos, Vector3 rot)
	{
		if (transform.position.FromTo(pos).magnitude < speed)
		{
			transform.position = pos;
			transform.eulerAngles = rot;
		}
		else
		{
			Vector3 newPos = transform.position + transform.position.FromTo(pos).normalized * speed;
			newPos.y = 0;
			transform.position = newPos;
			transform.LookAt(pos);
		}
		
	}
}
