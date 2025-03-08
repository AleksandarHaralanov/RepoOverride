using System.Collections.Generic;
using RepoOverride.Models;

namespace RepoOverride.Interfaces
{
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        IReadOnlyList<LogMessage> GetLogs();
        void ClearLogs();
    }
}