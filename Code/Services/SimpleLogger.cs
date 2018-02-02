using Newtonsoft.Json;
using OKM.EmailAttachmentWorkflow.Code.Helpers;
using OKM.EmailAttachmentWorkflow.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OKM.EmailAttachmentWorkflow.Code.Services {
	/// <summary>
	/// A very basic injectable logger which is good for building a callstack.
	/// </summary>
	public class SimpleLogger : ILogger {
		/// <summary>
		/// The lowest log level reached during execution. Higher is better.
		/// </summary>
		public TraceLevel LowestLogLevel { get; private set; } = TraceLevel.Verbose;
		public Stack<string> CallStack { get; } = new Stack<string>();

		TraceLevel LogLevel { get; set; } = TraceLevel.Error;

		Stopwatch Stopwatch { get; } = Stopwatch.StartNew();

		public void SetTraceLevel(string traceLevel) {
			if (Enum.TryParse(traceLevel, out TraceLevel traceLevelValue))
				LogLevel = traceLevelValue;
		}

		/// <summary>
		/// Serializes a given object and adds it to the log stack.
		/// </summary>
		public void SerializeObjectToLog(object obj, TraceLevel level = TraceLevel.Verbose) {
			var value = JsonConvert.SerializeObject(obj, Formatting.Indented);
			WriteLogItem(level, value);
		}

		/// <summary>
		/// Allows for pass-through exception logging
		/// </summary>
		/// <example>
		/// catch(Exception e) when (Log.Pass(e)) { }
		/// </example>
		public bool Pass(Exception exception, string message = "", TraceLevel level = TraceLevel.Verbose) {
			Exception(exception, message, level);
			return false;
		}

		/// <summary>
		/// Output the information on an exception
		/// </summary>
		public void Exception(Exception exception, string message = "", TraceLevel level = TraceLevel.Error) {
			exception.ThrowIfNull(nameof(exception));

			var stack = new StackTrace(exception).ToString();

			// Unwrap inner exceptions
			while (exception.InnerException != null)
				exception = exception.InnerException;

			if (!string.IsNullOrEmpty(message))
				message += Environment.NewLine;

			WriteLogItem(level, message + exception.ToString());
		}

		/// <summary>
		/// 2017-10-03 JPY: We log all successes for now. Maybe this changes in the future once we have system settings.
		/// </summary>
		public void Success(string message, params object[] args) => WriteLogItem(TraceLevel.Off, message, args);

		/// <summary>
		/// Output the formatted message at the Info level.
		/// </summary>
		public void Verbose(string message, params object[] args) => WriteLogItem(TraceLevel.Verbose, message, args);

		/// <summary>
		/// Output the formatted message at the Info level.
		/// </summary>
		public void Info(string message, params object[] args) => WriteLogItem(TraceLevel.Info, message, args);

		/// <summary>
		/// Output the formatted message at the Warn level.
		/// </summary>
		public void Warn(string message, params object[] args) => WriteLogItem(TraceLevel.Warning, message, args);

		/// <summary>
		/// Output the formatted message at the Error level.
		/// </summary>
		public void Error(string message, params object[] args) => WriteLogItem(TraceLevel.Error, message, args);

		/// <example>
		/// WriteLog(LogCategory.Unexpected, $"{Constants.Log.Service.UlsKey}: List Item Event Service call failed {e.Message}");
		/// </example>
		public void WriteLogItem(TraceLevel itemSeverity, string message, params object[] args) {
			if (itemSeverity > LogLevel)
				return;

			if (itemSeverity < LowestLogLevel)
				LowestLogLevel = itemSeverity;

			CallStack.Push(message);
		}

		/// <summary>
		/// Returns the collapsed callstack
		/// </summary>
		public string CommitLog() {
			Stopwatch.Stop();

			if (!CallStack.Any())
				return string.Empty;

			var logDuration = $"{Convert.ToInt32(Stopwatch.Elapsed.TotalMilliseconds)} ms";
			WriteLogItem(TraceLevel.Info, $"Process Duration: {logDuration}");

			return BuildCallStack();
		}

		public string BuildCallStack() {
			var callStack = new StringBuilder();
			var stackenumerator = CallStack.GetEnumerator();

			while (stackenumerator.MoveNext())
				callStack.AppendLine(stackenumerator.Current);

			return callStack.ToString();
		}
	}
}
