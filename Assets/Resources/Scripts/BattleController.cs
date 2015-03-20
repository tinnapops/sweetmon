﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
	public Transform[] leftLocation	= new Transform[3];
	public Transform[] rightLocation	= new Transform[3];
	public GameObject[] battleSF;
	public GameObject[] DeathSF;

	public GameObject defaultMon;
	public GameControl gCtrl;

	List<Monster> leftTeam = new List<Monster> ();
	List<Monster> rightTeam = new List<Monster> ();
	Monster[] monsters	= new Monster[6];
		
	// Use this for initialization


	public SkillBox[,] sb = new SkillBox[3,3];
	public void OnEnable() 
	{
		result = null;
		leftTeam.Clear ();
		rightTeam.Clear ();

		for (int i = 0; i < sb.GetLength(0); i++)
		{
			for (int j = 0 ; j < sb.GetLength(1); j++)
			{
				sb[i,j].last_y = j;
				sb[i,j].id = Random.Range(0, 3);
				//sb[i,j].id = 3;
			}
		}

		leftTeam.Clear();
		foreach(int i in Enumerable.Range(0,leftLocation.Length))
			leftTeam.Add(AddFighter(leftLocation[i],gCtrl.teamSelect[i],false));

		foreach(int i in Enumerable.Range(0,rightLocation.Length))
			rightTeam.Add(AddFighter(rightLocation[i],gCtrl.CurrentQuest.enemy[i],true));
		
		int x	= 0;
		foreach(var mon in leftTeam.Concat(rightTeam))
		{
			monsters[x]	= mon;
			x++;
		}

		foreach(var monster in monsters)
			Debug.Log(monster);

		ClearActionQueue (false);
	}

	public Texture2D[] colorBox = new Texture2D[6];
	Monster AddFighter(Transform location,int index,bool inverse)
	{
		GameObject monObj;
		if(gCtrl.monsters[index].prefab != null)
			monObj	= GameObject.Instantiate(gCtrl.monsters[index].prefab);
		else monObj	= GameObject.Instantiate(defaultMon);

		monObj.transform.parent	= gameObject.transform;
		monObj.transform.position	= location.position;
		monObj.transform.localScale	= Vector3.Scale(monObj.transform.localScale,location.localScale);
		var newFighter = monObj.GetComponent<Monster>();
		if(newFighter == null)
			newFighter = monObj.AddComponent<Monster>();

		newFighter.hp = gCtrl.monsters[index].hp;
		newFighter.atk = gCtrl.monsters[index].atk;
		newFighter.def = gCtrl.monsters[index].def;
		newFighter.wis = gCtrl.monsters[index].wis;
		newFighter.limitBreak = 5;
		newFighter.currentHp = newFighter.hp;
		newFighter.currentLimit = 0;


		var skillData	= gCtrl.skills[gCtrl.monsters[index].skillID];
		newFighter.skill = new Skill(skillData.name,skillData.multiply,skillData.target);
		return newFighter;
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log(isAnimationPlay);
			Debug.Log (leftAnimQueue.Count);
			Debug.Log (RightAnimQueue.Count);
		}

		if(result.HasValue)
			return;

		if (actionQueue.Count >= maxActionPoint && !isExecute)
		{
			isExecute = true;
			while(actionQueue.Count > 0)
			{
				var pair	= actionQueue.Dequeue();
				leftAnimQueue.Add(new KeyValuePair<Monster,int>(pair.Key,pair.Value ? 1 : 2));
			}
		}

		if (leftAnimQueue.Count > 0 && !isAnimationPlay)
		{
			var first	= leftAnimQueue.First();
			first	= BattleFunction(first.Value,first.Key,rightTeam,true);

			isAnimationPlay = first.Key.Animate(first.Value);
			leftAnimQueue.RemoveAt(0);
			
			if (leftAnimQueue.Count <= 0)
			{
				EnemyCommand();
			}
		}

		if (leftAnimQueue.Count == 0 && !isAnimationPlay && RightAnimQueue.Count > 0)
		{
			var first	= RightAnimQueue.First();
			Debug.Log("Enemy Turn = " + first.Key);
			first	= BattleFunction(first.Value,first.Key,leftTeam,false);

			isAnimationPlay = first.Key.Animate(first.Value);
			RightAnimQueue.RemoveAt(0);
			if (RightAnimQueue.Count <= 0)
				ClearActionQueue(false);
		}

		foreach(var monster in monsters)
		{
			if(monster == null)
				continue;

			if(!monster.IsIdle)
			{
				monster.Animate(0);
				isAnimationPlay = false;
			}
		}
	}
	
	const int maxActionPoint = 3;
	readonly Queue<KeyValuePair<Monster,bool>> actionQueue = new Queue<KeyValuePair<Monster,bool>>(maxActionPoint);
	readonly List<KeyValuePair<Monster,int>> leftAnimQueue = new List<KeyValuePair<Monster,int>>();
	readonly List<KeyValuePair<Monster,int>> RightAnimQueue = new List<KeyValuePair<Monster,int>>();
	public static bool isAnimationPlay = false;

	public float rotAngle = 0;
	private Vector2 pivotPoint;
	void OnGUI ()
	{
		UI.AutoResize (1024, 768);

		//HP Left Zone

		if(result.HasValue)
		{
			GUI.BeginGroup(new Rect((1024 / 2) - 200, (768 / 2) - 150, 400, 300), "");
			{
				if(result.Value == Result.Win)
					GUI.Box(new Rect(0,0, 400, 300), "Victory");
				else if(result.Value == Result.Lose)
					GUI.Box(new Rect(0,0, 400, 300), "Defeated");
				else GUI.Box(new Rect(0,0, 400, 300), "Draw Game");

				GUI.Box(new Rect(25, 50, 350, 130), "Reward");
				if (GUI.Button(new Rect(100, 190, 200, 80), "OK"))
				{
					gameObject.SetActive(false);
					gCtrl.GetComponent<AudioSource>().Stop();
					gCtrl.GetComponent<AudioSource>().clip = gCtrl.bgm[0];

					gCtrl.GetComponent<AudioSource>().Play();

					foreach(var checkMon in monsters)
					{
						if(checkMon != null)
							GameObject.Destroy(checkMon.gameObject);
					}
				}
			}
			GUI.EndGroup();
		}
		else
		{
			int i	= 0;
			while(i < maxActionPoint - actionQueue.Count)
			{
				GUI.DrawTexture(new Rect(5, 550 + (i * 55), 50 , 50), colorBox[0]);
				i++;
			}

			if(actionQueue.Count > 0)
			{
				if (GUI.Button (new Rect (5, 500 + (55 * 4), 50, 50), "CE")) 
					ClearActionQueue(true);
			}

			i	= 0;
			foreach(var mon in leftTeam)
			{
				if(mon.currentHp > 0 && actionQueue.Count < maxActionPoint)
				{
					if(mon.currentLimit >= mon.limitBreak)
					{
						if(GUI.Button(new Rect(75 +  300  - (i * 150),500,100,50),""))
						{
							mon.currentLimit = 0;
							actionQueue.Enqueue(new KeyValuePair<Monster,bool>(leftTeam[i],true));
						}
					}

					if(GUI.Button(new Rect(75 +  300  - (i * 150),550,100,100),""))
						actionQueue.Enqueue(new KeyValuePair<Monster,bool>(leftTeam[i],false));
				}



				GUI.BeginGroup(new Rect(75 + 300 - (i * 150),650,(100 * mon.currentHp) / mon.hp,30),"");
				{
					GUI.Box(new Rect(0,0,100,30),"HP : " + mon.currentHp +  "/" + mon.hp);
				}
				GUI.EndGroup();


				GUI.BeginGroup(new Rect(75 + 300 - (i * 150),680,100,30),"LB : " + mon.currentLimit + "/" + mon.limitBreak);
				{
					GUI.Box(new Rect(0, 0, 100, 30), "");
					for(int j = 0;j < mon.currentLimit; j++)
					{
						GUI.Box(new Rect(j * 20, 0, 20, 30), "");
					}
				}
				GUI.EndGroup();

				i++;
			}
			
			
			// Right Zone
			foreach(var mon in rightTeam)
			{
				GUI.BeginGroup(new Rect(75 + 500 +(i * 150),5,(100 * mon.currentHp) / mon.hp,30),"");
				{
					GUI.Box(new Rect(0,0,100,30),"HP : " + mon.currentHp +  "/" + mon.hp);
				}
				GUI.EndGroup();


				GUI.BeginGroup(new Rect(75 + 500 + (i * 150),35,100,30),"LB : " + mon.currentLimit + "/" + mon.limitBreak);
				{
					GUI.Box(new Rect(0, 0, 100, 30), "");
					for (int j = 0;j < mon.currentLimit; j++)
					{
						GUI.Box(new Rect(j * 20, 0, 20, 30), "");
					}
				}
				GUI.EndGroup();
			}
		}
	}

	void ClearActionQueue(bool cleanByButton)
	{
		isExecute = false;
		while(actionQueue.Count > 0)
		{
			var action	= actionQueue.Dequeue();
			if(cleanByButton && action.Value)
				action.Key.currentLimit = 5;
		}
	}

	int[] enemyAnimateQueue = new int[3];
	void EnemyCommand()
	{
		enemyAnimateQueue[0] = 0;
		enemyAnimateQueue[1] = 0;
		enemyAnimateQueue[2] = 0;

		enemyAnimateQueue[Random.Range(0,2)] += 1;
		enemyAnimateQueue[Random.Range(0,2)] += 1;
		enemyAnimateQueue[Random.Range(0,2)] += 1;

		for(int i = 0;i < enemyAnimateQueue.Length;i++)
		{
			if(enemyAnimateQueue[i] > 0)
				RightAnimQueue.Add(new KeyValuePair<Monster,int>(rightTeam[i],enemyAnimateQueue[i]));
		}
	}


	bool isExecute = false;
	KeyValuePair<Monster,int> BattleFunction(int step,Monster userMon,List<Monster> targetList, bool isLeftTurn)
	{
		var skill = userMon.skill;

		if(step > 2)
		{
			if(skill.name == "S_ATK")
			{
				PowerAttack(step,skill.target,skill.multiplier,userMon,targetList);
			}
			else if(skill.name == "S_AIM")
			{
				AimAttack(step,skill.target,skill.multiplier,userMon,targetList);
			}
		}
		else NormalAttack(step,0,userMon,targetList);

		CheckDead ();

		checkBattleResult ();

		return new KeyValuePair<Monster,int>(userMon,step);
	}
	
	void CheckDead()
	{
		foreach(var mon in monsters)
		{
			if(mon != null && mon.gameObject != null && mon.currentHp <= 0)
			{
				var cloneSFX = Instantiate(DeathSF[0],mon.gameObject.transform.position - new Vector3(0,0,0.5f),Quaternion.identity) as GameObject;
				GameObject.Destroy(mon.gameObject);
			}
		}
	}

	void HitEffect(GameObject mon)
	{
		var clone = Instantiate(battleSF[0],new Vector3(mon.transform.position.x,mon.transform.position.y,mon.transform.position.z - 0.5f),Quaternion.identity) as GameObject;
	}

	enum Result
	{
		Lose	= -1,
		Draw	= 0,
		Win	= 1,
	}

	Result? result	= null;
	void checkBattleResult()
	{
		if(rightTeam.All((mon) => mon.currentHp <= 0))
		{
			Debug.Log ("You Win");
			result = Result.Win;
		}
		if(leftTeam.All((mon) => mon.currentHp <= 0))
		{
			Debug.Log ("You Lose");
			result = Result.Lose;
		}
	}

	void NormalAttack(float stack,int targetID,Monster userMon,List<Monster> targetList)
	{
		float damage = 1;
		if (targetID < 0)
		{
			foreach (var target in targetList)
			{
				if(target.currentHp <= 0)
					continue;

				damage = (userMon.atk - target.def) + (stack/10f);
				damage = Mathf.Clamp(damage, 1, 99);
				target.currentHp -= damage;
				HitEffect(target.gameObject);
			}
		}
		else
		{
			while(targetID < targetList.Count && (targetList[targetID] == null || targetList[targetID].currentHp <= 0))
				targetID++;

			if(targetID < targetList.Count)
			{
				Debug.Log ("TARGET ID >  " + targetID + " : User >" + userMon);

				damage = (userMon.atk - targetList[targetID].def) + (stack/10f);
				damage = Mathf.Clamp(damage, 1, 99);

				var target	= targetList[targetID];
				target.currentHp -= damage;
				HitEffect(target.gameObject);

				Debug.Log("Normal Atk : " + userMon + " DEAL : " + userMon.atk + " ( " + stack + ") > ENEMY ID : " + target + " : Def " +  target.def + " True Damage > " + damage + " == " + target.currentHp);
			}
		}
	}

	void PowerAttack(float stack, int targetID, float multiply,Monster userMon,List<Monster> targetList)
	{
		Debug.Log ("USE Power ATK");
		float damage = 1;
		if (targetID < 0)
		{
			foreach (var monster in targetList)
			{
				if(monster.currentHp > 0 && !result.HasValue)
				{
					damage = (userMon.atk - monster.def) * (stack * multiply);
					damage = Mathf.Clamp(damage, 1, 99);
					monster.currentHp -= damage;
					HitEffect(monster.gameObject);
				}
			}
		}
		else
		{
			for (int i = 0;i < rightTeam.Count;i++)
			{
				if(targetList[targetID].currentHp <= 0 && !result.HasValue)
				{
					targetID++;
				}
				//Sinoze.Engine.Assert.True(targetID < rightTeam.Count);
			}
			
			if (targetID <= rightTeam.Count)
			{
				damage = ((userMon.atk - targetList[targetID].def) + (stack * multiply));// Power Attack * 2
				damage = Mathf.Clamp(damage, 1, 99);
				targetList[targetID].currentHp -= damage;
				HitEffect(targetList[targetID].gameObject);
			}
			Debug.Log("Power Atk : ATTACKER ID : " + userMon + " DEAL : " + userMon.atk + " * " + stack + " " + " > ENEMY ID : " + targetID + " : Def " +  targetList[targetID].def + " True Damage > " + damage);
		}
	}

	void  AimAttack(float stack, int targetID, float multiply, Monster usreMon, List<Monster> targetList)
	{
		Debug.Log ("USE AIM");
		float damage = 1;
		{
			for (int i = 0; i < rightTeam.Count; i++) 
			{
				if(rightTeam[2 - i].currentHp <= 0 && !result.HasValue) 
				{
					targetID--;
				}
			}
			
			if (targetID <= rightTeam.Count) 
			{
				damage = ((usreMon.atk - rightTeam[targetID].def) * stack);
				damage = Mathf.Clamp(damage, 1, 99);
				rightTeam [targetID].currentHp -= damage;// Power Attack * 2
				HitEffect (rightTeam [targetID].gameObject);
			}
			Debug.Log("AIM Atk : ATTACKER ID : " + usreMon + " DEAL : " + usreMon.atk + " * " + stack + " " + " > ENEMY ID : " + targetID + " : Def " + rightTeam[targetID].def + " True Damage > " + ((usreMon.atk - rightTeam[targetID].def) * stack));
		}
	}
}

[System.Serializable]
public class Skill
{
	public string name;
	public float multiplier;
	public int target;// -1 all >>>
	public Skill(string n,float m, int t)
	{
		name = n;
		multiplier = m;
		target = t;
	}
}

[System.Serializable]
public struct SkillBox
{
	
	public float last_y;
	public int id;
}
