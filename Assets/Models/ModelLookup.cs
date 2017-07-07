using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLookup : MonoBehaviour
{
	[Serializable]
	public class LookupEntry
	{
		public string name;
		public GameObject gameObject;
	}

	[SerializeField]
	public List<LookupEntry> Lookup = new List<LookupEntry>();

	public GameObject LookupModel(string str)
	{
		foreach (var pair in Lookup)
		{
			if (pair.name == str)
				return pair.gameObject;
		}
		return null;
	}

	[Header("MapEnvironment")]
	public GameObject WheatField;


	[Header("Anglo Settlements")]
	public GameObject Hovel;
	public GameObject TownHouse;	
	public GameObject Wall;
	public GameObject Turret;
	public GameObject Gates;
	public GameObject Bridge;
	public GameObject WoodenWall;
	public GameObject WoodenGate;

	[Header("Dwarf Settlements")]
	public GameObject DwarfHouse;
	public GameObject DwarfWall;
	public GameObject DwarfGates;
	public GameObject DwarfTower;

	[Header("Orc Settlements")]
	public GameObject OrcWall;
	public GameObject OrcGate;
	public GameObject OrcTower;
	public GameObject OrcHut;
}
