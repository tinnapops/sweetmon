using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class Shop : MonoBehaviour
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

			if (GUI.Button(new Rect(1024 / 2 - 300, 768 /2 - 100, 200, 200),"Regular Gatcha"))
			{
				control.MonsterGachapon(-1);
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}
			if (GUI.Button(new Rect(1024 / 2 + 100, 768 /2 - 100, 200, 200),"Shiny Gatcha\nPLUS 5 % Shiny Chance"))
			{
				control.MonsterGachapon(-2);
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}

			if(GUI.Button(new Rect(1024 - 200, 150,25,25),"X"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<Lobby>();
				audioSource.PlayOneShot(control.buttonSFX[1]);
			}
		}
	}
}