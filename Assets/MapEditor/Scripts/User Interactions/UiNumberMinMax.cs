using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiNumberMinMax : MonoBehaviour
{
	public TMP_InputField Input;

	public int Value;
	public string Key;
	public int Default;

	private int min = 20;
	private int max = 120;
	// Use this for initialization
	void Start ()
	{
		Value = PlayerPrefs.GetInt(Key);
		if (Value == 0)
			Value = Default;

		Input.onEndEdit.AddListener(HandleEndEdit);
		HandleEndEdit(Value.ToString());
	}

	public void HandleEndEdit(string s)
	{
		Int32.TryParse(s, out Value);
		if (Value < min)
			Value = min;
		if (Value > max)
			Value = max;
		Input.text = Value.ToString();
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.SetInt(Key, Value);
		PlayerPrefs.Save();
	}
}
