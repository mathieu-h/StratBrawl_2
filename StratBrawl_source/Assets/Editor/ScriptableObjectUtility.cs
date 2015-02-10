using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
	/// SUMMARY : This makes it easy to create, name and place unique new ScriptableObject asset files. 
	/// PARAMETERS : 
	/// RETURN : Void.
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension (path) != "") 
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}
		
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
		
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	/// SUMMARY : 
	/// PARAMETERS : None.
	/// RETURN : Void.
	[MenuItem("Custom/CreateGameSettings")]
	public static void CreateGameSettings()
	{
		ScriptableObjectUtility.CreateAsset<SO_game_settings>();
	}
}