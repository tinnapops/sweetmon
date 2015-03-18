using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public float atk = 1;
	public float def = 1;
	public float wis = 1;
	public float hp = 5;
	public float currentHp = 5;
	public int limitBreak = 5;
	public int currentLimit = 5;
	public int avatarID = 0;
	public Skill skill;
	
	public Texture[] textureMonster;
	public GameControl gCTRL;
	// Use this for initialization

	public int monID = 0;
	void Start () 
	{
		if (gameObject.GetComponent<MeshRenderer>())
		{
			gameObject.GetComponent<MeshRenderer> ().materials [0].mainTexture = gCTRL.monsters[gCTRL.teamSelect[monID]].image as Texture2D;
			if (gCTRL.monsters[gCTRL.teamSelect[monID]].image is Texture2D)
			{
				gameObject.GetComponent<MeshRenderer> ().materials [0].mainTexture = gCTRL.monsters[gCTRL.teamSelect[monID]].image as Texture2D;
			}
			else if (gCTRL.monsters[gCTRL.teamSelect[monID]].image is Sprite)
			{
				var sprite	= gCTRL.monsters[gCTRL.teamSelect[monID]].image as Sprite;
				
				var rect	= sprite.rect;
				rect.x	/= sprite.texture.width;
				rect.width	/= sprite.texture.width;
				rect.y	/= sprite.texture.height;
				rect.height	/= sprite.texture.height;

				gameObject.GetComponent<MeshRenderer> ().materials [0].mainTexture = sprite.texture;
				gameObject.GetComponent<MeshRenderer> ().materials [0].mainTextureOffset  = new Vector2(rect.x + rect.width, rect.y);
				gameObject.GetComponent<MeshRenderer> ().materials [0].mainTextureScale = new Vector2(-rect.width, rect.height);
			}
			else 
				Debug.Log(gCTRL.monsters[gCTRL.teamSelect[monID]].image);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
