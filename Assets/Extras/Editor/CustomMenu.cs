using UnityEditor;

public class CustomMenu
{
	[MenuItem("CustomMenu/Reload Scripts")]
	private static void ReloadScripts()
	{
		EditorApplication.ExitPlaymode();
		EditorUtility.RequestScriptReload();
		AssetDatabase.Refresh();
		UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
	}
}