﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Xamarin.iOS.UnitTests
{
	public abstract class TestRunner
	{
		public long InconclusiveTests { get; protected set; } = 0;
		public long FailedTests { get; protected set; } = 0;
		public long PassedTests { get; protected set; } = 0;
		public long SkippedTests { get; protected set; } = 0;
		public long ExecutedTests { get; protected set; } = 0;
		public long TotalTests { get; protected set; } = 0;
		public long FilteredTests { get; protected set; } = 0;
		public bool RunInParallel { get; set; } = false;
		public string TestsRootDirectory { get; set; }
		public TextWriter Writer { get; set; }
		public List<TestFailureInfo> FailureInfos { get; } = new List<TestFailureInfo> ();

		protected LogWriter Logger { get; }
		protected abstract string ResultsFileName { get; set; }

		protected TestRunner (LogWriter logger)
		{
			Logger = logger ?? throw new ArgumentNullException (nameof (logger));
		}

		public abstract void Run (IList <TestAssemblyInfo> testAssemblies);
		public abstract string WriteResultsToFile ();
		public abstract void WriteResultsToFile (TextWriter writer);

		protected void OnError (string message)
		{
			Logger.OnError (message);
		}

		protected void OnWarning (string message)
		{
			Logger.OnWarning (message);
		}

		protected void OnDebug (string message)
		{
			Logger.OnDebug (message);
		}

		protected void OnDiagnostic (string message)
		{
			Logger.OnDiagnostic (message);
		}

		protected void OnInfo (string message)
		{
			Logger.OnInfo (message);
		}

		protected void OnAssemblyStart (Assembly asm)
		{
		}

		protected void OnAssemblyFinish (Assembly asm)
		{
		}

		protected void LogFailureSummary ()
		{
			if (FailureInfos == null || FailureInfos.Count == 0)
				return;

			OnInfo ("Failed tests:");
			for (int i = 1; i <= FailureInfos.Count; i++) {
				TestFailureInfo info = FailureInfos [i - 1];
				if (info == null || !info.HasInfo)
					continue;

				OnInfo ($"{i}) {info.Message}");
			}
		}

		void AssertExecutionState (TestExecutionState state)
		{
			if (state == null)
				throw new ArgumentNullException (nameof (state));
		}

		protected virtual string GetResultsFilePath ()
		{
			if (String.IsNullOrEmpty (ResultsFileName))
				throw new InvalidOperationException ("Runner didn't specify a valid results file name");


			string resultsPath = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			if (!Directory.Exists (resultsPath))
				Directory.CreateDirectory (resultsPath);
			return Path.Combine (resultsPath, ResultsFileName);
		}
	}
}