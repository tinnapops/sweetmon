using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class TeamSetup : MonoBehaviour
	{
		AudioSource audioSource;
		GameControl control;
		void Start()
		{
			audioSource	= gameObject.GetComponent<AudioSource>();
			control	= gameObject.GetComponent<GameControl>();
		}

		void OnGUI()
		{
			control.AutoResize();

			var battlePosition	= control.battlePosition;
			var battlePosSize	= control.battlePosSize;
			var CurrentQuest	= control.CurrentQuest;
			var teamSelect	= control.teamSelect;
			var monsters	= control.monsters;

			var monsterStockRect	= control.monsterStockRect;
			var playerMonster	= control.playerMonster;
			var iconSize	= control.iconSize;

			var monsterInfoRect	= control.monsterInfoRect;
			var questInfoRect	= control.questInfoRect;

			GUI.BeginGroup(battlePosition,"");
			{
				for(int i = 0;i < 7;i++)
				{
					if(i != 3)
					{
						GUI.Box(new Rect(i * battlePosSize,0,battlePosSize,battlePosSize),"");
						if(i > 3)
						{
							var monIndex	= CurrentQuest.enemy[i - 4];
							monsters[monIndex].Draw(new Rect((i * battlePosSize) + battlePosSize,0,-battlePosSize,battlePosSize));
						}
					}
					else
						GUI.Box(new Rect(i * battlePosSize,0,battlePosSize,battlePosSize),"VS");
				}

				int count	= 0;
				foreach(int index in teamSelect)
				{
					var monsterButton = new Rect((count * battlePosSize),0,battlePosSize,battlePosSize);
					monsters[index].Draw(monsterButton);
					count++;
				}
			}
			GUI.EndGroup();

			GUI.BeginGroup(monsterStockRect,"");
			{
				for(int i = 0;i < playerMonster.Length;i++)
				{
					var monsterButton = new Rect(0 + ((i%7) * iconSize.x),0 + ((i/7) * iconSize.y),iconSize.x,iconSize.y);

					var monIndex	= playerMonster[i].id;
					if(teamSelect.Contains(monIndex))
						GUI.Box(monsterButton,"SELECTED");
					monsters[monIndex].Draw(monsterButton);

					GUI.color = new Color(0,0,0,0);

					if(GUI.Button(monsterButton,""))
					{
						control.currentSelectMonsterInfo = i;
						control.AddMonsterToTeam(monIndex);

					}
					GUI.color = Color.white;
				}
			}
			GUI.EndGroup();

			GUI.BeginGroup(monsterInfoRect,"");
			{
				GUI.Box(new Rect(0,0,monsterInfoRect.width,monsterInfoRect.height),"INFO");

				var monIndex	= playerMonster[control.currentSelectMonsterInfo].id;
				GUI.Box(new Rect(0,25,monsterInfoRect.width,25),monsters[monIndex].name);

				GUI.Box(new Rect(25,55,75,75),"Skill Pic");
				GUI.Label(new Rect(125,50,100,50),monsters[monIndex].skill.name);

				int hpStar,atkStar,defStar,intStar;
				monsters[monIndex].GetStar(out hpStar,out atkStar,out defStar,out intStar);

				GUI.Label(new Rect(25,135,100,25),"HP");
				for(int i = 0;i < hpStar;i++)
					GUI.Box(new Rect(100 + (i * 25),135,25,25),"*");

				GUI.Label(new Rect(25,160,100,25),"ATK");
				for(int i = 0;i < atkStar;i++)
					GUI.Box(new Rect(100 + (i * 25),160,25,25),"*");

				GUI.Label(new Rect(25,185,100,25),"DEF");
				for(int i = 0;i < defStar;i++)
					GUI.Box(new Rect(100 + (i * 25),185,25,25),"*");

				GUI.Label(new Rect(25,210,100,25),"INT");
				for(int i = 0;i < intStar;i++)
					GUI.Box(new Rect(100 + (i * 25),210,25,25),"*");

				GUI.Box(new Rect(350,60,50,25),"LV");
				GUI.Box(new Rect(350 + 50,60,250,25),"EXP 0/100");
				GUI.Box(new Rect(350,60 + 30,50,25),"FLV");
				GUI.Box(new Rect(350 + 50,60 + 30,250,25),"FULLNESS 0/100");
				GUI.Box(new Rect(350,130,100,100),"EVO BUTTON\nMax LV\nMax FULL");
				GUI.Box(new Rect(350 + 105,130,100,100),"SELL");
				GUI.Box(new Rect(350 + 105 + 105,130,100,100),"TOPPING\n(Coming Soon)");
			}
			GUI.EndGroup();

			GUI.BeginGroup(questInfoRect,"");
			{
				GUI.Box(new Rect(0,0,questInfoRect.width,questInfoRect.height),"Quest Detail");

				GUI.Box(new Rect(10,35,questInfoRect.width - 20,50),CurrentQuest.name);
				GUI.Box(new Rect(10,90,questInfoRect.width - 20,100),"Quest Award");

				if(teamSelect.Count == 3)
				{
					if(GUI.Button(new Rect(10,200,questInfoRect.width - 20,40),"GO"))
					{
						GameObject.Destroy(this);
						control.battleOBJ.gameObject.SetActive(true);

						audioSource.PlayOneShot(control.buttonSFX[0]);
						audioSource.Stop();
						audioSource.clip = control.bgm[1];
						audioSource.volume = 0.3F;
						audioSource.Play();
					}
				}
				else
				{
					GUI.Box(new Rect(10,200,questInfoRect.width - 20,40),"SELECT " +  teamSelect.Count + "/3 ");
				}


			}
			GUI.EndGroup();
		}
	}
}