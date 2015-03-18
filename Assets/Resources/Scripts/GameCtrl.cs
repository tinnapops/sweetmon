using UnityEngine;
using System.Collections;

public class GameCtrl : MonoBehaviour {


	public enum Step{Home, TeamSetup, BattleField}
	void Start () {


		screenSize.x = 768;
		screenSize.y = 1024;
	
	}

	void Update () {
	
	}



	//Rect Stuff
	Vector2 screenSize = Vector2.zero;

	Rect footerGroup = new Rect(0, 1024 - 100, 768, 100);
	Rect foodButton = new Rect(0, 0, 100, 100);

	void OnGUI()
	{
		UI.AutoResize ((int)screenSize.x, (int)screenSize.y);
		GUI.BeginGroup (footerGroup);
		{
			if (GUI.Button (foodButton, "Food")) 
			{
				
			}
		}
		GUI.EndGroup ();


	}
}
