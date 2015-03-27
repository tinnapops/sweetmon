using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class MonsterList : MonoBehaviour
	{

		AudioSource audioSource;
		GameControl control;
		void Start()
		{
			audioSource	= gameObject.GetComponent<AudioSource>();
			control	= gameObject.GetComponent<GameControl>();
		}

		public List<int> foodSelect = new List<int>();
		bool feedingMode = false;
		void OnGUI()
		{
			control.AutoResize();

			var FeedingPosition	= control.FeedingPosition;
			var monsterInfoRect	= control.monsterInfoRect;
			var playerMonster	= control.playerMonster;
			var monsters	= control.monsters;

			var monsterListStockRect	= control.monsterListStockRect;
			var teamSelect	= control.teamSelect;
			var iconSize	= control.iconSize;

			GUI.BeginGroup(FeedingPosition,"");
			{
				if(GUI.Button(new Rect(FeedingPosition.width - 25,0,25,25),"X"))
				{
					GameObject.Destroy(this);
					gameObject.AddComponent<Lobby>();
				}

				GUI.Box(new Rect(0,0,FeedingPosition.width,monsterInfoRect.height),"INFO");

				var monIndex	= playerMonster[control.currentSelectMonsterInfo].id;
				GUI.Box(new Rect(25,25,256,25),monsters[monIndex].name);
				GUI.Box(new Rect(5 + 256,160,75,75),"TOPPING");


				GUI.Box(new Rect(350,130,75,75),"Skill Pic");
				GUI.Box(new Rect(350,130 + 80,75,25),monsters[monIndex].skill.name);

				int hpStar,atkStar,defStar,intStar;
				monsters[monIndex].GetStar(out hpStar,out atkStar,out defStar,out intStar);

				GUI.Label(new Rect(25 + 430,135,100,25),"HP");
				for(int i = 0;i < hpStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430,135,25,25),"*");

				GUI.Label(new Rect(25 + 430,160,100,25),"ATK");
				for(int i = 0;i < atkStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430,160,25,25),"*");

				GUI.Label(new Rect(25 + 430,185,100,25),"DEF");
				for(int i = 0;i < defStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430,185,25,25),"*");

				GUI.Label(new Rect(25 + 430,210,100,25),"INT");
				for(int i = 0;i < intStar;i++)
					GUI.Box(new Rect(100 + (i * 25) + 430,210,25,25),"*");


				playerMonster[control.currentSelectMonsterInfo].Draw(new Rect(5,5,256,256));
				if (playerMonster.Length > 3)
				{
					if (GUI.Button(new Rect(5 + 256, 75, 75, 75), "Feed Mode"))
					{
						feedingMode = !feedingMode;
						foodSelect.Clear();
						
					}
				}
				else
				{
					GUI.Box(new Rect(5 + 256, 75, 75, 75), "Need 3+\nMON\nto\nFeed");
				}

				GUI.Box(new Rect(350,60,50,25),"LV");
				GUI.Box(new Rect(350 + 50,60,250,25),"EXP" + playerMonster[control.currentSelectMonsterInfo].exp +  "/" + (100 * (playerMonster[control.currentSelectMonsterInfo].level + 1)));
				GUI.Box(new Rect(350,60 + 30,50,25),"FLV");
				GUI.Box(new Rect(350 + 50,60 + 30,250,25),"FULLNESS " + playerMonster[control.currentSelectMonsterInfo].full +  "/" + (100 * (playerMonster[control.currentSelectMonsterInfo].level + 1)));

				if (playerMonster[control.currentSelectMonsterInfo].evoTo.Length > 0 && playerMonster[control.currentSelectMonsterInfo].exp >= 100 && playerMonster[control.currentSelectMonsterInfo].full >= 100)
				{
					if (GUI.Button(new Rect(350 + 200 + 105,180,300,50),"Evolution"))
					{
						control.Evolution(control.currentSelectMonsterInfo, -1);
					}
					
					int evoCal = (50 + playerMonster[control.currentSelectMonsterInfo].happy * 5);
					if (GUI.Button(new Rect(350 + 200 + 105,180 - 55,145,50), evoCal + " %\nDNA LOCK"))
					{
						control.Evolution(control.currentSelectMonsterInfo, 0);
					}
					if (GUI.Button(new Rect(350 + 200 + 105 + 155,180 - 55,145,50), (100 - evoCal) + " %\nDNA LOCK"))
					{
						control.Evolution(control.currentSelectMonsterInfo, 1);
					}
					
					

				}
				else
				{
					GUI.Box(new Rect(350 + 200 + 105,180 - 55,300, 100),"You Need to Max \nLevel and Fullness to Evolution");
				}

				if(monsters[monIndex].evoTo.Length > 0)
				{
					if(!monsters[monsters[monIndex].evoTo[0]].isSeen)
					{
						GUI.color = new Color(0,0,0,1);
					}
					monsters[monsters[monIndex].evoTo[0]].Draw(new Rect(350 + 200 + 125,180 - 55 - 105,100,100));
					if(!monsters[monsters[monIndex].evoTo[1]].isSeen)
					{
						GUI.color = new Color(0,0,0,1);
					}
					monsters[monsters[monIndex].evoTo[1]].Draw(new Rect(350 + 200 + 125 + 155,180 - 55 - 105,100,100));
					GUI.color = Color.white;
				}
			}
			GUI.EndGroup();

			GUI.BeginGroup(monsterListStockRect,"");
			{
				GUI.Box(new Rect(0,0,monsterListStockRect.width,monsterListStockRect.height),"");
				for(int i = 0;i < playerMonster.Length;i++)
				{
					var monsterButton = new Rect(0 + ((i%7) * iconSize.x),0 + ((i/7) * iconSize.y),iconSize.x,iconSize.y);

					var monIndex	= playerMonster[i].id;
					if(foodSelect.Contains(i))
						GUI.Box(monsterButton,"Food");

					if(i == control.currentSelectMonsterInfo)
						GUI.Box(monsterButton,"SELECTED");
					playerMonster[i].Draw(monsterButton);

					GUI.color = new Color(0,0,0,0);

					if(feedingMode)
 					{
						if(GUI.Button(monsterButton,"") && control.currentSelectMonsterInfo != i && playerMonster.Length - foodSelect.Count > 3)
						{
							if(!foodSelect.Remove(i))
								foodSelect.Add(i);
						}
					}
					else if(GUI.Button(monsterButton,""))
						control.currentSelectMonsterInfo = i;

					GUI.color = Color.white;
				}
			}
			GUI.EndGroup();

			if (feedingMode)
			{
				if (GUI.Button(new Rect(1024 / 2 - 250, 768 - 100, 200, 100), "OK"))
				{
					for (int i = 0; i < foodSelect.Count;i++)
					{
						playerMonster[control.currentSelectMonsterInfo].full += (25 * (playerMonster[foodSelect[i]].level + 1));
						if (monsters[playerMonster[control.currentSelectMonsterInfo].id].like == playerMonster[foodSelect[i]].taste)
						{
							playerMonster[control.currentSelectMonsterInfo].happy++;						
						}
						else
						{
							playerMonster[control.currentSelectMonsterInfo].happy--;
						}
						playerMonster[control.currentSelectMonsterInfo].happy = Mathf.Clamp(playerMonster[control.currentSelectMonsterInfo].happy, -5, 5);
						playerMonster[control.currentSelectMonsterInfo].full = Mathf.Clamp(playerMonster[control.currentSelectMonsterInfo].full, 0, 100);

						playerMonster[control.currentSelectMonsterInfo].exp += 50;
					}
					control.playerMonster = playerMonster.Where((mon) => !foodSelect.Select((index) => playerMonster[index].id).Contains(mon.id)).ToArray();
					foodSelect.Clear();
					control.currentSelectMonsterInfo = 0;// ** Hack 02
					feedingMode = false;

					control.writePlayerData();

				}
				if (GUI.Button(new Rect(1024 / 2 + 150, 768 - 100, 200, 100), "Cancle"))
				{
					feedingMode = false;
					foodSelect.Clear();
				}
			}

		}
	}
}