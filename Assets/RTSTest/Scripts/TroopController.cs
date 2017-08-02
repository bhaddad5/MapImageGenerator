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

	public static bool Set = false;

	void Awake()
	{
		anim = GetComponent<Animator>();

		if (!Set)
		{
			AnimationClip clip = anim.runtimeAnimatorController.animationClips[3];

			AnimationEvent evt;
			evt = new AnimationEvent();
			evt.time = .5f;
			evt.functionName = "ExecuteAttack";

			clip.AddEvent(evt);
			Set = true;
		}
	}

	public IEnumerable SectorsToTest(Int2 startPos)
	{
		IEnumerable<TroopController> ctrls = new List<TroopController>();
		
		Int2 offset = new Int2(1, 1);
		if (transform.position.x % SceneGraph.ForceMapPartitionSize - SceneGraph.ForceMapPartitionSize/2f <= 0)
			offset.X = -1;
		if (transform.position.z % SceneGraph.ForceMapPartitionSize - SceneGraph.ForceMapPartitionSize/2f <= 0)
			offset.Y = -1;

		ctrls = ctrls.Concat(SceneGraph.ForceMap.Get(startPos))
			.Concat(SceneGraph.ForceMap.Get(startPos + new Int2(offset.X, 0)))
			.Concat(SceneGraph.ForceMap.Get(startPos + new Int2(0, offset.Y)))
			.Concat(SceneGraph.ForceMap.Get(startPos + offset));

		return ctrls;
	}

	public void MoveTowardsDesiredPos(Vector3 pos, Vector3 rot)
	{
		Vector3 startPos = transform.position;
		Int2 startCell = new Int2((int)transform.position.x/10, (int)transform.position.z/10);

		Vector3 closestEnemyPos = Vector3.zero;
		List<Vector3> pushingTroops = new List<Vector3>();
		
		float maxRange = SceneGraph.ForceMapPartitionSize;
		float closestRange = SceneGraph.ForceMapPartitionSize + .1f;

		float pushRange = 0.5f;
		foreach (TroopController troop in SectorsToTest(startCell))
		{
			if (troop == this)
				continue;

			float dist = transform.position.FromTo(troop.transform.position).magnitude;
			if (troop.unit != unit)
			{
				if (dist < closestRange)
				{
					closestRange = dist;
					closestEnemyPos = troop.transform.position;
				}
			}
			if (dist < pushRange)
			{
				pushingTroops.Add(troop.transform.position);
			}
		}

		float attackRange = 1f;
		Vector3 desiredPos = pos;
		Vector3 desiredRot = rot;
		if (closestRange <= maxRange)
		{
			desiredPos = closestEnemyPos;
			if (closestRange <= attackRange)
			{
				anim.SetBool("Fighting", true);
			}
		}

		if(closestRange > attackRange)
			transform.position = Vector3.MoveTowards(transform.position, desiredPos, speed);

		if (transform.position.FromTo(pos).magnitude < 0.001f)
			transform.eulerAngles = desiredRot;
		else transform.LookAt(desiredPos);

		foreach (Vector3 push in pushingTroops)
		{
			var vecToPushingTroop = transform.position.FromTo(push);
			transform.position += -vecToPushingTroop.normalized * (pushRange - vecToPushingTroop.magnitude);
		}

		transform.position = SceneGraph.HeightAdjustedPos(transform.position);

		anim.SetFloat("Speed", startPos.FromTo(transform.position).magnitude);

		/*HandleEnemyInteraction();
		if (transform.position.FromTo(pos).magnitude < speed)
		{
			transform.position = SceneGraph.HeightAdjustedPos(pos);
			transform.eulerAngles = rot;
			anim.SetFloat("Speed", unit.currMoveSpeed);
		}
		else
		{
			transform.position = SceneGraph.HeightAdjustedPos(TestGetNewPos(pos));
			transform.LookAt(pos);
			anim.SetFloat("Speed", unit.currMoveSpeed + speed);
		}*/

		Int2 newPos = new Int2((int)transform.position.x / 10, (int)transform.position.z / 10);

		SceneGraph.ForceMap.Get(startCell).Remove(this);
		SceneGraph.ForceMap.Get(newPos).Add(this);
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
					currEnemy = troop;
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

	private TroopController currEnemy;
	public void ExecuteAttack()
	{
		currEnemy.RecieveAttack(this);
	}

	public void RecieveAttack(TroopController enemyTroop)
	{
		float adjustedDefense = unit.defense * 8f;
		float totalOdds = enemyTroop.unit.attack + adjustedDefense;
		if (Helpers.Odds(enemyTroop.unit.attack / totalOdds))
			DestroyTroop();
	}

	public void DestroyTroop()
	{
		unit.LoseTroop(this);
		GameObject.Destroy(gameObject);
	}
}
