using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class GameControl : MonoBehaviour
{
	BattleController battleOBJ;
	void Start() 
	{
		var audioSource	= gameObject.GetComponent<AudioSource>();
		audioSource.Stop();
		audioSource.clip = bgm[0];
		audioSource.Play();

		battleOBJ	= gameObject.transform.GetChild(0).GetComponent<BattleController>();

		int i	= 0;
		while(i < monsters.Length)
		{
			monsters[i].skill.displayName	= "Sugar Punch";
			monsters[i].skill.name	= "S_ATK";
			monsters[i].skill.desc	= "...";
			monsters[i].skill.multiply	= 3;
			monsters[i].skill.target	= 3;
			i++;
		}
	}

	public AudioClip[] bgm;

	public AudioClip[] buttonSFX;

	
	public int[] playerMonster;

	public MonsterData[] monsters;
	public Quest[] quest;

	int currentQuestSelect = 0;
	public Quest CurrentQuest
	{
		get { return quest[currentQuestSelect]; }
	}

	public List<int> teamSelect = new List<int> ();

	public Rect statusPosition;

	public Rect battlePosition;

	public Rect monsterStockRect;

	public Rect monsterInfoRect;

	public Rect questInfoRect;

	public enum UI_STEP{Title, Lobby, StageSelect, TeamSetup, Battle, Shop, DessertDex, Topping, Evolution};
	public enum POPUP_STEP{None, MonsterInfo, FriendList};

	public UI_STEP currentStep = UI_STEP.Title;
	public POPUP_STEP currentPopupStep = POPUP_STEP.None;
	public bool isMonsterInfoPopup = false;

	public Texture2D startScreen;

	public Vector2 scrollPosition;

	void OnGUI()
	{
		UI.AutoResize (1024, 768);
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

		if(currentStep == UI_STEP.Battle)
			currentStep	= UI_STEP.Lobby;

		var audioSource	= gameObject.GetComponent<AudioSource>();

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
		

		//
	

		if(currentStep == UI_STEP.Title)//TiTle Screen
		{
			GUI.DrawTexture(new Rect(0,0,1024,768), startScreen);
			GUI.color = new Color(0,0,0,0);
			if (GUI.Button(new Rect(1024/2 - 250, 768/2 - 250, 1024, 768), "Tap To Start"))
			{
				currentStep = UI_STEP.Lobby;
				audioSource.PlayOneShot(buttonSFX[0]);
			}
			GUI.color = Color.white;
		}
		else if (currentStep == UI_STEP.Lobby)//Lobby
		{
			if (GUI.Button(new Rect(1024 - 200, 768 - 200, 200, 200), "Adventure"))
			{
				currentStep = UI_STEP.StageSelect; 
				audioSource.PlayOneShot(buttonSFX[0]);
			}

			if (GUI.Button(new Rect(0, 768 - 100, 100, 100), "Shop"))
			{
				
			}
			if (GUI.Button(new Rect(100, 768 - 100, 100, 100), "Friend List"))
			{
				
			}
			if (GUI.Button(new Rect(200, 768 - 100, 100, 100), "Monster"))
			{
				
			}
			if (GUI.Button(new Rect(0, 100, 100, 100), "Dessrt Dex"))
			{
				currentStep = UI_STEP.DessertDex;
			}
		}
		else if (currentStep == UI_STEP.DessertDex)//Desert Dex Select
		{

			GUI.BeginGroup(new Rect(25, 75, 1024 - 50, 768 - 100), "");
			{
				GUI.Box(new Rect(0, 0 , 1024 - 50, 768 - 100), "Sweet Recipe");

				GUI.Button(new Rect(5 , 25, 200, 100), "ID");

				scrollPosition = GUI.BeginScrollView(new Rect(5, 125, 950, 500), scrollPosition, new Rect(0, 0, 950 - 25, 1000));
				{
					GUI.Button(new Rect(0 , 0, 950, 150), "0");
				}
				GUI.EndScrollView();
			}
			GUI.EndGroup();
		}
		else if (currentStep == UI_STEP.StageSelect)//Stage Select
		{
			for (int i = 0;i < quest.Length;i++)
			{
				if (GUI.Button(new Rect(300, 100 + (i * 100), 400, 100), "STAGE : " + i + " " + quest[i].name))
				{
					currentStep = UI_STEP.TeamSetup;
					currentQuestSelect = i;
					audioSource.PlayOneShot(buttonSFX[0]);
				}
			}
		}
		else if (currentStep == UI_STEP.TeamSetup)//Team Setup
		{
			GUI.BeginGroup (battlePosition, "");
			{
				for (int i = 0; i < 7;i++)
				{
					if(i != 3)
					{
						GUI.Box(new Rect(i * battlePosSize, 0, battlePosSize, battlePosSize), "");
						if (i > 3)
						{
							var monIndex	= CurrentQuest.enemy[i - 4];
							monsters[monIndex].Draw(new Rect((i * battlePosSize) + battlePosSize,0,-battlePosSize,battlePosSize));
						}
					}
					else GUI.Box(new Rect(i * battlePosSize,0,battlePosSize,battlePosSize),"VS");
				}

				int count	= 0;
				foreach(int index in teamSelect)
				{
					var monsterButton = new Rect((count * battlePosSize),0,battlePosSize,battlePosSize);
					monsters[index].Draw(monsterButton);
					count++;
				}
			}
			GUI.EndGroup ();

			GUI.BeginGroup (monsterStockRect, "");
			{
				for (int i = 0; i < playerMonster.Length;i ++)
				{
					var monsterButton = new Rect(0 + ((i%7) * iconSize.x), 0 + ((i/7) * iconSize.y), iconSize.x ,iconSize.y);
					
					var monIndex	= playerMonster[i];
					if(teamSelect.Contains(monIndex))
						GUI.Box(monsterButton,"SELECTED");
					monsters[playerMonster[i]].Draw(monsterButton);
						
					GUI.color = new Color(0,0,0,0);
						
					if (GUI.Button(monsterButton, ""))
					{
						currentSelectMonsterInfo = i;
						AddMonsterToTeam(monIndex);
							
					}
					GUI.color = Color.white;
				}
			}
			GUI.EndGroup ();

			GUI.BeginGroup (monsterInfoRect, "");
			{
				GUI.Box(new Rect(0,0, monsterInfoRect.width, monsterInfoRect.height), "INFO");

				var monIndex	= playerMonster[currentSelectMonsterInfo];
				GUI.Box(new Rect(0, 25, monsterInfoRect.width, 25), monsters[monIndex].name);
					
				GUI.Box(new Rect(25, 55, 75, 75), "Skill Pic");
				GUI.Label(new Rect(125,50,100,50),monsters[monIndex].skill.name);

				int hpStar,atkStar,defStar,intStar;
				monsters[monIndex].GetStar(out hpStar,out atkStar,out defStar,out intStar);

				GUI.Label(new Rect(25, 135, 100, 25), "HP");
				for(int i = 0;i < hpStar;i++)
					GUI.Box(new Rect(100 + (i * 25), 135 , 25, 25), "*");
				
				GUI.Label(new Rect(25, 160, 100, 25), "ATK");
				for(int i = 0;i < atkStar;i++)
					GUI.Box(new Rect(100 + (i * 25), 160 , 25, 25), "*");
	
				GUI.Label(new Rect(25, 185, 100, 25), "DEF");
				for(int i = 0;i < defStar;i++)
					GUI.Box(new Rect(100 + (i * 25), 185 , 25, 25), "*");

				GUI.Label(new Rect(25, 210, 100, 25), "INT");
				for(int i = 0;i < intStar;i++)
					GUI.Box(new Rect(100 + (i * 25), 210 , 25, 25), "*");
					
				GUI.Box(new Rect(350 , 60, 50, 25), "LV");
				GUI.Box(new Rect(350 + 50, 60, 250, 25), "EXP 0/100");
				GUI.Box(new Rect(350 , 60 + 30, 50, 25), "FLV");
				GUI.Box(new Rect(350 + 50, 60 + 30, 250, 25), "FULLNESS 0/100");
				GUI.Box(new Rect(350, 130, 100, 100), "EVO BUTTON\nMax LV\nMax FULL");
				GUI.Box(new Rect(350 + 105, 130, 100, 100), "SELL");
				GUI.Box(new Rect(350 + 105 + 105, 130, 100, 100), "TOPPING\n(Coming Soon)");
			}
			GUI.EndGroup ();
				
			GUI.BeginGroup (questInfoRect, "");
			{
				GUI.Box (new Rect (0, 0, questInfoRect.width, questInfoRect.height), "Quest Detail");
					
				GUI.Box(new Rect(10, 35, questInfoRect.width - 20, 50), CurrentQuest.name);
				GUI.Box(new Rect(10, 90, questInfoRect.width - 20, 100), "Quest Award");
					
				if (teamSelect.Count == 3)
				{
					if (GUI.Button(new Rect(10 , 200, questInfoRect.width - 20, 40), "GO"))
					{
						currentStep = UI_STEP.Battle;
						battleOBJ.gameObject.SetActive(true);

						audioSource.PlayOneShot(buttonSFX[0]);
						audioSource.Stop();
						audioSource.clip = bgm[1];
						audioSource.volume = 0.3F;
						audioSource.Play();
					}
				}
				else
				{
					GUI.Box(new Rect(10 , 200, questInfoRect.width - 20, 40), "SELECT " +  teamSelect.Count + "/3 ");
				}
					
					
			}
			GUI.EndGroup ();

			if (isMonsterInfoPopup)
			{

			}
		}
	}

	private int currentSelectMonsterInfo = 0;
	void AddMonsterToTeam(int id)
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
		SortTeam ();
	}


	void SortTeam()
	{
	}

	public Vector2 iconSize = new Vector2(128, 128);
	public int battlePosSize = 500;
}