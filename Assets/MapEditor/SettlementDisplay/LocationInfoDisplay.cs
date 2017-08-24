using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationInfoDisplay : MonoBehaviour
{
	public Text LocationNameDisplay;
	public Location Location;

	void Start()
	{
		LocationNameDisplay.text = Location.LocationName;
	}
}
