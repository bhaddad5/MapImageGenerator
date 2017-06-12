using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey(KeyCode.Q))
			Camera.main.transform.eulerAngles += new Vector3(0, -1f, 0);
		if (Input.GetKey(KeyCode.E))
			Camera.main.transform.eulerAngles += new Vector3(0, 1f, 0);

		Vector3 camFlatForward = Camera.main.transform.forward;
		camFlatForward.y = 0;

		if (Input.GetKey(KeyCode.W))
			Camera.main.transform.position += camFlatForward * 0.1f;
		if (Input.GetKey(KeyCode.A))
			Camera.main.transform.position += Camera.main.transform.right * -0.1f;
		if (Input.GetKey(KeyCode.S))
			Camera.main.transform.position += camFlatForward * -0.1f;
		if (Input.GetKey(KeyCode.D))
			Camera.main.transform.position += Camera.main.transform.right * 0.1f;

		float zoom = Input.GetAxis("Mouse ScrollWheel") * 3f;
		float newZoom = Mathf.Max(1, Mathf.Min(13f, Camera.main.transform.position.y - zoom));

		Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, newZoom, Camera.main.transform.position.z);
	}
}
