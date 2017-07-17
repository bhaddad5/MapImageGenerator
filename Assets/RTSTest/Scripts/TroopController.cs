using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TroopController : MonoBehaviour
{
	private float speed = .2f;
	public UnitController unit { get; set; }

	private float radius = 1f;

	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}


	public void MoveTowardsDesiredPos(Vector3 pos, Vector3 rot)
	{
		if (transform.position.FromTo(pos).magnitude < speed)
		{
			transform.position = SceneGraph.HeightAdjustedPos(pos);
			transform.eulerAngles = rot;
			anim.SetFloat("Speed", 0);
		}
		else
		{
			transform.position = SceneGraph.HeightAdjustedPos(TestGetNewPos(pos));
			transform.LookAt(pos);
			anim.SetFloat("Speed", speed);
		}

		HandleEnemyInteraction();
	}

	private Vector3 lastGoodVec = Vector3.zero;
	private Vector3 TestGetNewPos(Vector3 targetPos)
	{
		Vector3 desiredDir = transform.position.FromTo(targetPos);
		if (TryDir(desiredDir))
		{
			lastGoodVec = desiredDir;
			return GetNewPos(desiredDir);
		}

		SortedDupList<Vector3> dirsToTry = new SortedDupList<Vector3>();
		dirsToTry.Insert(Vector3.forward, Vector3.Dot(lastGoodVec, Vector3.forward));
		dirsToTry.Insert(-Vector3.forward, Vector3.Dot(lastGoodVec, -Vector3.forward));
		dirsToTry.Insert(Vector3.right, Vector3.Dot(lastGoodVec, Vector3.right));
		dirsToTry.Insert(-Vector3.right, Vector3.Dot(lastGoodVec, -Vector3.right));

		foreach (var pair in dirsToTry.GetList())
		{
			if (TryDir(pair.Value))
			{
				lastGoodVec = pair.Value;
				return GetNewPos(pair.Value);
			}
		}
		lastGoodVec = desiredDir;
		return GetNewPos(desiredDir);
	}

	private bool TryDir(Vector3 dir)
	{
		Vector3 newPos = GetNewPos(dir);
		return SceneGraph.PosIsPassable(newPos);


	}

	private Vector3 GetNewPos(Vector3 dir)
	{
		return transform.position + dir.normalized * speed;
	}

	private bool currFighting = false;
	private void HandleEnemyInteraction()
	{
		bool nowFighting = false;

		int layerMask = (1 << LayerMask.NameToLayer("Unit"));
		Collider[] coll = Physics.OverlapSphere(transform.position + new Vector3(0, 0.05f, 0), radius, layerMask);
		var otherTroops = OverlapUnit(coll);
		if (otherTroops.Count > 0)
		{
			foreach (TroopController troop in otherTroops)
			{
				float distToTroop = transform.position.FromTo(troop.transform.position).magnitude;
				if (distToTroop < 1f)
					transform.position -= transform.position.FromTo(troop.transform.position) * (1f-distToTroop);

				if (!nowFighting && troop.unit != unit)
				{
					troop.RecieveAttack(this);
					transform.LookAt(troop.transform);
					nowFighting = true;
				}
			}
		}

		if (nowFighting != currFighting)
		{
			currFighting = nowFighting;
			anim.SetBool("Fighting", nowFighting);
		}
	}

	private List<TroopController> OverlapUnit(Collider[] coll)
	{
		List<TroopController> overlappingUnits = new List<TroopController>();
		foreach (Collider collider in coll)
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("Unit"))
			{
				var tc = collider.gameObject.GetComponent<TroopController>();
				overlappingUnits.Add(tc);
			}
		}
		return overlappingUnits;
	}

	public float lastAttackTime = 0;
	public float attackTimeout = 2f;
	public void RecieveAttack(TroopController enemyTroop)
	{
		if (Time.time > lastAttackTime + attackTimeout)
		{
			lastAttackTime = Time.time;
			HandleAttack(enemyTroop);
		}
	}

	public void HandleAttack(TroopController enemyTroop)
	{
		float adjustedDefense = unit.defense * 3f;
		float totalOdds = enemyTroop.unit.attack + unit.defense * 3f;
		if (Helpers.Odds(enemyTroop.unit.attack / totalOdds))
			DestroyTroop();
	}

	public void DestroyTroop()
	{
		unit.LoseTroop(this);
		GameObject.Destroy(gameObject);
	}
}
