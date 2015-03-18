using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
	public GameObject defaultMon;
	public Transform[] leftLocation	= new Transform[3];
	public Transform[] rightLocation	= new Transform[3];

	public List<Monster> leftTeam = new List<Monster> ();
	public List<Monster> rightTeam = new List<Monster> ();

	public GameObject[] monsters;

	public GameObject[] battleSF;
	public GameObject[] DeathSF;
	
	// Use this for initialization

	public GameControl gCtrl;

	public void OnEnable() 
	{
		isGameFinish = false;
		leftTeam.Clear ();
		rightTeam.Clear ();
		RefillActionPoint ();

		for (int i = 0; i < sb.GetLength(0); i++)
		{
			for (int j = 0 ; j < sb.GetLength(1); j++)
			{
				sb[i,j].last_y = j;
				sb[i,j].id = Random.Range(0, 3);
				//sb[i,j].id = 3;
			}
		}

		leftTeam	= new List<Monster>(AddFighter(leftLocation,gCtrl.teamSelect,false));
		var quest	= gCtrl.quest[gCtrl.currentQuestSelect];
		rightTeam	= new List<Monster>(AddFighter(rightLocation,quest.enemy,true));

		foreach(var monster in monsters)
			Debug.Log(monster);

	
		ClearActionQueue (false);
		RefillActionPoint ();

		//leftTeam[1].skill = new Skill("S_AIM", 1, 2);
		//leftTeam[2].skill = new Skill("S_AIM", 1, 2);
	}

	public Texture2D[] colorBox = new Texture2D[6];

	IEnumerable<Monster> AddFighter(Transform[] locations,IEnumerable<int> indexes,bool inverse)
	{
		int i	= inverse ? 3 : 0;
		foreach(int index in indexes)
		{
			if(gCtrl.monsters[index].prefab != null)
				monsters[i]	= GameObject.Instantiate(gCtrl.monsters[index].prefab) as GameObject;
			else monsters[i]	= GameObject.Instantiate(defaultMon) as GameObject;

			var location	= inverse ? locations[i - 3] : locations[i];

			monsters[i].transform.parent	= gameObject.transform;
			monsters[i].transform.position	= location.position;
			monsters[i].transform.localScale	= Vector3.Scale(monsters[i].transform.localScale,location.localScale);
			var newFighter = monsters[i].GetComponent<Monster>();
			if(newFighter == null)
				newFighter = monsters[i].AddComponent<Monster>();

			newFighter.hp = gCtrl.monsters[index].hp;
			newFighter.atk = gCtrl.monsters[index].atk;
			newFighter.def = gCtrl.monsters[index].def;
			newFighter.wis = gCtrl.monsters[index].wis;
			newFighter.limitBreak = 5;
			newFighter.currentHp = newFighter.hp;
			newFighter.currentLimit = 0;


			var skillData	= gCtrl.skills[gCtrl.monsters[index].skillID];
			newFighter.skill = new Skill(skillData.name,skillData.multiply,skillData.target);
			yield return newFighter;

			i++;
		}
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
		if (currentActionPoint == 0 && !isExecute)
		{
			isExecute = true;
			var action = ActionExecute();

			if(action != null)
			{
				foreach (var pair in action)
					leftAnimQueue.Add(pair);
			}
		}
		if (leftAnimQueue.Count > 0 && !isAnimationPlay)
		{
			var first	= leftAnimQueue.First();
			first	= BattleFunction(first.Key,first.Value,leftTeam,rightTeam,true);

			isAnimationPlay = true;

			ActionAnimate(first.Key,first.Value);
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
			first	= BattleFunction(first.Key,first.Value,rightTeam,leftTeam,false);
			isAnimationPlay = true;

			ActionAnimate(first.Key,first.Value);

			RightAnimQueue.RemoveAt(0);
			if (RightAnimQueue.Count <= 0)
			{
				RefillActionPoint();
				ClearActionQueue(false);
			}
		}

		foreach(var monster in monsters)
		{
			if(monster == null)
				continue;

			var monAnim	= monster.gameObject.GetComponent<Animation>();
			foreach(AnimationState state in monAnim)
			{
				if(state.name == "anim_idle_01")
				{
					if(!monAnim.IsPlaying("anim_idle_01"))
					{
						monster.gameObject.GetComponent<Animation>().CrossFade("anim_idle_01",0.2f);
						BattleController.isAnimationPlay = false;
					}
					break;
				}

				if(state.name == "idle")
				{
					if(!monAnim.IsPlaying("idle"))
					{
						monster.gameObject.GetComponent<Animation>().CrossFade("idle",0.2f);
						BattleController.isAnimationPlay = false;
					}
					break;
				}
			}
		}
	}
	
	const int maxActionPoint = 3;
	int currentActionPoint = maxActionPoint;
	int[] actionQueue = new int[maxActionPoint]{-1,-1,-1};

	List<KeyValuePair<int,int>> leftAnimQueue = new List<KeyValuePair<int,int>>();
	List<KeyValuePair<int,int>> RightAnimQueue = new List<KeyValuePair<int,int>>();
	public static bool isAnimationPlay = false;


	public KeyValuePair<int, int>[] currentButtonID = new KeyValuePair<int, int>[maxActionPoint];

	public float rotAngle = 0;
	private Vector2 pivotPoint;
	void OnGUI ()
	{
		UI.AutoResize (1024, 768);

		//HP Left Zone

		if (isGameFinish)
		{
			GUI.BeginGroup(new Rect((1024 / 2) - 200, (768 / 2) - 150, 400, 300), "");
			{
				if (result == 0)
				{
					GUI.Box(new Rect(0,0, 400, 300), "Victory");
				}
				else
				{
					GUI.Box(new Rect(0,0, 400, 300), "Defeated");
				}

				GUI.Box(new Rect(25, 50, 350, 130), "Reward");
				if (GUI.Button(new Rect(100, 190, 200, 80), "OK"))
				{
					GameControl.isBattle = false;
					gameObject.SetActive(false);
					gCtrl.GetComponent<AudioSource>().Stop();
					gCtrl.GetComponent<AudioSource>().clip = gCtrl.bgm[0];

					gCtrl.GetComponent<AudioSource>().Play();

					ClearMonsterPrefab();
				}
			}
			GUI.EndGroup();
		}
		else
		{
			for (int i = 0; i < currentActionPoint;i++)
			{
				GUI.DrawTexture(new Rect(5, 550 + (i * 55), 50 , 50), colorBox[0]);
			}
			if (currentActionPoint < 3)
			{
				if (GUI.Button (new Rect (5, 500 + (55 * 4), 50, 50), "CE")) 
				{
					RefillActionPoint();
					ClearActionQueue(true);
				}
			}
			
			for (int i = 0; i < leftTeam.Count; i++)
			{
				if (leftTeam[i].currentHp > 0)
				{
					if (leftTeam[i].currentLimit >= leftTeam[i].limitBreak)
					{
						if (GUI.Button(new Rect(75 +  300  - (i * 150), 500,  100, 50), "") && currentActionPoint > 0)
						{
							leftTeam[i].currentLimit = 0;
							AddActionQueue(i+3);
						}
					}
					
					if (GUI.Button(new Rect(75 +  300  - (i * 150), 550,  100, 100), ""))
					{
						AddActionQueue(i);
					}
				}
				
				
				
				GUI.BeginGroup(new Rect(75 + 300 - (i * 150), 650, (100 * leftTeam[i].currentHp) / leftTeam[i].hp, 30), "");
				{
					GUI.Box(new Rect(0, 0, 100, 30), "HP : " + leftTeam[i].currentHp +  "/" + leftTeam[i].hp);
				}
				GUI.EndGroup();
				
				
				GUI.BeginGroup(new Rect(75 + 300 - (i * 150), 680, 100, 30) , "LB : " + leftTeam[i].currentLimit + "/" + leftTeam[i].limitBreak);
				{
					GUI.Box(new Rect(0, 0, 100, 30), "");
					for (int j = 0;j < leftTeam[i].currentLimit; j++)
					{
						GUI.Box(new Rect(j * 20, 0, 20, 30), "");
					}
				}
				GUI.EndGroup();
				
			}
			
			
			// Right Zone
			for (int i = 0; i < rightTeam.Count; i++)
			{	
				GUI.BeginGroup(new Rect(75 + 500 +(i * 150), 5, (100 * rightTeam[i].currentHp) / rightTeam[i].hp, 30), "");
				{
					GUI.Box(new Rect(0, 0, 100, 30), "HP : " + rightTeam[i].currentHp +  "/" + rightTeam[i].hp);
				}
				GUI.EndGroup();
				
				
				GUI.BeginGroup(new Rect(75 + 500 + (i * 150), 35, 100, 30) , "LB : " + rightTeam[i].currentLimit + "/" + rightTeam[i].limitBreak);
				{
					GUI.Box(new Rect(0, 0, 100, 30), "");
					for (int j = 0;j < rightTeam[i].currentLimit; j++)
					{
						GUI.Box(new Rect(j * 20, 0, 20, 30), "");
					}
				}
				GUI.EndGroup();
			}
		}
	}
	void DrawPuzzle()
	{
		GUI.BeginGroup (new Rect (125, 520, 5 * boxSize, 5 * boxSize), "");
		{
			if (!isGameFinish)
			{
				for (int x = 0; x < sb.GetLength(0); x++) 
				{
					for (int y = 0; y < sb.GetLength(1); y++) 
					{
						
						Rect puzzleRect = new Rect((x * boxSize)
						                           , ((sb[x,y].last_y * boxSize))
						                           , collisionBoxSize
						                           , collisionBoxSize);
						if (currentActionPoint > 0 && Event.current.isMouse && Input.GetMouseButton(0) && puzzleRect.Contains(Event.current.mousePosition))
						{
							if (!currentButtonID.Contains(new KeyValuePair<int, int>(x, y)))
							{
								currentButtonID[maxActionPoint - currentActionPoint] = new KeyValuePair<int, int>(x, y);
								
								AddActionQueue(sb[x,y].id);
							}
						}
						if (Input.GetMouseButtonUp(0))
						{
							RefillActionPoint();
							ClearActionQueue(false);
						}
						puzzleRect.width = boxSize;
						puzzleRect.height = boxSize;
						GUI.DrawTexture(puzzleRect, colorBox[sb[x,y].id]);
						if (sb[x,y].last_y < y)
						{
							sb[x,y].last_y += Time.deltaTime * 2.75f;
						}
						else
						{
							sb[x,y].last_y = y;
						}
					}
				}
				
				for (int i = 0;i < 3; i++)
				{
					if (currentButtonID [i].Key != -1 && currentButtonID [i].Key != -1 && !isShowSelectGrid) 
					{
						GUI.Box (new Rect ((currentButtonID [i].Key * boxSize), (currentButtonID [i].Value * boxSize), boxSize, boxSize), "");
					}
				}
			}
			else
			{
				GUI.BeginGroup(ResultRect, "");
				{
					GUI.Box(new Rect(0,0, ResultRect.width, ResultRect.height), "Result Screen");
				}
				GUI.EndGroup();
			}
		}
		GUI.EndGroup ();
	}

	public Rect ResultRect; 

	bool isShowSelectGrid = false;
	int boxSize = 175;
	int collisionBoxSize = 150;

	
	public  SkillBox[,] sb = new SkillBox[3,3];
	
	void RefillActionPoint()
	{
		currentActionPoint = maxActionPoint;
		//Sinoze.Engine.Assert.True (currentActionPoint==maxActionPoint, "uyiutiu", Sinoze.Engine.AssertLevel.Warning);
	}
	
	void ClearActionQueue(bool cleanByButton)
	{
		for (int i = 0; i < actionQueue.Length; i ++)
		{
			if (actionQueue[i] > 2 && cleanByButton)
			{
				leftTeam[actionQueue[i] - 3].currentLimit = 5;
			}
			actionQueue[i] = -1;
		}
		for (int i = 0; i < currentButtonID.Length; i ++)
		{
			currentButtonID[i] = new KeyValuePair<int, int>(-1,-1);
		}
		isExecute = false;
		isShowSelectGrid = false;
	}
	
	
	void AddActionQueue(int id)
	{
		if (maxActionPoint - currentActionPoint < 3)
		{
			actionQueue [maxActionPoint - currentActionPoint] = id;
			currentActionPoint--;
		}

	}
	
	void ActionAnimate(int id,int step)
	{
		if (id > -1)
		{
			if (monsters[id] != null && monsters[id].gameObject.GetComponent<Animation>() && monsters[id].GetComponent<Monster>().currentHp > 0)
			{
				var monAnim	= monsters[id].gameObject.GetComponent<Animation>();
				foreach(var state in monAnim.Cast<AnimationState>())
				{
					if(state.name == "anim_idle_01")
					{
						if(step == 1)
						{
							monAnim.CrossFade("anim_active_01",0.2f);
						}
						else if(step == 2)
						{
							monAnim.CrossFade("anim_eat_03",0.2f);
						}
						else if(step == 3)
						{
							monAnim.CrossFade("anim_grapStop_01",0.2f);
						}
						break;
					}

					if(state.name == "idle")
					{
						if(step == 1)
						{
							monAnim.CrossFade("attack",0.2f);
						}
						else if(step == 2)
						{
							monAnim.CrossFade("win",0.2f);
						}
						else if(step == 3)
						{
							monAnim.CrossFade("useSkill",0.2f);
						}
						break;
					}
				}
			}
			else
			{
				isAnimationPlay = false;
			}
		}
	}


	int[] enemyAnimateQueue = new int[3];
	void EnemyCommand()
	{
		int randCM = Random.Range (0, 11);

		randCM = 10;
		if (randCM == 0)
		{
			enemyAnimateQueue[0] = 1;
			enemyAnimateQueue[1] = 1;
			enemyAnimateQueue[2] = 1;
		}
		else if (randCM == 1)
		{
			enemyAnimateQueue[0] = 2;
			enemyAnimateQueue[1] = 0;
			enemyAnimateQueue[2] = 1;
		}
		else if (randCM == 2)
		{
			enemyAnimateQueue[0] = 2;
			enemyAnimateQueue[1] = 1;
			enemyAnimateQueue[2] = 0;
		}
		else if (randCM == 3)
		{
			enemyAnimateQueue[0] = 1;
			enemyAnimateQueue[1] = 2;
			enemyAnimateQueue[2] = 0;
		}
		else if (randCM == 4)
		{
			enemyAnimateQueue[0] = 1;
			enemyAnimateQueue[1] = 0;
			enemyAnimateQueue[2] = 2;
		}
		else if (randCM == 5)
		{
			enemyAnimateQueue[0] = 0;
			enemyAnimateQueue[1] = 1;
			enemyAnimateQueue[2] = 2;
		}
		else if (randCM == 6)
		{
			enemyAnimateQueue[0] = 0;
			enemyAnimateQueue[1] = 2;
			enemyAnimateQueue[2] = 1;
		}
		else if (randCM == 7)
		{
			enemyAnimateQueue[0] = 0;
			enemyAnimateQueue[1] = 2;
			enemyAnimateQueue[2] = 1;
		}

		else if (randCM == 8)
		{
			enemyAnimateQueue[0] = 3;
			enemyAnimateQueue[1] = 0;
			enemyAnimateQueue[2] = 0;
		}
		else if (randCM == 9)
		{
			enemyAnimateQueue[0] = 0;
			enemyAnimateQueue[1] = 0;
			enemyAnimateQueue[2] = 3;
		}
		else if (randCM == 10)
		{
			enemyAnimateQueue[0] = 0;
			enemyAnimateQueue[1] = 0;
			enemyAnimateQueue[2] = 3;
		}

		for(int i = 0;i < enemyAnimateQueue.Length;i++)
		{
			if(enemyAnimateQueue[i] > 0)
				RightAnimQueue.Add(new KeyValuePair<int,int>(i + 3,enemyAnimateQueue[i]));
		}
	}


	bool isExecute = false;
	List<KeyValuePair<int, int>> ActionExecute()
	{
		isShowSelectGrid = true;
		int cbDiffX1 = Mathf.Abs(currentButtonID [0].Key - currentButtonID [1].Key);
		int cbDiffX2 = Mathf.Abs(currentButtonID [1].Key - currentButtonID [2].Key);
		int cbDiffY1 = Mathf.Abs(currentButtonID [0].Value - currentButtonID [1].Value);
		int cbDiffY2 = Mathf.Abs(currentButtonID [1].Value - currentButtonID [2].Value);


		if (cbDiffX1 > 1 || cbDiffY1 > 1|| cbDiffX2 > 1 || cbDiffY2 > 1)
		{
			RefillActionPoint();
			ClearActionQueue(false);
			return null;
		}
		var output = new List<KeyValuePair<int, int>> ();


		for (int i = 0;i < actionQueue.Length;i++)
		{
			if (actionQueue[i] < 3)
			{
				GainLimitBreak(actionQueue [i], 1);
			}
		}
		
		if (actionQueue [0] == actionQueue [1] && actionQueue [0] == actionQueue [2])
		{
			output.Add(new KeyValuePair<int,int>(actionQueue[0],2));
		} 
		else 
		{
			if (actionQueue [0] == actionQueue [1])
			{
				output.Add(new KeyValuePair<int,int>(actionQueue[0],1));
				output.Add(new KeyValuePair<int,int>(actionQueue[0],1));
				output.Add(new KeyValuePair<int,int>(actionQueue[2],1));
			}
			else if (actionQueue [1] == actionQueue [2])
			{
				output.Add(new KeyValuePair<int,int>(actionQueue[0],1));
				output.Add(new KeyValuePair<int,int>(actionQueue[1],1));
				output.Add(new KeyValuePair<int,int>(actionQueue[1],1));
			}
			else
			{
				output.Add(new KeyValuePair<int,int>(actionQueue[0],1));
				output.Add(new KeyValuePair<int,int>(actionQueue[1],1));
				output.Add(new KeyValuePair<int,int>(actionQueue[2],1));
			}
		}
		
		return output;
	}
	
	KeyValuePair<int,int> BattleFunction(int userID, int step, List<Monster> userList, List<Monster> targetList, bool isLeftTurn)
	{
		if (isLeftTurn)
		{
			if (userID > 2)
			{
				userID -= 3;
				var skill = userList [userID].skill;
				
				if (skill.name == "S_ATK")
				{
					PowerAttack(userID, step, skill.target, skill.multiplier, userList, targetList);
				}
				else if (skill.name == "S_AIM")
				{
					AimAttack(userID, step, skill.target, skill.multiplier , userList, targetList);
				}
			}
			else
			{
				NormalAttack (userID, step, 0, userList, targetList);
			}
		}
		else
		{
			if (userID > 5)
			{
				userID -= 3;
				var skill = userList [userID].skill;
				
				if (skill.name == "S_ATK")
				{
					PowerAttack(userID, step, skill.target, skill.multiplier, userList, targetList);
				}
				else if (skill.name == "S_AIM")
				{
					AimAttack(userID, step, skill.target, skill.multiplier , userList, targetList);
				}
			}
			else
			{
				NormalAttack (userID - 3, step, 0, userList, targetList);
			}
		}

		CheckDead ();

		checkBattleResult ();

		return new KeyValuePair<int,int>(userID,step);
	}

	void GainLimitBreak(int userID, int step)
	{
		if (userID < 3 && userID > -1)
		{
			leftTeam[userID].currentLimit += step;
			leftTeam[userID].currentLimit = Mathf.Clamp(leftTeam[userID].currentLimit, 0, 5);
		}
			
	}

	void CheckDead()
	{
		foreach (var checkMon in monsters)
		{
			if(checkMon == null)
				continue;

			var mon = checkMon.GetComponent<Monster>();
			if (mon.currentHp <= 0)
			{
				var cloneSFX = Instantiate (DeathSF [0], new Vector3 (mon.gameObject.transform.position.x, mon.gameObject.transform.position.y , mon.gameObject.transform.position.z - 0.5f), Quaternion.identity) as GameObject;
				GameObject.Destroy(mon.gameObject);
			}
		}
	}

	void ClearMonsterPrefab()
	{
		foreach (var checkMon in monsters)
		{
			GameObject.Destroy(checkMon);
		}
	}

	void HitEffect(GameObject mon)
	{
		GameObject clone = Instantiate (battleSF [0], new Vector3 (mon.transform.position.x, mon.transform.position.y , mon.transform.position.z - 0.5f), Quaternion.identity) as GameObject;
	}

	bool isGameFinish = false;
	int result = 0;// 0 - win/1 - lose/2 - draw
	
	void checkBattleResult()
	{
		bool enemyAlive = false;
		for (int i = 0; i < rightTeam.Count; i++)
		{
			if (rightTeam[i].currentHp > 0)
			{
				enemyAlive = true;
			}
		}
		bool playerAlive = false;
		for (int i = 0; i < leftTeam.Count; i++)
		{
			if (leftTeam[i].currentHp > 0)
			{
				playerAlive = true;
			}
		}
		if(!enemyAlive)
		{
			Debug.Log ("You Win");
			isGameFinish = true;
			result = 0;
		}
		if (!playerAlive)
		{
			Debug.Log ("You Lose");
			isGameFinish = true;
			result = 1;
		}

	}

	void  NormalAttack(int userID, float stack, int targetID, List<Monster> userList, List<Monster> targetList)
	{
		float damage = 1;
		if (targetID < 0)
		{
			foreach (var monster in targetList)
			{
				if (monster.currentHp > 0 && !isGameFinish)
				{
					damage = (userList [userID].atk - monster.def) + (stack/10f);
					damage = Mathf.Clamp(damage, 1, 99);
					monster.currentHp -= damage;
					HitEffect(monster.gameObject);
				}
			}
		}
		else
		{
			for (int i = 0;i < targetList.Count;i++)
			{
				if (targetList[targetID].currentHp <= 0 && !isGameFinish)
				{
					targetID++;
				}
				//Sinoze.Engine.Assert.True(targetID < rightTeam.Count);
			}

			if(targetID < targetList.Count && targetList[targetID])
			{
				Debug.Log ("TARGET ID >  " + targetID + " : User >" + userID);
				damage = (userList [userID].atk - targetList[targetID].def) + (stack/10f);
				damage = Mathf.Clamp(damage, 1, 99);
				targetList[targetID].currentHp -= damage;
				HitEffect(targetList[targetID].gameObject);

				Debug.Log("Normal Atk : ATTACKER ID : " + userID + " DEAL : " + userList[userID].atk + " ( " + stack +") > ENEMY ID : " + targetID + " : Def " +  targetList[targetID].def + " True Damage > " + damage);
			}
		}
	}

	void  PowerAttack(int userID, float stack, int targetID, float multiply, List<Monster> userList, List<Monster> targetList)
	{
		Debug.Log ("USE Power ATK");
		float damage = 1;
		if (targetID < 0)
		{
			foreach (var monster in targetList)
			{
				if (monster.currentHp > 0 && !isGameFinish)
				{
					damage = (userList [userID].atk - monster.def) * (stack * multiply);
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
				if (targetList[targetID].currentHp <= 0 && !isGameFinish)
				{
					targetID++;
				}
				//Sinoze.Engine.Assert.True(targetID < rightTeam.Count);
			}
			
			if (targetID <= rightTeam.Count)
			{
				damage = ((userList [userID].atk - targetList[targetID].def) + (stack * multiply));// Power Attack * 2
				damage = Mathf.Clamp(damage, 1, 99);
				targetList[targetID].currentHp -= damage;
				HitEffect(targetList[targetID].gameObject);
			}
			Debug.Log ("Power Atk : ATTACKER ID : " + userID + " DEAL : " + userList [userID].atk + " * " + stack + " " + " > ENEMY ID : " + targetID + " : Def " +  targetList[targetID].def + " True Damage > " + damage);
		}
	}

	void  AimAttack(int userID, float stack, int targetID, float multiply, List<Monster> userList, List<Monster> targetList)
	{
		Debug.Log ("USE AIM");
		float damage = 1;
		{
			for (int i = 0; i < rightTeam.Count; i++) 
			{
				if (rightTeam [2 - i].currentHp <= 0 && !isGameFinish) 
				{
					targetID--;
				}
			}
			
			if (targetID <= rightTeam.Count) 
			{
				damage = ((leftTeam [userID].atk - rightTeam [targetID].def) * stack);
				damage = Mathf.Clamp(damage, 1, 99);
				rightTeam [targetID].currentHp -= damage;// Power Attack * 2
				HitEffect (rightTeam [targetID].gameObject);
			}
			Debug.Log ("AIM Atk : ATTACKER ID : " + userID + " DEAL : " + leftTeam [userID].atk + " * " + stack + " " + " > ENEMY ID : " + targetID + " : Def " + rightTeam [targetID].def + " True Damage > " + ((leftTeam [userID].atk - rightTeam [targetID].def) * stack));
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
