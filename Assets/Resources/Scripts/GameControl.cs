using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class GameControl : MonoBehaviour
{
	public static int coin = 1000;
	public static int gem = 1000;
	public static int key = 100;

	public BattleController battleOBJ;
	AudioSource audioSource;
	void Start() 
	{
		ReadPlayerData ();
		CheckSeen ();
		audioSource	= gameObject.GetComponent<AudioSource>();
		audioSource.Stop();
		audioSource.clip = bgm[0];
		audioSource.Play();

		battleOBJ	= gameObject.transform.GetChild(0).GetComponent<BattleController>();

		gameObject.AddComponent<GameControlUI.Title>();
	}

	public AudioClip[] bgm;

	public AudioClip[] buttonSFX;

	
	public MonsterData[] playerMonster;
	public MonsterData[] monsters;
	public Quest[] quest;

	public int currentQuestSelect = 0;
	public Quest CurrentQuest
	{
		get { return quest[currentQuestSelect]; }
	}

	public List<int> teamSelect = new List<int>();

	public Rect statusPosition;

	public Rect battlePosition;
	public Rect FeedingPosition;

	public Rect monsterStockRect;
	public Rect monsterListStockRect;

	public Rect monsterInfoRect;

	public Rect questInfoRect;

	public bool isMonsterInfoPopup = false;

	public Texture2D startScreen;

	public Vector2 scrollPosition;

	public int currentSelectMonsterInfo = 0;
	public void AutoResize()
	{
		int screenWidth	= 1024;
		int screenHeight	= 768;
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth,(float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(resizeRatio.x,resizeRatio.y,1.0f));
	}

	void OnGUI()
	{
		/*
		if(start >= 0)
		{
			if(start == 0)
			{
				if(Input.anyKeyDown)
					start	+= Time.deltaTime;
			}
			else start	+= Time.deltaTime;
			if(start > 1)
				start	= -1;

			var tmp	= GUI.color;
			GUI.color = new Color(1,1,1,1 - start);
			GUI.DrawTexture(new Rect(0,0,1024,768), startScreen);
			GUI.color	= tmp;

			return;
		}
		*/

		if(battleOBJ.gameObject.activeSelf)
			return;

		AutoResize();

		//Status Bar
		GUI.BeginGroup (statusPosition, "");
		{
			GUI.Box(new Rect(0, 0, statusPosition.width, statusPosition.height), "");
			GUI.Box(new Rect(5, 5, 310, 50), "Player LV");
			
			if (teamSelect.Count > 0)
			{
				monsters[teamSelect.First()].Draw(new Rect(5, 5, 50, 50));
			}
			else
			{
				GUI.Box(new Rect(5, 5, 50, 50), "?");
			}
			GUI.Box(new Rect(320, 5, 215, 50), "Currency 1");
			GUI.Box(new Rect(320 + 220, 5, 215, 50), "Currency 2");
			GUI.Box(new Rect(320 + 220 + 220, 5, 215, 50), "Dungeon Ticket");
		}
		GUI.EndGroup ();
		

	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.H))
		{
			PlayerPrefs.DeleteAll();
			isHackShiny = true;
		}
	}

	public void AddMonsterToTeam(int id)
	{

		if (!teamSelect.Contains(id))
		{
			if (teamSelect.Count < 3)
			{
				teamSelect.Add(id);
			}
		}
		else
		{
			teamSelect.Remove(id);
		}
	}


	public void writePlayerData()
	{
		string playerData = "";
		for (int i = 0; i < playerMonster.Length;i++)
		{
			string tempData = playerMonster[i].id + "^" + playerMonster[i].level + "^" + playerMonster[i].exp + "^" + playerMonster[i].full + "^" + playerMonster[i].happy;
			if (playerMonster[i].isShiny)
			{
				tempData += "^1";
			}
			else
			{
				tempData += "^0";
			}
			if (i != playerMonster.Length - 1)
			{
				tempData += "/";
			}
			playerData += tempData;
		}
		Debug.Log ("PLAYERCODE = " + playerData);

		string playerDexData = "";
		for (int i = 0; i < monsters.Length;i++)
		{
			if (monsters[i].isSeen)
			{
				playerDexData += "1";
			}
			else
			{
				playerDexData += "0";
			}

			if (i != monsters.Length - 1)
			{
				playerDexData += "^";
			}
		}
		Debug.Log ("DEXCODE = " + playerDexData);

		PlayerPrefs.SetString ("PLAYERCODE", playerData);
		PlayerPrefs.SetString ("DEXCODE", playerDexData);
	}

	void OnApplicationQuit() 
	{
		PlayerPrefs.Save();
	}

	public void ReadPlayerData()
	{
		string playerCode = PlayerPrefs.GetString ("PLAYERCODE", "0^0^0^0^0^0/6^0^0^0^0^0/6^0^0^0^0^1/9^0^0^0^0^0/9^0^0^0^0^1/10^0^0^0^0^0/12^0^0^0^0^0/15^0^0^0^0^0/16^0^0^0^0^0/17^0^0^0^0^0");
		string dexCode = PlayerPrefs.GetString ("DEXCODE", "1^0^0^0^0^0^1^0^0^0^0^0^1^0^0^0^0^0");
		string[] spiltPlayerCode = 	playerCode.Split('/');
		playerMonster = new MonsterData[spiltPlayerCode.Length];
		for (int i = 0; i < spiltPlayerCode.Length;i++)
		{
			string[] tempStat = spiltPlayerCode[i].Split('^');
			MonsterData tempMon = new MonsterData();
			tempMon = monsters[int.Parse(tempStat[0])];
			tempMon.id = int.Parse(tempStat[0]);
			tempMon.level = int.Parse(tempStat[1]);
			tempMon.exp = int.Parse(tempStat[2]);
			tempMon.full = int.Parse(tempStat[3]);
			tempMon.happy = int.Parse(tempStat[4]);

			if (int.Parse(tempStat[5]) == 1)
			{
				Debug.Log ("Detect Shiny");
				tempMon.isShiny = true;
				tempMon.image = monsters[int.Parse(tempStat[0])].shinyImage;
			}

			playerMonster[i] = tempMon;
		}
		string[] spiltDexCode = dexCode.Split('^');

		for (int i = 0; i < spiltDexCode.Length;i++)
		{
			if (spiltDexCode[i] == "1")
			{
				monsters[i].isSeen = true;
			}
			else
			{
				monsters[i].isSeen = false;
			}
		}
	}


	void CheckSeen()
	{
		for (int i = 0;i < monsters.Length;i++)
		{
			for (int j = 0;j < playerMonster.Length;j++)
			{
				if (i == playerMonster[j].id)
				{
					monsters[i].isSeen = true;
				}
			}
		}
	}

	bool isHackShiny = false;
	public void MonsterGachapon(int id)//-1 = Normal Random, -2 = Shiny Chance Random, 0++ = fix ID
	{
		MonsterData tempMon = new MonsterData();
		int rid = id;
		if (id < 0)
		{
			rid = Random.Range(0, monsters.Length);
		}

		tempMon = monsters[rid];
		int shinyChance = 0;
		if (id == -2)
		{
			shinyChance = Random.Range(0,101);
		}
		if (shinyChance > 95 || isHackShiny)
		{
			tempMon = monsters[6];// ** Hack 01
			tempMon.isShiny = true;
		}
		System.Array.Resize (ref playerMonster, playerMonster.Length + 1);
		playerMonster [playerMonster.Length - 1] = tempMon;
	}

	public void Breed(int userMonsterID,int friendMonsterID, bool isFriendShiny)
	{
		MonsterData tempMon = new MonsterData();
		int starterMon = 0;
		int randomShape = Random.Range(0, 2);
		if (randomShape < 1)
		{
			if (userMonsterID == 3 || userMonsterID == 4 || userMonsterID == 3)
			{
				starterMon = 0;
			}
			else if (userMonsterID == 9 || userMonsterID == 10 || userMonsterID == 11)
			{
				starterMon = 6;
			}
			else if (userMonsterID == 15 || userMonsterID == 16 || userMonsterID == 17)
			{
				starterMon = 12;
			}
		}
		else
		{
			if (friendMonsterID == 3 || friendMonsterID == 4 || friendMonsterID == 3)
			{
				starterMon = 0;
			}
			else if (friendMonsterID == 9 || friendMonsterID == 10 || friendMonsterID == 11)
			{
				starterMon = 6;
			}
			else if (friendMonsterID == 15 || friendMonsterID == 16 || friendMonsterID == 17)
			{
				starterMon = 12;
			}
		}
		tempMon = monsters[starterMon];

		int shinyChance = Random.Range(5,101);;
		if (isFriendShiny)
		{
			shinyChance += 5;
		}
		if (shinyChance > 95)
		{
			tempMon.isShiny = true;
		}
		System.Array.Resize (ref playerMonster, playerMonster.Length + 1);
		playerMonster [playerMonster.Length - 1] = tempMon;
	}


	public void Evolution(int monsterID,int lockDNA)//-1 Random From Happy, 0 Left Lock, 1 Right Lock
	{
		MonsterData tempMon = new MonsterData();
		if (lockDNA == -1)
		{
			int randomEvo1 = 50 + (playerMonster[monsterID].happy * 5);
			int randomEvo2 = 50 - (playerMonster[monsterID].happy * 5);
			if (randomEvo1 >= randomEvo2)
			{
				tempMon = monsters[playerMonster[monsterID].evoTo[0]];
				tempMon.id = playerMonster[monsterID].evoTo[0];
				if (playerMonster[monsterID].isShiny)
				{
					tempMon.image = monsters[playerMonster[monsterID].evoTo[0]].shinyImage;
				}
			}
			else
			{
				tempMon = monsters[playerMonster[monsterID].evoTo[1]];
				tempMon.id = playerMonster[monsterID].evoTo[1];
				if (playerMonster[monsterID].isShiny)
				{
					tempMon.image = monsters[playerMonster[monsterID].evoTo[1]].shinyImage;
				}
			}
		}
		else if (lockDNA == 0)
		{
			tempMon = monsters[playerMonster[monsterID].evoTo[0]];
			tempMon.id = playerMonster[monsterID].evoTo[0];
			if (playerMonster[monsterID].isShiny)
			{
				tempMon.image = monsters[playerMonster[monsterID].evoTo[0]].shinyImage;
			}
		}
		else if (lockDNA == 1)
		{
			tempMon = monsters[playerMonster[monsterID].evoTo[1]];
			tempMon.id = playerMonster[monsterID].evoTo[1];
			if (playerMonster[monsterID].isShiny)
			{
				tempMon.image = monsters[playerMonster[monsterID].evoTo[1]].shinyImage;
			}
		}



		playerMonster [monsterID] = tempMon;

		writePlayerData ();
		
	}

	void RandomTopping(int monsterID)
	{
		playerMonster [monsterID].toppingID = Random.Range (0, toppingIMG.Length);
	}

	public Texture2D[] toppingIMG;
	public Vector2 iconSize = new Vector2(128, 128);
	public int battlePosSize = 500;
}