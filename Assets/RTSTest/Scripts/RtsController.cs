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
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
				{
					SelectedUnit = hit.collider.gameObject.GetComponentInParent<TroopController>().unit;
				}
			}

			if (Input.GetMouseButtonDown(1))
			{
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
				{
					if (SelectedUnit != null)
					{
						SelectedUnit.transform.LookAt(hit.point);
						SelectedUnit.transform.position = hit.point;
					}
				}
			}
		}
	}
}
