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
		settlementImage.sprite = Sprite.Create(settlement.kingdom.heraldry, new Rect(0, 0, settlement.kingdom.heraldry.width, settlement.kingdom.heraldry.height), new Vector2(0.5f, 0.5f));
	}
}
