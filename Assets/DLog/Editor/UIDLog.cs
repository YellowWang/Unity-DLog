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

public class UIDLog : EditorWindow
{
	[MenuItem("Kabam/Log/DLog #d")]
	static public void CreateUIWizard ()
	{
		var w = EditorWindow.GetWindow<UIDLog>(false, "logger", true);
		w.autoRepaintOnSceneChange = true;

	}

	void OnPlayModeChanged()
	{
		if (!EditorApplication.isPlaying) {
			lastClick = 0;
			lastClick_stack = 0;
		}
	}

	public UIDLog()
	{
		EditorApplication.playmodeStateChanged = OnPlayModeChanged;

		barStyle.border = new RectOffset(1,1,1,1);
		barStyle.normal.background = getImage("dlog_bar" , size.x , size.y) ;
		barStyle.active.background = getImage("dlog_button_active" , size.x , size.y ) ;
		barStyle.alignment = TextAnchor.MiddleCenter ;
		barStyle.margin = new RectOffset(1,1,1,1);
		barStyle.clipping = TextClipping.Clip;
		barStyle.fontSize = barFontSize;

		buttonActiveStyle.border = new RectOffset(1,1,1,1);
		buttonActiveStyle.normal.background = getImage("dlog_button_active" ,size.x,size.y);
		buttonActiveStyle.alignment = TextAnchor.MiddleCenter ;
		buttonActiveStyle.margin = new RectOffset(1,1,1,1);
		//buttonActiveStyle.padding = new RectOffset(4,4,4,4);
		buttonActiveStyle.fontSize = barFontSize;


		// log
		logIcon 	= getImage("dlog_log_icon" ,size.x,size.y);
		logErrorIcon = getImage ("dlog_error_icon", size.x, size.y);
		logContent = new GUIContent ("", logIcon, "show or hide logs");
		logErrorContent = new GUIContent ("", logErrorIcon, "show or hide logs");

		nonStyle.clipping = TextClipping.Clip;
		nonStyle.border = new RectOffset(0,0,0,0);
		nonStyle.normal.background = null ;
		nonStyle.fontSize = (int)(size.y /2 );
		nonStyle.alignment = TextAnchor.MiddleLeft ;
		
		
		evenLogStyle.normal.background = getImage("dlog_even_log" ,size.x/2,size.y/2);
		evenLogStyle.fixedHeight = size.y /2;
		evenLogStyle.clipping = TextClipping.Clip ;
		evenLogStyle.alignment = TextAnchor.UpperLeft ;
		evenLogStyle.imagePosition = ImagePosition.ImageLeft ;
		evenLogStyle.fontSize = barFontSize;
		evenLogStyle.fontStyle = FontStyle.Normal;
		evenLogStyle.normal.textColor = new Color (0.78f, 0.78f, 0.78f);
		
		oddLogStyle.normal.background = getImage("dlog_odd_log" ,size.x/2,size.y/2);
		oddLogStyle.fixedHeight = size.y / 2;
		oddLogStyle.clipping = TextClipping.Clip ;
		oddLogStyle.alignment = TextAnchor.UpperLeft ;
		oddLogStyle.imagePosition = ImagePosition.ImageLeft ;
		oddLogStyle.fontSize = barFontSize;
		oddLogStyle.fontStyle = FontStyle.Normal;
		oddLogStyle.normal.textColor = new Color (0.78f, 0.78f, 0.78f);
		
		
		selectedLogStyle.normal.background = getImage("dlog_selected" ,size.x/2,size.y/2);
		selectedLogStyle.fixedHeight = size.y / 2 ;
		selectedLogStyle.clipping = TextClipping.Clip ;
		selectedLogStyle.alignment = TextAnchor.UpperLeft ;
		selectedLogStyle.normal.textColor = Color.white;
		//selectedLogStyle.wordWrap = true;
		selectedLogStyle.fontSize = barFontSize;

		
		btnStyle.fixedHeight = size.y ;
		btnStyle.clipping = TextClipping.Clip ;
		btnStyle.border = new RectOffset(0,0,0,0);
		btnStyle.normal.background = null ;
		btnStyle.alignment = TextAnchor.MiddleCenter;


		// stack
		backStyle.normal.background = getImage("dlog_even_log" ,size.x/2,size.y/2);
		backStyle.clipping = TextClipping.Clip ;
		backStyle.fontSize = 16;


		int paddingX = (int)(size.x*0.2f) ;
		int paddingY = (int)(size.y*0.2f) ;
		stackLabelStyle.wordWrap = true ;
		stackLabelStyle.fontSize = (int)(size.y /2 );
		stackLabelStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 

		btnNoneStyle.fixedHeight = size.y / 2 ;
		btnNoneStyle.clipping = TextClipping.Clip ;
		btnNoneStyle.border = new RectOffset(0,0,0,0);
		btnNoneStyle.normal.background = null ;
		btnNoneStyle.alignment = TextAnchor.MiddleLeft;
		btnNoneStyle.fontSize = 12;
		btnNoneStyle.normal.textColor = new Color (0.78f, 0.78f, 0.78f);

		btnSelectedStyle.normal.background = getImage("dlog_selected" ,size.x/2,size.y/2);
		btnSelectedStyle.fixedHeight = size.y / 2 ;
		btnSelectedStyle.clipping = TextClipping.Clip ;
		btnSelectedStyle.alignment = TextAnchor.UpperLeft ;
		btnSelectedStyle.normal.textColor = Color.white ;
		btnSelectedStyle.fontSize = 12;
	}


