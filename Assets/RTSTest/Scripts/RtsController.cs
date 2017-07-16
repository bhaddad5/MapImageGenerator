using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsController : MonoBehaviour
{
	private UnitController SelectedUnit;

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
				SelectedUnit.transform.LookAt(hit.point);
				SelectedUnit.transform.position = hit.point;

				downPos = hit.point;
			}
		}
	}

	private void HandleRightDrag(RaycastHit hit)
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			if (SelectedUnit != null && Vector3.Magnitude(downPos.FromTo(hit.point)) > 2f)
			{
				Vector3 desiredPos = (downPos + hit.point) / 2;
				Vector3 desiredAngle = Quaternion.AngleAxis(-90, Vector3.up) * downPos.FromTo(hit.point);
				float dist = Vector3.Magnitude(downPos.FromTo(hit.point));

				SelectedUnit.transform.position = desiredPos;
				SelectedUnit.transform.LookAt(SelectedUnit.transform.position + desiredAngle);
				SelectedUnit.numColumns = (int)(dist / SelectedUnit.spacing);
			}
		}
	}
}
