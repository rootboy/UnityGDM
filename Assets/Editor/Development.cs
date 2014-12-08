using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Reflection;


public class Development
{
	/// <summary>
	/// Shortkey: Activate the gameobject conveniently.
	/// </summary>
	[MenuItem("Development/Shortcut/Activer &a")]
	static void Activer()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			go.SetActive(!go.activeSelf);
		}
	}


	/// <summary>
	/// Shortkey: Lock the inspector conveniently.
	/// </summary>
	[MenuItem("Development/Shortcut/Locker &l")]
	static void Locker()
	{
		EditorWindow _mouseOverWindow = null;
		if (_mouseOverWindow == null)
		{
			if (!EditorPrefs.HasKey("LockableInspectorIndex"))
				EditorPrefs.SetInt("LockableInspectorIndex", 0);
			int i = EditorPrefs.GetInt("LockableInspectorIndex");
			
			System.Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
			Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
			_mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
		}
		
		if (_mouseOverWindow != null && _mouseOverWindow.GetType().Name == "InspectorWindow")
		{
			System.Type type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
			PropertyInfo propertyInfo = type.GetProperty("isLocked");
			bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
			propertyInfo.SetValue(_mouseOverWindow, !value, null);
			_mouseOverWindow.Repaint();
		}
	}
	
	[MenuItem("Assets/Create/Lua", false, 28)]
	static void LuaCreater()
	{
		Object folder = null;
		string folderPath = string.Empty;

		foreach(Object obj in Selection.objects){
			string selectionPath = AssetDatabase.GetAssetPath(obj); 
			if (Directory.Exists(selectionPath)) {
				folder = obj;
				folderPath = selectionPath.Remove(0, 6);
				break;
			}
		}

		string path = Application.dataPath;
		if(folder != null){
			path = path + folderPath;
		}

		string fullPath= path + "/NewLua.lua";
		int i = 1;
		while(File.Exists(fullPath))
		{
			fullPath = path + "/NewLua" + (i++).ToString() + ".lua";
		}
		File.Create(fullPath).Dispose();
		AssetDatabase.Refresh();

//		Object script =  AssetDatabase.LoadAssetAtPath("Assets/11.lua", typeof(Object));
//		Selection.activeObject = script;
//		EditorApplication.ExecuteMenuItem("Window/Project"); 
//		Event e = new Event { keyCode = KeyCode.F2, type = EventType.keyDown };
//		EditorWindow.focusedWindow.SendEvent(e);
	}
}