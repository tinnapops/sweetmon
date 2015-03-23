using UnityEngine;

using System.Linq;
using System.Collections.Generic;

namespace GameControlUI
{
	public class StageSelect : MonoBehaviour
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

			for(int i = 0;i < control.quest.Length;i++)
			{
				if(GUI.Button(new Rect(300,100 + (i * 100),400,100),"STAGE : " + i + " " + control.quest[i].name))
				{
					GameObject.Destroy(this);
					gameObject.AddComponent<TeamSetup>();
					control.currentQuestSelect = i;
					audioSource.PlayOneShot(control.buttonSFX[0]);
				}
			}
		}
	}
}