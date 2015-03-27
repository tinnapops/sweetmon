using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class Lobby : MonoBehaviour
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

			if(GUI.Button(new Rect(1024 - 200,768 - 200,200,200),"Adventure"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<StageSelect>();
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}

			if(GUI.Button(new Rect(0,768 - 100,100,100),"Shop"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<Shop>();
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}
			if(GUI.Button(new Rect(100,768 - 100,100,100),"Friend List"))
			{

			}
			if(GUI.Button(new Rect(200,768 - 100,100,100),"Monster"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<MonsterList>();
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}
			if(GUI.Button(new Rect(0,100,100,100),"Dessrt Dex"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<DessertDex>();
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}
		}
	}
}