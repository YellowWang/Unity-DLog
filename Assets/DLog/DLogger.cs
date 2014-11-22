using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class LogData
{
	public enum LogType
	{
		COMMON = 0,
		FTE,
		FEED,
		DRAGON_ARENA,
		PVP,
		CHAT,
		Daily,
		END,
	}
	
	public enum LogSeverity
	{
		NORMAL,
		WARNING,
		EXCEPTION,
		ERROR
	}

	
	public int 	   		count = 1;	
	public LogType 		logType ;
	public LogSeverity  logSeverity;
	public string  		condition ;
	public string  		stacktrace ;
	public int 			sampleId ;

	public LogData(string content, int type, LogSeverity severity)
	{
		condition = content;
		logType = (LogType)type;
		logSeverity = severity;
	}
}

//[ExecuteInEditMode] 
public class Logs : MonoBehaviour
{	
	private static Dictionary<int, bool> _types = new Dictionary<int, bool>();
	private static LogData.LogSeverity _severity = LogData.LogSeverity.NORMAL;
	private static string _search_string = "";

	static Logs()
	{
		Init ();
	}

	public static void SetSearchString(string search_string)
	{
		_search_string = search_string;
	}

	public static string GetSearchString()
	{
		return _search_string;
	}

	private static void Init()
	{
		_types.Clear ();
		for (int i = 0; i < (int)LogData.LogType.END; i++)
		{
			int r = PlayerPrefs.GetInt("unity.log.type_" + i);
			_types.Add(i, r==1);
		}
		Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
	}

	static void CaptureLog (string condition, string stacktrace, LogType type)
	{
		log (condition, 1, (type == LogType.Error)?LogData.LogSeverity.ERROR:LogData.LogSeverity.NORMAL, stacktrace);
	}

	public static void SwitchTypes(LogData.LogType type)
	{
		if (!_types.ContainsKey((int)type))
			return;

		_types [(int)type] = !_types [(int)type];

		PlayerPrefs.SetInt("unity.log.type_" + ((int)type).ToString(), (_types [(int)type]?1:0));
	}

	public static bool GetTypes(LogData.LogType type)
	{
		if (!_types.ContainsKey((int)type))
			return false;
		
		return _types [(int)type];
	}

	public static void AddSeverity(LogData.LogSeverity s)
	{
		_severity = s;
	}

	public static void Clear()
	{
		logs.Clear ();
	}
	
	private static List<LogData> logs = new List<LogData>();
	public static void log(string content, int type = 0, LogData.LogSeverity severity = LogData.LogSeverity.NORMAL, string stackTrace = "")
	{
		if (Debug.isDebugBuild) 
		{
			LogData d = new LogData (content, type, severity);

			d.stacktrace = stackTrace;
			if (string.IsNullOrEmpty(stackTrace))
				d.stacktrace = StackTraceUtility.ExtractStackTrace ();
			string[] stackFrames = d.stacktrace.Split (new char[] {'\n'});
			d.stacktrace = "";
			for (int i = 1; i < stackFrames.Length; i++)
			{
				d.stacktrace += stackFrames[i] + '\n';
			}
			logs.Add (d);
		}
	}

	public static List<LogData> LogsData
	{
		get {
			List<LogData> filtered_logs = new List<LogData>();

			foreach( var l in logs)
			{
				bool match_type = false;
				foreach (var t in _types)
				{
					if ((int)l.logType == t.Key && t.Value)
					{
						match_type = true;
						break;
					}
				}

				bool match_s = false;
				if ((int)_severity <= (int)l.logSeverity )
					match_s = true;

				if (match_type && match_s)
				{
					if (!string.IsNullOrEmpty( _search_string ))
					{
						if (l.condition.ToLower().IndexOf(_search_string.ToLower()) >= 0)
						{
							filtered_logs.Add(l);
						}
					}
					else 
						filtered_logs.Add(l);
				}
			}
			return filtered_logs;
		}
	}
}