	static Texture2D getImage(string path , float width , float height )
	{
		Texture2D texture =  ( Texture2D  )Resources.Load( path , typeof(Texture2D  ));
		return texture ;
	}

	public static Vector2 size = new Vector2( 32 , 32 ) ;
	public static int selected_index = -1;
	Vector2 scrollPosition = Vector2.zero;
	static Vector2 scrollPosition2 = Vector2.zero;
	static float lastClick = 0;
	static int menu_height = 0;

	static GUIContent clearContent 	= new GUIContent("",getImage("dlog_clear",size.x,size.y),"Clear logs");
	static GUIContent searchContent 	= new GUIContent("",getImage("dlog_search",size.x*2,size.y),"search logs");
	static GUIContent collapseContent = new GUIContent("",null,"logs type");
	static GUIStyle buttonActiveStyle = new GUIStyle();
	static GUIStyle barStyle = new GUIStyle();
	static int barFontSize = 14;
	void OnGUIToolbar()
	{
		GUILayout.BeginHorizontal(  barStyle );
		int clear_width = 16;
		int btn_width = 32;
		if( GUILayout.Button( clearContent, barStyle ,  GUILayout.Width(clear_width) , GUILayout.Height(size.y/2)))
		{
			Logs.Clear();
		}
		
		int splite_space = 10;
		GUILayout.Space (10);

		for (int i = 0; i < (int)LogData.LogType.END; i++)
		{
			collapseContent.text = i.ToString();
			collapseContent.tooltip = i.ToString();
			if( GUILayout.Button( collapseContent,  Logs.GetTypes((LogData.LogType)i)? buttonActiveStyle : barStyle ,  GUILayout.Width(btn_width) , GUILayout.Height(size.y/2)))
			{
				Logs.SwitchTypes((LogData.LogType)i);
			}
		}

		int search_icon_width = 16;
		int padding = 20;
		GUILayout.Space (Screen.width - 100 - splite_space - btn_width*((int)LogData.LogType.END) - clear_width - search_icon_width - padding);
		GUILayout.Box (searchContent, GUILayout.Width(16), GUILayout.Height(16));
		string newstring = GUILayout.TextField (Logs.GetSearchString(), GUILayout.Width(100));
		if (newstring != Logs.GetSearchString ()) 
		{
			Logs.SetSearchString(newstring);
		}


		GUILayout.EndHorizontal();
	}

	void OnInspectorUpdate()
	{
		Repaint ();
	}

