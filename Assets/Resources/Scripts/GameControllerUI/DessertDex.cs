using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class DessertDex : MonoBehaviour
	{
		AudioSource audioSource;
		GameControl control;
		void Start()
		{
			audioSource	= gameObject.GetComponent<AudioSource>();
			control	= gameObject.GetComponent<GameControl>();
		}

		enum POPUP_STEP { None, MonsterInfo, FriendList };
		POPUP_STEP currentPopupStep = POPUP_STEP.None;

		Vector2 scrollPosition;
		int currentDexID;
		void OnGUI()
		{
			control.AutoResize();

			var monsters	= control.monsters;
			
			if(currentPopupStep == POPUP_STEP.None)//Desert Dex Select
			{
				GUI.BeginGroup(new Rect(25,75,1024 - 50,768 - 100),"");
				{
					if(GUI.Button(new Rect(1024 - 50 - 25,0,25,25),"X"))
					{
						GameObject.Destroy(this);
						gameObject.AddComponent<Lobby>();
						audioSource.PlayOneShot(control.buttonSFX[1]);
						return;
					}
					GUI.Box(new Rect(0,0,1024 - 50,768 - 100),"Sweet Recipe");

					GUI.Button(new Rect(5,25,200,100),"ID");

					scrollPosition = GUI.BeginScrollView(new Rect(5,125,950,500),scrollPosition,new Rect(0,0,950 - 25,450 * 3));
					{
						for(int i = 0;i < 3;i++)
						{
							int id = i * 6;
							GUI.BeginGroup(new Rect(0,i * 450,950,450),"0");
							{
								GUI.Box(new Rect(0,0,950,460),"monster");

								if(!monsters[id].isSeen)
								{
									GUI.color = new Color(0,0,0,1);
								}
								else
								{
									GUI.color = Color.white;
								}
								monsters[id].Draw(new Rect(50,460 / 2 - 128 / 2,128,128));




								if(!monsters[id + 1].isSeen)
								{
									GUI.color = new Color(0,0,0,1);
								}
								else
								{
									GUI.color = Color.white;
								}
								monsters[id + 1].Draw(new Rect(250,460 / 2 - 128 / 2 - (128 / 2),128,128));

								if(!monsters[id + 2].isSeen)
								{
									GUI.color = new Color(0,0,0,1);
								}
								else
								{
									GUI.color = Color.white;
								}
								monsters[id + 2].Draw(new Rect(250,460 / 2 - 128 / 2 + (128 / 2),128,128));


								if(!monsters[id + 3].isSeen)
								{
									GUI.color = new Color(0,0,0,1);
								}
								else
								{
									GUI.color = Color.white;
								}
								monsters[id + 3].Draw(new Rect(500,460 / 2 - 128 / 2 - (128),128,128));


								if(!monsters[id + 4].isSeen)
								{
									GUI.color = new Color(0,0,0,1);
								}
								else
								{
									GUI.color = Color.white;
								}
								monsters[id + 4].Draw(new Rect(500,460 / 2 - 128 / 2,128,128));

								if(!monsters[id + 5].isSeen)
								{
									GUI.color = new Color(0,0,0,1);
								}
								else
								{
									GUI.color = Color.white;
								}
								monsters[id + 5].Draw(new Rect(500,460 / 2 - 128 / 2 + (128),128,128));


								GUI.color = Color.white;

								if(GUI.Button(new Rect(750,450 / 2 -  (150 / 2) - 100,150,150),"Evolution Reward"))
								{

								}
								if(GUI.Button(new Rect(750,450 / 2 +  (150 / 2) - 50,150,150),"Discovery Reward"))
								{

								}

								GUI.color = new Color(0,0,0,0);

								if(currentPopupStep == POPUP_STEP.None)
								{
									//0
									if(GUI.Button(new Rect(50,460 / 2 - 128 / 2,128,128),""))
									{
										currentPopupStep = POPUP_STEP.MonsterInfo;
										currentDexID = id;
									}

									// 1 - 2
									if(GUI.Button(new Rect(250,460 / 2 - 128 / 2 - (128 / 2),128,128),""))
									{
										currentPopupStep = POPUP_STEP.MonsterInfo;
										currentDexID = id + 1;
									}
									if(GUI.Button(new Rect(250,460 / 2 - 128 / 2 + (128 / 2),128,128),""))
									{
										currentPopupStep = POPUP_STEP.MonsterInfo;
										currentDexID = id + 2;
									}

									//3 - 5
									if(GUI.Button(new Rect(500,460 / 2 - 128 / 2 - (128),128,128),""))
									{
										currentPopupStep = POPUP_STEP.MonsterInfo;
										currentDexID = id + 3;
									}

									if(GUI.Button(new Rect(500,460 / 2 - 128 / 2,128,128),""))
									{
										currentPopupStep = POPUP_STEP.MonsterInfo;
										currentDexID = id + 4;
									}

									if(GUI.Button(new Rect(500,460 / 2 - 128 / 2 + (128),128,128),""))
									{
										currentPopupStep = POPUP_STEP.MonsterInfo;
										currentDexID = id + 5;
									}

									if(currentPopupStep == POPUP_STEP.MonsterInfo)
										audioSource.PlayOneShot(monsters[currentDexID].Cry);
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


			if(currentPopupStep == POPUP_STEP.MonsterInfo)
			{
				GUI.Box(new Rect(0,0,1024,768),"");
				MonsterInfo(currentDexID,new Rect(1024 / 2 - 800 / 2,678/ 2 - 600 / 2,800,600));
			}
		}

		void MonsterInfo(int id,Rect rect)
		{
			var monsters	= control.monsters;

			GUI.BeginGroup(rect,"");
			{
				GUI.Box(new Rect(0,0,rect.width,rect.height),"");
				// 400 - 400

				GUI.Box(new Rect(10,35,380,50),monsters[id].name);

				if(!monsters[id].isSeen)
				{
					GUI.color = new Color(0,0,0,1);
				}
				monsters[id].Draw(new Rect(10 + 400 / 2 - 256 / 2,25 + 10 + 50 + 10,256,256));

				GUI.color = Color.white;
				GUI.Box(new Rect(10,25 + 10 + 50 + 10 + 256 + 10,380,100),"Detail");
				GUI.Box(new Rect(10,25  + 10 + 50 + 10 + 256 + 10 + 100 + 10,380,100),"Share Facebook");



				var monIndex = id;

				int hpStar,atkStar,defStar,intStar;
				monsters[monIndex].GetStar(out hpStar,out atkStar,out defStar,out intStar);

				GUI.Box(new Rect(410,35,380,25),"LV 10/10");

				GUI.Box(new Rect(410,35 + 10 + 25,380,25),"Class");

				GUI.Label(new Rect(410,35 + 10 + 25 + 10 + 25,100,25),"HP");
				for(int i = 0;i < hpStar;i++)
					GUI.Box(new Rect(485 + (i * 25),35 + 10 + 25 + 5 + 25,25,25),"*");

				GUI.Label(new Rect(410,35 + 10 + 25 + 5 + 25 + 5 + 25,100 + 25,25),"ATK");
				for(int i = 0;i < atkStar;i++)
					GUI.Box(new Rect(485 + (i * 25),35 + 10 + 25 + 5 + 25 + 5 + 25,25,25),"*");

				GUI.Label(new Rect(410,35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25,100,25),"DEF");
				for(int i = 0;i < defStar;i++)
					GUI.Box(new Rect(485 + (i * 25),35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25,25,25),"*");

				GUI.Label(new Rect(410,35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25,100,25),"INT");
				for(int i = 0;i < intStar;i++)
					GUI.Box(new Rect(485 + (i * 25),35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25,25,25),"*");

				GUI.Box(new Rect(425,35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25,75,75),"Skill Pic");
				GUI.Box(new Rect(425 + 100,35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25,200,75),"Skill Detail");

				GUI.BeginGroup(new Rect(410,35 + 10 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 25 + 5 + 75,380,275),"Evolution");
				{
					GUI.Box(new Rect(0,0,380,275),"");
					if(!monsters[id].isSeen)
					{
						GUI.color = new Color(0,0,0,1);
					}
					else
					{
						GUI.color = Color.white;
					}
					monsters[id].Draw(new Rect(10,225 / 2 - 128 / 2,128,128));
					if(monsters[id].evoTo.Length > 0)
					{
						if(!monsters[monsters[id].evoTo[0]].isSeen)
						{
							GUI.color = new Color(0,0,0,1);
						}
						else
						{
							GUI.color = Color.white;
						}
						monsters[monsters[id].evoTo[0]].Draw(new Rect(178 + 25,225 / 2 - 128,128,128));

						if(!monsters[monsters[id].evoTo[1]].isSeen)
						{
							GUI.color = new Color(0,0,0,1);
						}
						else
						{
							GUI.color = Color.white;
						}
						monsters[monsters[id].evoTo[1]].Draw(new Rect(178 + 25,225 / 2,128,128));
					}

				}
				GUI.EndGroup();
			}
			GUI.EndGroup();


			if(GUI.Button(new Rect(rect.width - 50,0,50,50),"X"))
			{
				currentPopupStep = POPUP_STEP.None;
			}
		}
	}
}