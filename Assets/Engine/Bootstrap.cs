using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Sinoze.Engine
{
	/// <summary>
	/// Starting point of the Engine
	/// A game should start with this scene (see "Assets/Engine/Bootstrap.unity") to have engine components properly setup
	/// In editor, you can also use Sinzoe menu bar > Start Game to automatically start bootstrap scene
	/// </summary>
	public class Bootstrap : MonoBehaviour 
	{
		/// <summary>
		/// The sinoze game identifier.
		/// </summary>
		public string sinozeGameId;

		/// <summary>
		/// The start scene, user should specify in the inspector so that the bootstrap can load the game after its initialization
		/// </summary>
		public string startScene;
		
		static bool _isStarted;

		void Awake()
		{
			UnityEngine.Debug.Log("Bootstrap awake");
			if(_isStarted)
			{
				UnityEngine.Debug.LogWarning("Bootstrap already started.");
				Done ();
			}
		}

		void Start()
		{
			// create root engine game object (to hold engine's components)
			if(!_isStarted)
				Init();
			_isStarted = true;

			Done();
		}

		/// <summary>
		/// create root game object and attach engine components
		/// </summary>
		void Init()
		{

			// create root game object
			Root.GameObject = new GameObject("Sinoze.Engine");
			DontDestroyOnLoad(Root.GameObject);
			
			// search classes SinozeEngineComponent attribute and add them as components
			var componentTypes = SinozeEngineComponentAttribute.GetComponentTypes();
			foreach(var type in componentTypes)
			{
				Root.GameObject.AddComponent(type);
				UnityEngine.Debug.Log(type + " created");
			}

			UnityEngine.Debug.Log("Bootstrap init success!");
		}

		void Done()
		{
			// boostrap done!
			if(string.IsNullOrEmpty(startScene))
			{
				UnityEngine.Debug.Log("startScene is not set in Bootstrap. put your first game scene name there.");
				Destroy(this.gameObject);
			}
			else
			{
				UnityEngine.Debug.Log("LOAD LEVEL => " + startScene);
				Application.LoadLevelAsync(startScene);
			}
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	class SinozeEngineComponentAttribute : Attribute
	{
		static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly) 
		{
			foreach(Type type in assembly.GetTypes()) 
			{
				if (Attribute.IsDefined(type, typeof(SinozeEngineComponentAttribute)))
				{
					yield return type;
				}
			}
		}

		public static IEnumerable<Type> GetComponentTypes()
		{
			return GetTypesWithHelpAttribute(Assembly.GetExecutingAssembly());
		}
	}
}