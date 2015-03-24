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

				var monIndex	= playerMonster[control.currentSelectMonsterInfo];
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


				monsters[monIndex].Draw(new Rect(5,5,256,256));

				GUI.Box(new Rect(350,60,50,25),"LV");
				GUI.Box(new Rect(350 + 50,60,250,25),"EXP 0/100");
				GUI.Box(new Rect(350,60 + 30,50,25),"FLV");
				GUI.Box(new Rect(350 + 50,60 + 30,250,25),"FULLNESS 100/100");

				GUI.Box(new Rect(350 + 200 + 105,180,300,50),"Evolution");

				GUI.Box(new Rect(350 + 200 + 105,180 - 55,145,50),"50 %\nDNA LOCK");
				GUI.Box(new Rect(350 + 200 + 105 + 155,180 - 55,145,50),"50 %\nDNA LOCK");


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

					var monIndex	= playerMonster[i];
					if(teamSelect.Contains(monIndex))
						GUI.Box(monsterButton,"SELECTED");
					monsters[playerMonster[i]].Draw(monsterButton);

					GUI.color = new Color(0,0,0,0);

					if(GUI.Button(monsterButton,""))
					{
						control.currentSelectMonsterInfo = i;
					}
					GUI.color = Color.white;
				}
			}
			GUI.EndGroup();
		}
	}
}