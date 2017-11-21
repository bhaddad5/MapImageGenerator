using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInstantiationController : MonoBehaviour
{
	public Transform TextParent;

	public SettlementInfoDisplay SettlementInfoDisplay;

	public void DisplayText(Vector3 position, MapTextModel textModel)
	{
		if (textModel is SettlementTextModel)
		{
			SettlementInfoDisplay display = Instantiate(SettlementInfoDisplay, TextParent);
			display.settlementDescr.text = (textModel as SettlementTextModel).SettlementDescription;
			display.settlementName.text = (textModel as SettlementTextModel).Text;
			display.transform.position = position;
		}
	}
}
