using UnityEngine;
using System.Collections;
using System;
using Sinoze.Engine;

public class SinozeEngineTest : MonoBehaviour 
{	
	Siege<int> siege1 = new Siege<int>();
	Siege<int> siege2 = new Siege<int>();
	Siege<string> siege3 = new Siege<string>();

	void Start () 
	{
		Logger.LogError("error");
		Assert.True(false, "kuy", AssertLevel.Warning );

		siege1.Listen((s, msg) => { Logger.Log("s1 message received : " + msg); Siege.Post("test");}, SiegeListenOption.SingleComsume);
		siege2.Listen((s, msg) => { Logger.Log("s2 message received : " + msg); }, SiegeListenOption.Persist);
		siege3.Listen((s, msg) => { Logger.Log("s3 message received : " + msg); siege3.Dispose(); }, SiegeListenOption.Persist);

	}

	void OnGUI()
	{
		if(GUILayout.Button("Post"))
		{
			var msg = Time.frameCount;
			Logger.Log("send msg : " + msg);
			Siege.Post(msg);
		}
	}

	void OnDestroy()
	{	
		siege1.Dispose();
		siege2.Dispose();
		siege3.Dispose();
	}
}
