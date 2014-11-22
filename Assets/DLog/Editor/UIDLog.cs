/*
* 
* 
* 
*/


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

public class DLog : MonoBehaviour {
	

	[MenuItem("Kabam/Log/DLog #d")]
	static public void CreateUIWizard ()
	{
		var w = EditorWindow.GetWindow<UIDLog>(false, "logger", true);
		w.autoRepaintOnSceneChange = true;
		//w.Init ();
		//Logs.Init ();
		//Logs.SwitchTypes (LogData.LogType.COMMON);

	}
}

public class UIDLog : EditorWindow
{
	UIDLogBase logbase;

	public UIDLog()
	{
		logbase = new UIDLogBase(true);
		EditorApplication.playmodeStateChanged = OnPlayModeChanged;
		logbase.openFileHandler = OpenFile;
	}

	public void OnPlayModeChanged()
	{
		if (!EditorApplication.isPlaying) {
			logbase.lastClick = 0;
			logbase.lastClick_stack = 0;
		}
	}

	private static void OpenFile(string stackframe)
	{
		Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
		//UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal
		System.Type type = assembly.GetType("UnityEditorInternal.InternalEditorUtility");
		if(type == null)
		{
			UnityEngine.Debug.Log("Failed to open source file");
			return;
		}

		string callingFrame = stackframe;

		string filename = "";
		int linenumber = 0;
		if (!SplitLog (callingFrame, ref filename, ref linenumber))
			return;

		MethodInfo method = type.GetMethod("OpenFileAtLineExternal");
		method.Invoke(method, new object[] { 
			@filename,
			linenumber 
		});
	}

	private static bool SplitLog(string source, ref string path, ref int line_no)
	{
		string[] splitLog = source.Split (':');		
		if (splitLog.Length < 3)
			return false;

		int no = splitLog [1].IndexOf ("(at");		
		path = splitLog [1].Substring (no + 3).Trim();		
		line_no = int.Parse (splitLog[2].Substring(0, splitLog[2].IndexOf(')')));

		return true;
	}

	void OnGUI ()
	{
		logbase.OnGUI();
	}

	void OnInspectorUpdate()
	{
		Repaint ();
	}
}