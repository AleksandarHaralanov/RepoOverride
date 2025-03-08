using System;
using UnityEngine;

namespace RepoOverride.Models
{
    public struct LogMessage
    {
        public string Message;
        public LogType Type;
        public DateTime Time;

        public LogMessage(string message, LogType type)
        {
            Message = message;
            Type = type;
            Time = DateTime.Now;
        }

        public readonly string GetFormattedMessage()
        {
            string prefix = Type switch
            {
                LogType.Error => "[ERROR]",
                LogType.Warning => "[WARN]",
                LogType.Log => "[INFO]",
                _ => "[DEBUG]"
            };

            return $"[{Time:HH:mm:ss}] {prefix} {Message}";
        }
    }
}