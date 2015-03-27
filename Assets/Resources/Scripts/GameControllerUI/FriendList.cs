using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class FriendList : MonoBehaviour
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
			GUI.BeginGroup (new Rect (1024 / 2 - 300, 768 / 2 - 300, 600, 600));
			{
				GUI.Box(new Rect (0, 0, 600, 600), "Friend List\nBreeding with Friend Have High Chance to get Shiny Monster (Or Pregnant)!!");

				control.monsters[3].Draw(new Rect(5, 50, 64, 64));

				if (control.playerMonster.Length > 0)
				{
					if (GUI.Button(new Rect(200, 50, 400, 64), "Thaina Walking Pedia"))
					{
						control.Breed(0, 3, false);
					}
					control.monsters[17].Draw(new Rect(5, 50 + 64, 64, 64));
					if (GUI.Button(new Rect(200, 50 + 64, 400, 64), "Yah jung cupp"))
					{
						control.Breed(0, 17, false);
					}
					control.monsters[16].Draw(new Rect(5, 50 + (64 * 2), 64, 64));
					if (GUI.Button(new Rect(200, 50 + (64 * 2), 400, 64), "Satobas"))
					{
						control.Breed(0, 16, false);
					}
					control.monsters[9].Draw(new Rect(5, 50 + (64 * 3), 64, 64));
					if (GUI.Button(new Rect(200, 50 + (64 * 3), 400, 64), "Om No Lia"))
					{
						control.Breed(0, 9, false);
					}
					control.monsters[10].Draw(new Rect(5, 50 + (64 * 4), 64, 64));
					if (GUI.Button(new Rect(200, 50 + (64 * 4), 400, 64), "Chain Frost"))
					{
						control.Breed(0, 10, false);
					}
					control.monsters[11].Draw(new Rect(5, 50 + (64 * 5), 64, 64));
					if (GUI.Button(new Rect(200, 50 + (64 * 5), 400, 64), "Peach Yik Yik"))
					{
						control.Breed(0, 11, false);
					}
					control.monsters[15].Draw(new Rect(5, 50 + (64 * 6), 64, 64));
					if (GUI.Button(new Rect(200, 50 + (64 * 6), 400, 64), "Burst Thunger Lazer God"))
					{
						control.Breed(0, 15, false);
					}
					control.monsters[5].Draw(new Rect(5, 50 + (64 * 7), 64, 64));
					if (GUI.Button(new Rect(200, 50 + (64 * 7), 400, 64), "Kai Dancing Machine"))
					{
						control.Breed(0, 5, false);
					}
				}
				else
				{
					GUI.Box (new Rect(5, 50, 400, 64 * 8), "You Have No Monster");
				}

			}
			GUI.EndGroup ();

			if(GUI.Button(new Rect(1024 - 200, 150,25,25),"X"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<Lobby>();
				audioSource.PlayOneShot(control.buttonSFX[1]);
			}
		}
	}
}