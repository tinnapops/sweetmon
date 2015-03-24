using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class GameControl : MonoBehaviour
{
	public BattleController battleOBJ;
	AudioSource audioSource;
	void Start() 
	{
		audioSource	= gameObject.GetComponent<AudioSource>();
		audioSource.Stop();
		audioSource.clip = bgm[0];
		audioSource.Play();

		battleOBJ	= gameObject.transform.GetChild(0).GetComponent<BattleController>();

		gameObject.AddComponent<GameControlUI.Title>();
	}

	public AudioClip[] bgm;

	public AudioClip[] buttonSFX;

	
	public int[] playerMonster;

	public MonsterData[] monsters;
	public Quest[] quest;

	public int currentQuestSelect = 0;
	public Quest CurrentQuest
	{
		get { return quest[currentQuestSelect]; }
	}

	public List<int> teamSelect = new List<int> ();

	public Rect statusPosition;

	public Rect battlePosition;
	public Rect FeedingPosition;

	public Rect monsterStockRect;
	public Rect monsterListStockRect;

	public Rect monsterInfoRect;

	public Rect questInfoRect;

	public bool isMonsterInfoPopup = false;

	public Texture2D startScreen;

	public Vector2 scrollPosition;

	public int currentSelectMonsterInfo = 0;
	public void AutoResize()
	{
		int screenWidth	= 1024;
		int screenHeight	= 768;
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth,(float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(resizeRatio.x,resizeRatio.y,1.0f));
	}

	void OnGUI()
	{
		/*
		if(start >= 0)
		{
			if(start == 0)
			{
				if(Input.anyKeyDown)
					start	+= Time.deltaTime;
			}
			else start	+= Time.deltaTime;
			if(start > 1)
				start	= -1;

			var tmp	= GUI.color;
			GUI.color = new Color(1,1,1,1 - start);
			GUI.DrawTexture(new Rect(0,0,1024,768), startScreen);
			GUI.color	= tmp;

			return;
		}
		*/

		if(battleOBJ.gameObject.activeSelf)
			return;

		AutoResize();

		//Status Bar
		GUI.BeginGroup (statusPosition, "");
		{
			GUI.Box(new Rect(0, 0, statusPosition.width, statusPosition.height), "");
			GUI.Box(new Rect(5, 5, 310, 50), "Player LV");
			
			if (teamSelect.Count > 0)
			{
				monsters[teamSelect.First()].Draw(new Rect(5, 5, 50, 50));
			}
			else
			{
				GUI.Box(new Rect(5, 5, 50, 50), "?");
			}
			GUI.Box(new Rect(320, 5, 215, 50), "Currency 1");
			GUI.Box(new Rect(320 + 220, 5, 215, 50), "Currency 2");
			GUI.Box(new Rect(320 + 220 + 220, 5, 215, 50), "Dungeon Ticket");


			
		}
		GUI.EndGroup ();
	}

	public void AddMonsterToTeam(int id)
	{

		if (!teamSelect.Contains(id))
		{
			if (teamSelect.Count < 3)
			{
				teamSelect.Add(id);
			}
		}
		else
		{
			teamSelect.Remove(id);
		}
		SortTeam ();
	}


	void SortTeam()
	{
	}

	public Vector2 iconSize = new Vector2(128, 128);
	public int battlePosSize = 500;
}