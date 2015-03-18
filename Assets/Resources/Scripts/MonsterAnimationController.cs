using UnityEngine;
using System.Collections;

public class MonsterAnimationController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameObject.GetComponent<Animation>().IsPlaying("anim_idle_01"))
		{
			gameObject.GetComponent<Animation>().CrossFade("anim_idle_01", 0.2f);
			BattleController.isAnimationPlay = false;
		}
	}
}
