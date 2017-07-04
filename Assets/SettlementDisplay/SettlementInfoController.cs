using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlementInfoController : MonoBehaviour
{
	public Text settlementName;
	public Text kingdomName;
	public Image crown;
	public Image heraldry;
	public Settlement settlement;
	// Use this for initialization
	void Start () {
		kingdomName.text = settlement.kingdom.name;
		settlementName.text = settlement.name;
		crown.gameObject.SetActive(settlement == settlement.kingdom.settlements[0]);
		heraldry.sprite = Sprite.Create(settlement.kingdom.heraldry, new Rect(0, 0, settlement.kingdom.heraldry.width, settlement.kingdom.heraldry.height), new Vector2(0.5f, 0.5f));
	}
}
