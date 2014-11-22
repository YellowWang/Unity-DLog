using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

public class UIDLogBase
{
	public delegate void OpenFileHandler(string GUIContent);
	public OpenFileHandler openFileHandler;

	public Vector2 size = new Vector2( 32 , 32 ) ;
	public int selected_index = -1;
	Vector2 scrollPosition = Vector2.zero;
	Vector2 scrollPosition2 = Vector2.zero;
	public float lastClick = 0;
	int menu_height = 0;

	GUIContent clearContent;
	GUIContent searchContent;
	GUIContent collapseContent = new GUIContent("",null,"logs type");
	GUIStyle buttonActiveStyle = new GUIStyle();
	GUIStyle barStyle = new GUIStyle();
	int barFontSize = 14;

	public UIDLogBase(bool active = true)
	{
		this.active = active;		

		clearContent 	= new GUIContent("",getImage("dlog_clear",size.x,size.y),"Clear logs");
     	searchContent 	= new GUIContent("",getImage("dlog_search",size.x*2,size.y),"search logs");

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

	Texture2D getImage(string path , float width , float height )
	{
		Texture2D texture =  ( Texture2D  )Resources.Load( path , typeof(Texture2D  ));
		return texture ;
	}

	public bool active = false;
	
	void OnGUIToolbar()
	{
		GUILayout.BeginHorizontal( active?barStyle:nonStyle );
		int clear_width = 16;
		int btn_width = 32;

		collapseContent.text = "A";
		if( GUILayout.Button( collapseContent,  active? buttonActiveStyle : barStyle ,  GUILayout.Width(btn_width) , GUILayout.Height(size.y/2)))
		{
			active = !active;
		}
		if (!active) {
			GUILayout.EndHorizontal ();
			return;
		}


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

	GUIStyle nonStyle = new GUIStyle();
	GUIStyle evenLogStyle = new GUIStyle();
	GUIStyle oddLogStyle = new GUIStyle();
	GUIStyle selectedLogStyle = new GUIStyle();
	GUIContent logContent 	= new GUIContent();
	GUIContent logErrorContent = new GUIContent ();
	GUIStyle btnStyle = new GUIStyle();
	Rect countRect = new Rect();
	Rect scrollRect = new Rect();
	Rect logRect = new Rect();
	Rect labelRect = new Rect();
	Texture2D logIcon;
	Texture2D logErrorIcon;
	int stackHeight = 224;
	public float lastClick_stack = 0;
	float lastScrollEndPos = 0;
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
					//OpenFile(logs[i].stacktrace);
					if (openFileHandler != null)
						openFileHandler (logs [i].stacktrace);
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

	public void OnGUI ()
	{
		OnGUIToolbar ();

		if (!active) {			
			return;
		}

		OnGUILog ();

		OnGUIStack (Logs.LogsData);

		//NGUIEditorTools.DrawSeparator ();
	}

	Rect stackRect = new Rect();
	Rect buttomRect= new Rect();
	Rect copyRect = new Rect();
	GUIStyle   	backStyle = new GUIStyle();
	GUIStyle stackLabelStyle = new GUIStyle();
	GUIStyle btnNoneStyle = new GUIStyle();
	GUIStyle btnSelectedStyle = new GUIStyle();
	Rect textRect = new Rect();
	int selected_stack = -1;
	void OnGUIStack(List<LogData> logs){

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
			/*if (GUI.Button(copyRect, "Copy", barStyle))
			{
				TextEditor te = new TextEditor();
				te.content = new GUIContent(selectedLog.condition + "\n" + selectedLog.stacktrace);
				te.SelectAll();
				te.Copy();
			}*/

			//UnityEngine.Debug.Log("positon : " + scrollPosition2.ToString());
			//UnityEngine.Debug.Log("scroll : " + stackRect
			scrollPosition2 = GUILayout.BeginScrollView( scrollPosition2  );
			//UnityEngine.Debug.Log("positon : " + scrollPosition2.ToString());

			string[] splitLog = selectedLog.stacktrace.Split('\n');

			GUILayout.BeginVertical();
			for (int i = 0; i < splitLog.Length; i++)
			{
				GUIStyle btnStyle2 = selected_stack==i? btnSelectedStyle : btnNoneStyle;
				if( GUILayout.Button( splitLog[i] , btnStyle2 ) )
				{
					if(Time.realtimeSinceStartup-lastClick_stack<0.2){
						if (openFileHandler != null)
							openFileHandler (splitLog [i]);
						//OpenFile(splitLog[i]);				
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

			/*GUILayout.BeginArea( buttomRect , backStyle);
			GUILayout.BeginHorizontal( ); 
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();*/

		}
		else {
			GUILayout.BeginArea( stackRect , backStyle );
			GUILayout.EndArea();
			GUILayout.BeginArea( buttomRect , backStyle );
			GUILayout.EndArea();
		}

	}
}