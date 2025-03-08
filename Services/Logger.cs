using System.Collections.Generic;
using UnityEngine;
using RepoOverride.Models;
using System;

namespace RepoOverride.Services
{
    public class Logger : Interfaces.ILogger
    {
        private static Logger _instance;
        public static Logger Instance => _instance ??= new Logger();

        private readonly List<LogMessage> _logs = new List<LogMessage>();
        private readonly int _maxLogCount = 100;

        public event Action OnLogAdded;

        private Logger() { }

        public void Log(string message)
        {
            AddLogMessage(new LogMessage(message, LogType.Log));
        }

        public void LogWarning(string message)
        {
            AddLogMessage(new LogMessage(message, LogType.Warning));
        }

        public void LogError(string message)
        {
            AddLogMessage(new LogMessage(message, LogType.Error));
        }

        private void AddLogMessage(LogMessage logMessage)
        {
            _logs.Add(logMessage);
            if (_logs.Count > _maxLogCount)
                _logs.RemoveAt(0);

            OnLogAdded?.Invoke();
        }

        public IReadOnlyList<LogMessage> GetLogs() => _logs;

        public void ClearLogs()
        {
            _logs.Clear();
        }
    }
}