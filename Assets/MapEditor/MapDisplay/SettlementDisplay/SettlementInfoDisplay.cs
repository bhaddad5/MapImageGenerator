using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettlementInfoDisplay : MonoBehaviour
{
	public TextMeshProUGUI SettlementName;
	public TextMeshProUGUI KingdomName;
	public TextMeshProUGUI SettlementDescr;
	public Image Crown;
	public Image KingdomHeraldry;
	public Image ProvinceHeraldry;

	void Update()
	{
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z);
	}
}
