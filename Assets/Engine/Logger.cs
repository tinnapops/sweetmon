using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Sinoze.Engine
{
	/// <summary>
	/// Logger.
	/// TODO: not run any of this in PRODUCTION
	/// </summary>
	[SinozeEngineComponent]
	public class Logger : MonoBehaviour
	{
		#region Instance
		private static Logger _instance;
		public static Logger Instance
		{
			get
			{
				if(_instance == null)
				{
					// if Bootstracp was init, this component should already be exist
					_instance = Root.GameObject.GetComponent<Logger>();

					// automatically create Logger instance without using Bootstrap
					if(_instance == null)
						_instance = Root.GameObject.AddComponent<Logger>();
				}
				return _instance;
			}
		}
		#endregion
		
		List<LogMeta> _logs = new List<LogMeta>();
		List<string> _tags = new List<string>();
		
		public ReadOnlyCollection<LogMeta> Logs { get; private set; }
		public ReadOnlyCollection<string> Tags { get; private set; }

		void Awake()
		{
			Logs = new ReadOnlyCollection<LogMeta>(_logs);
			Tags = new ReadOnlyCollection<string>(_tags);
		}

		void OnGUI()
		{
			foreach(var l in _logs)
			{
				var msg = l.message == null ? "(no message)" : l.message.ToString ();
				var meta = "[" + l.logType + "] " + l.timeStamp.ToShortDateString() + " " + l.timeStamp.ToLongTimeString()  + "." + l.timeStamp.Millisecond + " (" + l.frameIndex + ")";
				var output = "";
				output += msg;
				output += "\n" + l.dumpedStackTrace;
				output += "// " + meta;
				GUILayout.Label(output);
			}
		}

		int frameCount;
		void Update()
		{
			frameCount = Time.frameCount;
		}

		private void Record(LogType logType, System.Diagnostics.StackTrace stackTrace, string callerClassName, object message, Exception exception, string[] tags)
		{
			// record a new log
			var log = new LogMeta()
			{
				logType = logType,
				stackTrace = stackTrace,
				callerClassName = callerClassName,
				dumpedStackTrace = DumpStackTrace(stackTrace),
				timeStamp = DateTime.Now,
				frameIndex = frameCount,
				message = message,
				exception = exception,
				tags = tags,
			};
			_logs.Add(log);

			// also collect all unique tags
			if(tags != null)
			{
				for(int i=0;i<tags.Length;i++)
				{
					if(!_tags.Contains(tags[i]))
						_tags.Add(tags[i]);
				}
			}
		}

		#region Public API
		/// <summary>
		/// Log the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Log(object message, params string[] tags)
		{
			string callerClassName;
			var trace = GetStackTrace(1);
			var traceString = GetTraceString(trace.GetFrame(0), out callerClassName);
			Instance.Record(LogType.Log, trace, callerClassName, message, null, tags);

			// send quick snapshot to unity console
			UnityEngine.Debug.Log(message + "\n" + traceString);
		}

		public static void LogError(object message, params string[] tags)
		{
			string callerClassName;
			var trace = GetStackTrace(1);
			var traceString = GetTraceString(trace.GetFrame(0), out callerClassName);
			Instance.Record(LogType.Error, trace, callerClassName, message, null, tags);
			
			// send quick snapshot to unity console
			UnityEngine.Debug.LogError(message + "\n" + traceString);
		}

		public static void LogException(System.Exception exception, params string[] tags)
		{
			string callerClassName;
			var trace = GetStackTrace(1);
			GetTraceString(trace.GetFrame(0), out callerClassName);
			Instance.Record(LogType.Exception, trace, callerClassName, null, exception, tags);
			
			// send quick snapshot to unity console
			UnityEngine.Debug.LogException(exception);
		}

		public static void LogWarning(object message, params string[] tags)
		{
			string callerClassName;
			var trace = GetStackTrace(1);
			var traceString = GetTraceString(trace.GetFrame(0), out callerClassName);
			Instance.Record(LogType.Warning, trace, callerClassName, message, null, tags);
			
			// send quick snapshot to unity console
			UnityEngine.Debug.LogWarning(message + "\n" + traceString);
		}
		#endregion

		internal static void LogAssert(object message, LogType logType, System.Exception exception, params string[] tags)
		{
			string callerClassName;
			var trace = GetStackTrace(2);
			var traceString = GetTraceString(trace.GetFrame(0), out callerClassName);
			Instance.Record(logType, trace, callerClassName, message, null, tags);

			// send quick snapshot to unity console
			if(logType == LogType.Warning)
				UnityEngine.Debug.LogWarning(message + "\n" + traceString);
			else if(logType == LogType.Error)
				UnityEngine.Debug.LogError(message + "\n" + traceString);
			else if(logType == LogType.Exception)
				UnityEngine.Debug.LogException(exception);
		}

		private static System.Diagnostics.StackTrace GetStackTrace(int framesToSkip)
		{
			return new System.Diagnostics.StackTrace(framesToSkip + 1, true);
		}

		private static string DumpStackTrace(System.Diagnostics.StackTrace trace)
		{
			var sb = new System.Text.StringBuilder();
			for(int i=0;i<trace.FrameCount;i++)
			{
				var frame = trace.GetFrame(i);
				sb.AppendLine(GetTraceString(frame));
			}
			return sb.ToString ();
		}
		
		private static string GetTraceString(System.Diagnostics.StackFrame frame)
		{
			string callerClassName;
			return GetTraceString(frame, out callerClassName);
		}

		private static string GetTraceString(System.Diagnostics.StackFrame frame, out string callerClassName)
		{
			var fileName = frame.GetFileName();
			if(!string.IsNullOrEmpty(fileName))
			{
				var indexToTrim = fileName.IndexOf("/Assets");
				fileName = fileName.Remove(0, indexToTrim + 7);
			}
			var line = frame.GetFileLineNumber();
			var method = frame.GetMethod();
			var methodName = method.Name;
			callerClassName = method.ReflectedType.Name;
			return "   at " + callerClassName + "." + methodName + "() (in " + fileName + ":" + line + ")";
		}
	}

	public struct LogMeta
	{
		public LogType logType;
		public System.Diagnostics.StackTrace stackTrace;
		public string callerClassName;
		public string dumpedStackTrace;
		public DateTime timeStamp;
		public int frameIndex;
		public object message;
		public string[] tags;
		public Exception exception;
	}

	public enum LogType
	{
		Error, //	LogType used for Errors.
		Warning, //	LogType used for Warnings.
		Log, //	LogType used for regular log messages.
		Exception, //	LogType used for Exceptions.
	}
}