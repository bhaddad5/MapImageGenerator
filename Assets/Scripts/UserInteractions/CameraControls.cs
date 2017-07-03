using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey(KeyCode.Q))
			transform.eulerAngles += new Vector3(0, -1f, 0);
		if (Input.GetKey(KeyCode.E))
			transform.eulerAngles += new Vector3(0, 1f, 0);

		if (Input.GetKey(KeyCode.Alpha1))
			transform.eulerAngles += new Vector3(1f, 0, 0);
		if (Input.GetKey(KeyCode.Alpha2))
			transform.eulerAngles += new Vector3(-1f, 0, 0);

		Vector3 camFlatForward = transform.forward;
		camFlatForward.y = 0;

		if (Input.GetKey(KeyCode.W))
			transform.position += camFlatForward * 0.2f;
		if (Input.GetKey(KeyCode.A))
			transform.position += transform.right * -0.2f;
		if (Input.GetKey(KeyCode.S))
			transform.position += camFlatForward * -0.2f;
		if (Input.GetKey(KeyCode.D))
			transform.position += transform.right * 0.2f;

		float zoom = Input.GetAxis("Mouse ScrollWheel") * 15f;
		float newZoom = Mathf.Max(2, Mathf.Min(30f, transform.position.y - zoom));

		transform.position = new Vector3(transform.position.x, newZoom, transform.position.z);
	}
}
