using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Sinoze.Engine.Editor
{
	/// <summary>
	/// Sinoze menu bar.
	/// </summary>
	class SinozeMenuBar
	{	
		public const string BootstrapScenePath = "assets/engine/bootstrap.unity";

		[MenuItem ("Sinoze/Start Engine", false, 0)]
		static void StartGame()
		{
			// this will check if the current scene is dirty and popup save dialog
			// pass this condition if user click either don't save or save
			// if user click cancel, this will abort
			if(!EditorApplication.SaveCurrentSceneIfUserWantsTo())
				return;

			// load bootstrap scene along with its dialog
			EditorApplication.OpenScene(BootstrapScenePath);
			var window = EditorWindow.GetWindow<BootstrapDialog>("Sinoze Engine", true);
			window.Show();
		} 
		
		/// <summary>
		/// Callback from BootstrapEditor, confirms the start game.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		internal static void BootstrapEditorConfirmStartGame(string startSceneName, string startScenePath)
		{
			var bs = GameObject.FindObjectOfType<Bootstrap>();
			if(bs == null)
			{
				UnityEngine.Debug.LogWarning("Bootstrap game object not found!");
				return;
			}
			
			// detect sceneName changed, update the bootstrap scene
			if(bs.startScene != startSceneName)
			{
				bs.startScene = startSceneName;
				EditorApplication.SaveScene();
				UnityEngine.Debug.Log("Bootstrap start scene updated to " + startSceneName);

			}

			// make sure the bootstrap scene and the start scene are listed and enabled in build setting
			ValidateBuildSettingScenes(startScenePath);
			
			// start editor playing
			EditorApplication.isPlaying = true;
		}

		/// <summary>
		/// Validates the build setting scenes my making sure that Bootstrap is listed there at the first item
		/// </summary>
		private static void ValidateBuildSettingScenes(string startScenePath)
		{
			bool bootstrapSceneFound = false;
			bool bootstrapSceneDisabled = false;
			int bootstrapSceneIndex = -1;
			var startScenePathLower = startScenePath.ToLower();
			bool startSceneFound = false;
			bool startSceneDisabled = false;
			int startSceneIndex = -1;

			for(int i=0;i<UnityEditor.EditorBuildSettings.scenes.Length;i++)
			{
				var s = UnityEditor.EditorBuildSettings.scenes[i];
				var sLower = s.path.ToLower();
				if(sLower == BootstrapScenePath && !bootstrapSceneFound)
				{
					bootstrapSceneFound = true;
					bootstrapSceneDisabled = !s.enabled;
					bootstrapSceneIndex = i;
				}
				else if(sLower == startScenePathLower && !startSceneFound)
				{
					startSceneFound = true;
					startSceneDisabled = !s.enabled;
					startSceneIndex = i;
				}
			}
			
			if(startSceneDisabled)
			{
				// make sure the start scene is enabled
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length];
				System.Array.Copy(original, newSettings, original.Length);
				
				// enable
				newSettings[startSceneIndex].enabled = true;
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}

			if(bootstrapSceneDisabled || bootstrapSceneIndex != -1)
			{
				// make sure the bootstrap scene is in the first and enabled
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length];
				System.Array.Copy(original, newSettings, original.Length);
				
				// enable
				newSettings[bootstrapSceneIndex].enabled = true;
				
				// swap to first
				var tmp = newSettings[0];
				newSettings[0] = newSettings[bootstrapSceneIndex];
				newSettings[bootstrapSceneIndex] = tmp;
				
				// update
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}

			if(!bootstrapSceneFound)
			{
				// bootstrap scene not found in settings
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length + 1];
				System.Array.Copy(original, 0, newSettings, 1, original.Length);

				// put bootstrap scene at the first item
				newSettings[0] = new EditorBuildSettingsScene(BootstrapScenePath, true);
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}

			if(!startSceneFound)
			{
				// start scene not found in settings
				EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length + 1];
				System.Array.Copy(original, newSettings, original.Length);
				
				// put start scene scene at the last item
				newSettings[newSettings.Length - 1] = new EditorBuildSettingsScene(startScenePath, true);
				UnityEditor.EditorBuildSettings.scenes = newSettings;
			}

		}
	}
}