using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementInfoDisplay : MonoBehaviour
{
	public TextMeshProUGUI settlementName;
	public TextMeshProUGUI kingdomName;
	public TextMeshProUGUI settlementDescr;
	public Image crown;
	public Image heraldry;

	void Update()
	{
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
	}
}
