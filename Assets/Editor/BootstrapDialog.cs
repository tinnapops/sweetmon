using UnityEngine;
using UnityEditor;

namespace Sinoze.Engine.Editor
{
	/// <summary>
	/// Bootstrap editor.
	/// </summary>
	class BootstrapDialog : EditorWindow 
	{
		string startSceneName;
		public const string StartScenePrefKey = "Bootstrap.startScene";

		void Awake()
		{
			startSceneName = EditorPrefs.GetString(StartScenePrefKey);
		}
		
		string errorMessage;
		
		void OnGUI() 
		{
			EditorGUILayout.LabelField("Please specify your first game scene");
			if(!string.IsNullOrEmpty(errorMessage))
			{
				EditorGUILayout.LabelField(errorMessage);
			}
			startSceneName = EditorGUILayout.TextField("Scene Name", startSceneName);

			if(!string.IsNullOrEmpty(startSceneName))
			{
				if (GUILayout.Button("Start")) 
				{
					// try search for the given scene file
					var sceneFileName = startSceneName.ToLower() + ".unity";
					var sceneFilePath = "";
					var dotUnityFiles = System.IO.Directory.GetFiles(System.Environment.CurrentDirectory, "*.unity", System.IO.SearchOption.AllDirectories);
					var sceneExist = false;
					foreach(var f in dotUnityFiles)
					{
						if(sceneFileName == System.IO.Path.GetFileName(f).ToLower())
						{
							var filepath = System.IO.Path.GetFullPath(f).ToLower();
							var dir = System.Environment.CurrentDirectory.ToLower();
							if(filepath.StartsWith(dir))
								sceneFilePath = filepath.Remove(0, dir.Length+1);
							sceneExist = true;
							break;
						}
					}

					if(sceneExist)
					{
						EditorPrefs.SetString(StartScenePrefKey, startSceneName);
						SinozeMenuBar.BootstrapEditorConfirmStartGame(startSceneName, sceneFilePath);
						Close();
					}
					else
					{
						errorMessage = "scene name \"" + startSceneName + "\" does not exist";
					}
				}
			}
		}
	}
}