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
		logbase = new UIDLogBase();
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