	static GUIStyle nonStyle = new GUIStyle();
	static GUIStyle evenLogStyle = new GUIStyle();
	static GUIStyle oddLogStyle = new GUIStyle();
	static GUIStyle selectedLogStyle = new GUIStyle();
	static GUIContent logContent 	= new GUIContent();
	static GUIContent logErrorContent = new GUIContent ();
	static GUIStyle btnStyle = new GUIStyle();
	static Rect countRect = new Rect();
	static Rect scrollRect = new Rect();
	static Rect logRect = new Rect();
	static Rect labelRect = new Rect();
	static Texture2D logIcon;
	static Texture2D logErrorIcon;
	static int stackHeight = 224;
	static float lastClick_stack = 0;
	static float lastScrollEndPos = 0;
	void OnGUILog()
	{
		countRect.x = 4;
		countRect.y = 4;
		countRect.width = 24;
		countRect.height = 24;
		
		labelRect.x = 32;
		labelRect.y = 0;
		labelRect.width = Screen.width;
		labelRect.height = 32;
		
		scrollRect.x = 0;
		scrollRect.y = 0;
		scrollRect.width = Screen.width;
		scrollRect.height = 32;
		
		logRect.x = 0;
		logRect.y = 20;
		logRect.width = Screen.width;
		logRect.height = Screen.height - stackHeight - 20;
		
		GUILayout.BeginArea (logRect);
		if (scrollPosition.y == lastScrollEndPos) 
		{
			scrollPosition.y = (Logs.LogsData.Count * 32 - logRect.height);
		}
		scrollPosition = GUILayout.BeginScrollView( scrollPosition );
		lastScrollEndPos = (Logs.LogsData.Count * 32 - logRect.height)<=0?0:(Logs.LogsData.Count * 32 - logRect.height);

		var logs = Logs.LogsData;

		for (int i = 0; i < logs.Count; i++)
		{
			countRect.y = 32 * i + 4 + menu_height;
			labelRect.y = 32 * i + menu_height;
			
			if( GUILayout.Button( "" , btnStyle ) )
			{
				if(Time.realtimeSinceStartup-lastClick<0.2){
					OpenFile(logs[i].stacktrace);
				}else{
					selected_index = i;
					selected_stack = -1;
				}
				lastClick = Time.realtimeSinceStartup;
			}
			
			GUIStyle style = (i%2== 1)? oddLogStyle: evenLogStyle;
			style = (selected_index == i) ? selectedLogStyle : style;

			GUI.Box(countRect, logs[i].logSeverity==LogData.LogSeverity.ERROR?logErrorContent:logContent, nonStyle);
			GUI.Label(labelRect, logs[i].condition , style );
			
			labelRect.y = labelRect.y + size.y/2;
			GUI.Label(labelRect, logs[i].stacktrace , style );
			
		}
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
	}

	void OnGUI ()
	{
		OnGUIToolbar ();

		OnGUILog ();

		OnGUIStack (Logs.LogsData);
		
		//NGUIEditorTools.DrawSeparator ();
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

	static Rect stackRect = new Rect();
	static Rect buttomRect= new Rect();
	static Rect copyRect = new Rect();
	static GUIStyle   	backStyle = new GUIStyle();
	static GUIStyle stackLabelStyle = new GUIStyle();
	static GUIStyle btnNoneStyle = new GUIStyle();
	static GUIStyle btnSelectedStyle = new GUIStyle();
	static Rect textRect = new Rect();
	static int selected_stack = -1;
	static void OnGUIStack(List<LogData> logs){

		stackRect.x = 0f ;
		stackRect.y = Screen.height-stackHeight;
		stackRect.width = Screen.width ;
		stackRect.height = stackHeight - 20;

		buttomRect.x = 0f ;
		buttomRect.y = Screen.height - 32/*size.y*/ ;
		buttomRect.width = Screen.width ;
		buttomRect.height = 32/*size.y*/ ;

		textRect.x = 0f ;
		textRect.y = 0f;
		textRect.width = Screen.width ;
		textRect.height = 16 ;

		copyRect.x = Screen.width-50f;
		copyRect.y = 0;
		copyRect.width = 50f;
		copyRect.height = 16f;

		if( selected_index != -1 && selected_index < logs.Count)
		{
			var selectedLog = logs[selected_index];

			GUILayout.BeginArea( stackRect , backStyle );

			GUILayout.BeginHorizontal();
			GUILayout.Button(selectedLog.condition, btnNoneStyle);
			if (GUILayout.Button("Copy", barStyle, GUILayout.Width(50)))
			{
				TextEditor te = new TextEditor();
				te.content = new GUIContent(selectedLog.condition + "\n" + selectedLog.stacktrace);
				te.SelectAll();
				te.Copy();
			}
			GUILayout.EndHorizontal();

			scrollPosition2 = GUILayout.BeginScrollView( scrollPosition2  );		

			string[] splitLog = selectedLog.stacktrace.Split('\n');

			GUILayout.BeginVertical();
			for (int i = 0; i < splitLog.Length; i++)
			{
				GUIStyle btnStyle2 = selected_stack==i? btnSelectedStyle : btnNoneStyle;
				if( GUILayout.Button( splitLog[i] , btnStyle2 ) )
				{
					if(Time.realtimeSinceStartup-lastClick_stack<0.2){
						OpenFile(splitLog[i]);				
					}else{
						selected_stack = i;
					}
					lastClick_stack = Time.realtimeSinceStartup;
				}
				textRect.y = i*16;
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();

		}
		else {
			GUILayout.BeginArea( stackRect , backStyle );
			GUILayout.EndArea();
			GUILayout.BeginArea( buttomRect , backStyle );
			GUILayout.EndArea();
		}
		
	}

}