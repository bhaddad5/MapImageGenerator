using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiToggler : MonoBehaviour
{
	public List<GameObject> ObjsToToggle;

	public void ToggleObj()
	{
		foreach (GameObject o in ObjsToToggle)
		{
			o.SetActive(!o.activeInHierarchy);
		}
	}
}
