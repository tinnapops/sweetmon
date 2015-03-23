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
	public Rect FeedingPosition;

	public Rect monsterStockRect;
	public Rect monsterListStockRect;

	public Rect monsterInfoRect;

	public Rect questInfoRect;

	public enum UI_STEP{Title, Lobby, StageSelect, TeamSetup, Battle, Shop, DessertDex, Topping, Evolution, MonsterList};
	public enum POPUP_STEP{None, MonsterInfo, FriendList};

	public UI_STEP currentStep = UI_STEP.Title;
	public POPUP_STEP currentPopupStep = POPUP_STEP.None;
	public bool isMonsterInfoPopup = false;

	public Texture2D startScreen;

	public Vector2 scrollPosition;

	int currentDexID = 0;

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
				currentStep = UI_STEP.MonsterList;
			}
			if (GUI.Button(new Rect(0, 100, 100, 100), "Dessrt Dex"))
			{
				currentStep = UI_STEP.DessertDex;
			}
		}
		else if (currentStep == UI_STEP.DessertDex && currentPopupStep == POPUP_STEP.None)//Desert Dex Select
		{
			GUI.BeginGroup(new Rect(25, 75, 1024 - 50, 768 - 100), "");
			{
				if (GUI.Button(new Rect(1024 - 50 - 25, 0, 25, 25), "X"))
				{
					currentStep = UI_STEP.Lobby;
					currentPopupStep = POPUP_STEP.None;
				}
				GUI.Box(new Rect(0, 0 , 1024 - 50, 768 - 100), "Sweet Recipe");
				
				GUI.Button(new Rect(5 , 25, 200, 100), "ID");
				
				scrollPosition = GUI.BeginScrollView(new Rect(5, 125, 950, 500), scrollPosition, new Rect(0, 0, 950 - 25, 450 * 3));
				{
					for (int i = 0; i < 3;i++)
					{
						int id = i * 6;
						GUI.BeginGroup(new Rect(0 , i * 450, 950, 450), "0");
						{
							GUI.Box(new Rect(0, 0, 950, 460), "monster");

							if (!monsters[id].isSeen)
							{
								GUI.color = new Color(0, 0, 0, 1);
							}
							else
							{
								GUI.color = Color.white;
							}
							monsters[id].Draw(new Rect(50, 460 / 2 - 128 / 2, 128, 128));




							if (!monsters[id + 1].isSeen)
							{
								GUI.color = new Color(0, 0, 0, 1);
							}
							else
							{
								GUI.color = Color.white;
							}
							monsters[id + 1].Draw(new Rect(250, 460 / 2 - 128 / 2 - (128 / 2), 128, 128));

							if (!monsters[id + 2].isSeen)
							{
								GUI.color = new Color(0, 0, 0, 1);
							}
							else
							{
								GUI.color = Color.white;
							}
							monsters[id + 2].Draw(new Rect(250, 460 / 2 - 128 / 2 + (128 / 2), 128, 128));


							if (!monsters[id + 3].isSeen)
							{
								GUI.color = new Color(0, 0, 0, 1);
							}
							else
							{
								GUI.color = Color.white;
							}
							monsters[id + 3].Draw(new Rect(500, 460 / 2 - 128 / 2 - (128), 128, 128));


							if (!monsters[id + 4].isSeen)
							{
								GUI.color = new Color(0, 0, 0, 1);
							}
							else
							{
								GUI.color = Color.white;
							}
							monsters[id + 4].Draw(new Rect(500, 460 / 2 - 128 / 2, 128, 128));

							if (!monsters[id + 5].isSeen)
							{
								GUI.color = new Color(0, 0, 0, 1);
							}
							else
							{
								GUI.color = Color.white;
							}
							monsters[id + 5].Draw(new Rect(500, 460 / 2 - 128 / 2 + (128), 128, 128));


							GUI.color = Color.white;

							if (GUI.Button(new Rect(750, 450 / 2 -  (150 / 2) - 100 , 150, 150), "Evolution Reward"))
							{
								
							}
							if (GUI.Button(new Rect(750, 450 / 2 +  (150 / 2) - 50, 150, 150), "Discovery Reward"))
							{
								
							}

							GUI.color = new Color(0, 0, 0, 0);

							if (currentPopupStep == POPUP_STEP.None)
							{
								//0
								if (GUI.Button(new Rect(50, 460 / 2 - 128 / 2, 128, 128), ""))
								{
									currentPopupStep = POPUP_STEP.MonsterInfo;
									currentDexID = id;
								}
								
								// 1 - 2
								if (GUI.Button(new Rect(250, 460 / 2 - 128 / 2 - (128 / 2), 128, 128), ""))
								{
									currentPopupStep = POPUP_STEP.MonsterInfo;
									currentDexID = id + 1;
								}
								if (GUI.Button(new Rect(250, 460 / 2 - 128 / 2 + (128 / 2), 128, 128), ""))
								{
									currentPopupStep = POPUP_STEP.MonsterInfo;
									currentDexID = id + 2;
								}
								
								//3 - 5
								if (GUI.Button(new Rect(500, 460 / 2 - 128 / 2 - (128), 128, 128), ""))
								{
									currentPopupStep = POPUP_STEP.MonsterInfo;
									currentDexID = id + 3;
								}
								
								if (GUI.Button(new Rect(500, 460 / 2 - 128 / 2, 128, 128), ""))
								{
									currentPopupStep = POPUP_STEP.MonsterInfo;
									currentDexID = id + 4;
								}
								
								if (GUI.Button(new Rect(500, 460 / 2 - 128 / 2 + (128), 128, 128), ""))
								{
									currentPopupStep = POPUP_STEP.MonsterInfo;
									currentDexID = id + 5;
								}
							}
							GUI.color = Color.white;
						}
						GUI.EndGroup();
					}

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
		}
		else if (currentStep == UI_STEP.MonsterList)//Feeding Evolutioning
		{
			GUI.BeginGroup (FeedingPosition, "");
			{
				if (GUI.Button(new Rect(FeedingPosition.width - 25, 0 , 25, 25), "X"))
				{
					currentStep = UI_STEP.Lobby;
				}
				GUI.Box(new Rect(0,0, FeedingPosition.width, monsterInfoRect.height), "INFO");
				
				var monIndex	= playerMonster[currentSelectMonsterInfo];
				GUI.Box(new Rect(25 , 25, 256, 25), monsters[monIndex].name);
				GUI.Box(new Rect(5 + 256, 160, 75, 75), "TOPPING");

				
				GUI.Box(new Rect(350, 130, 75, 75), "Skill Pic");
				GUI.Box(new Rect(350, 130 + 80, 75, 25),skills[monIndex].name);
				
				int hpStar,atkStar,defStar,intStar;
				monsters[monIndex].GetStar(out hpStar,out atkStar,out defStar,out intStar);
				
				GUI.Label(new Rect(25 + 430, 135, 100, 25), "HP");
				for(int i = 0;i < hpStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430, 135 , 25, 25), "*");
				
				GUI.Label(new Rect(25 + 430, 160, 100, 25), "ATK");
				for(int i = 0;i < atkStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430, 160 , 25, 25), "*");
				
				GUI.Label(new Rect(25 + 430, 185, 100, 25), "DEF");
				for(int i = 0;i < defStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430, 185 , 25, 25), "*");
				
				GUI.Label(new Rect(25 + 430, 210, 100, 25), "INT");
				for(int i = 0;i < intStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430, 210 , 25, 25), "*");


				monsters[monIndex].Draw(new Rect(5, 5 , 256, 256));

				GUI.Box(new Rect(350 , 60, 50, 25), "LV");
				GUI.Box(new Rect(350 + 50, 60, 250, 25), "EXP 0/100");
				GUI.Box(new Rect(350 , 60 + 30, 50, 25), "FLV");
				GUI.Box(new Rect(350 + 50, 60 + 30, 250, 25), "FULLNESS 100/100");

				GUI.Box(new Rect(350 + 200 + 105, 180, 300, 50), "Evolution");

				GUI.Box(new Rect(350 + 200 + 105, 180 - 55, 145, 50), "50 %\nDNA LOCK");
				GUI.Box(new Rect(350 + 200 + 105 + 155, 180 - 55, 145, 50), "50 %\nDNA LOCK");


				if (monsters[monIndex].evoTo.Length > 0)
				{
					if (!monsters[monsters[monIndex].evoTo[0]].isSeen)
					{
						GUI.color = new Color(0,0,0,1);
					}
					monsters[monsters[monIndex].evoTo[0]].Draw(new Rect(350 + 200 + 125, 180 - 55 - 105, 100, 100));
					if (!monsters[monsters[monIndex].evoTo[1]].isSeen)
					{
						GUI.color = new Color(0,0,0,1);
					}
					monsters[monsters[monIndex].evoTo[1]].Draw(new Rect(350 + 200 + 125 + 155, 180 - 55 - 105, 100, 100));
					GUI.color = Color.white;
				}


			}
			GUI.EndGroup ();
			
			GUI.BeginGroup (monsterListStockRect, "");
			{
				GUI.Box(new Rect(0, 0 , monsterListStockRect.width, monsterListStockRect.height), "");
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
					}
					GUI.color = Color.white;
				}
			}
			GUI.EndGroup ();
		}

		if (currentPopupStep == POPUP_STEP.MonsterInfo)
		{
			GUI.Box(new Rect(0, 0, 1024, 768), "");
			MonsterInfo(currentDexID, new Rect(1024 / 2 - 800 / 2, 678/ 2 - 600 / 2, 800, 600));
		}
	}

	bool feedingMode = false;

	void MonsterInfo(int id, Rect rect)
	{
		GUI.BeginGroup (rect, "");
		{
			GUI.Box(new Rect(0, 0, rect.width, rect.height), "");
			// 400 - 400

			GUI.Box(new Rect(10, 35, 380, 50), monsters[id].name);

			if (!monsters[id].isSeen)
			{
				GUI.color = new Color(0,0,0,1);
			}
			monsters[id].Draw(new Rect(10 + 400 / 2 - 256 / 2, 25 + 10 + 50 + 10 , 256, 256));

			GUI.color = Color.white;
			GUI.Box(new Rect(10, 25 + 10 + 50 + 10 + 256 + 10, 380, 100), "Detail");
			GUI.Box(new Rect(10, 25  + 10 + 50 + 10 + 256 + 10 + 100 + 10, 380, 100), "Share Facebook");


			
			var monIndex = id;
			
			int hpStar,atkStar,defStar,intStar;
			monsters[monIndex].GetStar(out hpStar,out atkStar,out defStar,out intStar);

			GUI.Box(new Rect(410, 35, 380, 25), "LV 10/10");

			GUI.Box(new Rect(410, 35 + 10 + 25, 380, 25), "Class");

			GUI.Label(new Rect(410, 35 + 10 + 25 + 10 + 25, 100, 25), "HP");
			for(int i = 0;i < hpStar;i++)
				GUI.Box(new Rect(485 + (i * 25), 35 + 10 + 25 + 5 + 25, 25, 25), "*");
			
			GUI.Label(new Rect(410, 35 + 10 + 25 + 5 + 25 + 5 + 25, 100 + 25, 25), "ATK");
			for(int i = 0;i < atkStar;i++)
				GUI.Box(new Rect(485 + (i * 25), 35 + 10 + 25 + 5 + 25 + 5 + 25, 25, 25), "*");
			
			GUI.Label(new Rect(410, 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25, 100, 25), "DEF");
			for(int i = 0;i < defStar;i++)
				GUI.Box(new Rect(485 + (i * 25), 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25, 25, 25), "*");
			
			GUI.Label(new Rect(410, 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25, 100, 25), "INT");
			for(int i = 0;i < intStar;i++)
				GUI.Box(new Rect(485 + (i * 25), 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25, 25, 25), "*");

			GUI.Box(new Rect(425, 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25, 75, 75), "Skill Pic");
			GUI.Box(new Rect(425 + 100, 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25, 200, 75), "Skill Detail");

			GUI.BeginGroup(new Rect(410, 35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 75, 380, 275), "Evolution");
			{
				GUI.Box(new Rect(0,0, 380, 275), "");
				if (!monsters[id].isSeen)
				{
					GUI.color = new Color(0, 0, 0, 1);
				}
				else
				{
					GUI.color = Color.white;
				}
				monsters[id].Draw(new Rect(10, 225 / 2 - 128 / 2, 128, 128));
				if (monsters[id].evoTo.Length > 0)
				{
					if (!monsters[monsters[id].evoTo[0]].isSeen)
					{
						GUI.color = new Color(0, 0, 0, 1);
					}
					else
					{
						GUI.color = Color.white;
					}
					monsters[monsters[id].evoTo[0]].Draw(new Rect(178 + 25, 225 / 2 - 128, 128, 128));
					
					if (!monsters[monsters[id].evoTo[1]].isSeen)
					{
						GUI.color = new Color(0, 0, 0, 1);
					}
					else
					{
						GUI.color = Color.white;
					}
			         monsters[monsters[id].evoTo[1]].Draw(new Rect(178 + 25, 225 / 2, 128, 128));
				}

			}
			GUI.EndGroup();

			if (GUI.Button(new Rect(rect.width - 50, 0, 50, 50), "X"))
			{
				currentPopupStep = POPUP_STEP.None;
			}
		}
		GUI.EndGroup ();
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