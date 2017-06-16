using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlementInfoController : MonoBehaviour
{
	public Text settlementName;
	public Image settlementImage;
	public Settlement settlement;
	// Use this for initialization
	void Start () {
		settlementName.text = settlement.name;
		settlementImage.sprite = null;
	}
}
