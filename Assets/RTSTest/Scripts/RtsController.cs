using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsController : MonoBehaviour
{
	private UnitController SelectedUnit;
	private BoxCollider selector;

	// Update is called once per frame
	void Update ()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			if (Input.GetMouseButtonDown(0))
			{
				HandleLeftDown(hit);
			}

			if (Input.GetMouseButtonDown(1))
			{
				HandleRightDown(hit);
			}

			if (Input.GetMouseButton(1))
				HandleRightDrag(hit);
		}
	}

	private void HandleLeftDown(RaycastHit hit)
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
		{
			SelectedUnit = hit.collider.gameObject.GetComponentInParent<TroopController>().unit;
		}
	}

	Vector3 downPos;
	private void HandleRightDown(RaycastHit hit)
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			if (SelectedUnit != null)
			{
				downPos = hit.point;
			}
		}

		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
		{
			var unitHit = hit.collider.GetComponent<TroopController>().unit;
			if (SelectedUnit != null && unitHit != SelectedUnit)
			{
				SelectedUnit.SetTarget(unitHit);
			}
		}
	}

	private void HandleRightDrag(RaycastHit hit)
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			if (SelectedUnit != null && Vector3.Magnitude(downPos.FromTo(hit.point)) > 2f)
			{
				Vector3 desiredPos = hit.point;
				Vector3 desiredAngle = Quaternion.AngleAxis(-90, Vector3.up) * downPos.FromTo(hit.point);
				float dist = Vector3.Magnitude(downPos.FromTo(hit.point));

				Vector3 tmp = SelectedUnit.transform.eulerAngles;
				SelectedUnit.transform.LookAt(SelectedUnit.transform.position + desiredAngle);
				Vector3 desiredRot = SelectedUnit.transform.eulerAngles;
				SelectedUnit.transform.eulerAngles = tmp;

				SelectedUnit.SetDesiredUnitPos(desiredPos, desiredRot);
				
				SelectedUnit.desiredWidth = dist;
			}
		}
	}
}
