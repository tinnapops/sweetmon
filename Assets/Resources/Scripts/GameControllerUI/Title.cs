using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class Title : MonoBehaviour
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
			UI.AutoResize(1024,768);

			GUI.DrawTexture(new Rect(0,0,1024,768),control.startScreen);
			GUI.color = new Color(0,0,0,0);
			if(GUI.Button(new Rect(1024/2 - 250,768/2 - 250,1024,768),"Tap To Start"))
			{
				GameObject.Destroy(this);
				gameObject.AddComponent<Lobby>();
				audioSource.PlayOneShot(control.buttonSFX[0]);
			}
			GUI.color = Color.white;
		}
	}
}