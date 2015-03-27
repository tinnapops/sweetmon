using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Taste {Sweet, Freshy, Smooth}

[System.Serializable]
public struct MonsterData
{
	public int id;
	public GameObject prefab;
	public string name;
	public Object image;
	public Object shinyImage;
	public int role;// 0 Tank, 1 DD, 2 Heal
	public int currentEvo;
	public int[] evoTo;
	public float atk;
	public float def;
	public float wis;
	public float hp;
	public bool isSeen;
	public Taste taste;
	public Taste like;
	public int exp;
	public int full;
	public int happy;
	public int level;
	public bool isShiny;
	public int toppingID;

	public SkillData skill;

	public AudioClip Cry;

	public void Draw(Rect mb)
	{
		if(image is Texture2D)
		{
			GUI.DrawTexture(mb,image as Texture2D);
		}
		else if(image is Sprite)
		{
			mb.x += mb.width;
			mb.width = -mb.width;
			var sprite	= image as Sprite;

			var rect	= sprite.rect;
			rect.x	/= sprite.texture.width;
			rect.width	/= sprite.texture.width;
			rect.y	/= sprite.texture.height;
			rect.height	/= sprite.texture.height;
			GUI.DrawTextureWithTexCoords(mb,sprite.texture,rect);
		}
		else if(image is GameObject)
		{
		}
		else Debug.Log(image);
	}

	public void GetStar(out int hitpoint,out int attack,out int defence,out int wisdom)
	{
		hitpoint = Mathf.FloorToInt((hp/(13.4f/6)) + 1);
		attack	= Mathf.FloorToInt((atk/(28/6)) + 1);
		defence	= Mathf.FloorToInt((def/(28/6)) + 1);
		wisdom	= Mathf.FloorToInt((wis/(28/6)) + 1);
	}
}

[System.Serializable]
public struct SkillData
{
	public string displayName;
	public string name;
	public string desc;
	public float multiply;
	public int target;
	public Texture2D image;
	public int[] skillCode;
}

[System.Serializable]
public struct Quest
{
	public string name;
	public string desc;
	public int[] enemy;
	public int[] reward;
}

