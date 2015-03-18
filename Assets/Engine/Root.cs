using UnityEngine;
using System.Collections;

namespace Sinoze.Engine
{
	public static class Root
	{
		private static GameObject _gameObject;

		/// <summary>
		/// Reference to a singleton permanent game object. This will be shared by multiple engine components.
		/// </summary>
		/// <value>The game object.</value>
		public static GameObject GameObject
		{
			get
			{
				// auto create of not set by Bootstrap
				if(_gameObject == null)
				{
					_gameObject = new GameObject("Sinize.Engine (Temporary)");
					UnityEngine.Debug.Log("Root gameobject is temporarily created because the game is not started by Bootstrap.");
				}
				return _gameObject;
			}
			internal set
			{
				if(_gameObject != null)
					UnityEngine.Debug.LogError("Root instance should only be set once in Bootstrap");
				_gameObject = value;
			}
		}
	}
}