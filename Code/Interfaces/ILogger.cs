using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OKM.EmailAttachmentWorkflow.Code.Interfaces {
	public interface ILogger {
		TraceLevel LowestLogLevel { get; }
		Stack<string> CallStack { get; }

		void SetTraceLevel(string traceLevel);
		void SerializeObjectToLog(object obj, TraceLevel level = TraceLevel.Verbose);

		void WriteLogItem(TraceLevel itemSeverity, string message, params object[] args);

		string CommitLog();
		string BuildCallStack();

		bool Pass(Exception exception, string message = "", TraceLevel level = TraceLevel.Verbose);
		void Exception(Exception exception, string message = "", TraceLevel level = TraceLevel.Error);

		void Success(string message, params object[] args);
		void Verbose(string message, params object[] args);
		void Info(string message, params object[] args);
		void Warn(string message, params object[] args);
		void Error(string message, params object[] args);
	}
}