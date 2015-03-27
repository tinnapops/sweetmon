using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public enum Team { Left = -1,None = 0,Right = 1 }
public class Monster : MonoBehaviour
{
	public Team Team	= Team.None;

	public float atk = 1;
	public float def = 1;
	public float wis = 1;
	public float hp = 5;
	public float currentHp = 5;
	public int limitBreak = 5;
	public int currentLimit = 5;
	public int avatarID = 0;
	public SkillData skill;
	
	public Texture[] textureMonster;
	// Use this for initialization

	public int monID = 0;
	Animation animate;
	void Start ()
	{
		animate	= gameObject.GetComponent<Animation>();
		if(animate)
		{
			foreach(var state in animate.Cast<AnimationState>())
			{
				if(state.name == "anim_idle_01")
				{
					steps[0]	= "anim_idle_01";
					steps[1]	= "anim_active_01";
					steps[2]	= "anim_eat_03";
					steps[3]	= "anim_grapStop_01";
					break;
				}

				if(state.name == "idle")
				{
					steps[0]	= "idle";
					steps[1]	= "attack";
					steps[2]	= "win";
					steps[3]	= "useSkill";
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	readonly string[] steps	= new string[4];
	public bool IsIdle
	{
		get { return animate != null && animate.IsPlaying(steps[0]); }
	}
	public bool Animate(int step)
	{
		if(animate == null || string.IsNullOrEmpty(steps[step]))
			return false;

		animate.CrossFade(steps[step],0.3f);
		return true;
	}
}
