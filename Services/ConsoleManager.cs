extern alias UIMGUI;

using System;
using UnityEngine;
using RepoOverride.Interfaces;
using UGui = UIMGUI::UnityEngine;

namespace RepoOverride.Services
{
    public class ConsoleManager : IWindowManager
    {
        private bool _showConsole = false;
        private Rect _consoleRect = new Rect(250, 20, 600, 300);
        private Vector2 _scrollPosition;
        private string _input = "";
        private readonly ICheats _cheats;
        private readonly Interfaces.ILogger _logger;

        public bool IsVisible => _showConsole;

        public ConsoleManager(ICheats cheats, Interfaces.ILogger logger)
        {
            _cheats = cheats;
            _logger = logger;

            if (_logger is Logger loggerImpl)
            {
                loggerImpl.OnLogAdded += ScrollToBottom;
            }
        }

        public void Toggle()
        {
            _showConsole = !_showConsole;
            _logger.Log("Console " + (_showConsole ? "opened" : "closed"));
        }

        public void OnGUI()
        {
            if (!_showConsole)
                return;

            try
            {
                _consoleRect = UGui.GUI.Window(999, _consoleRect, DrawConsoleWindow, "Debug Console");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error drawing console window: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void DrawConsoleWindow(int windowID)
        {
            try
            {
                UGui.GUILayout.BeginVertical();

                _scrollPosition = UGui.GUILayout.BeginScrollView(_scrollPosition, UGui.GUILayout.Height(230));
                foreach (var logMessage in _logger.GetLogs())
                {
                    switch (logMessage.Type)
                    {
                        case LogType.Error:
                        case LogType.Exception:
                            UGui.GUI.color = Color.red;
                            break;
                        case LogType.Warning:
                            UGui.GUI.color = Color.yellow;
                            break;
                        default:
                            UGui.GUI.color = Color.white;
                            break;
                    }

                    UGui.GUILayout.Label(logMessage.GetFormattedMessage());
                }
                UGui.GUI.color = Color.white;
                UGui.GUILayout.EndScrollView();
                UGui.GUILayout.BeginHorizontal();
                UGui.GUI.SetNextControlName("ConsoleInput");
                _input = UGui.GUILayout.TextField(_input, UGui.GUILayout.Height(20));

                if (UGui.Event.current.isKey && UGui.Event.current.keyCode == KeyCode.Return &&
                    UGui.GUI.GetNameOfFocusedControl() == "ConsoleInput")
                {
                    if (!string.IsNullOrEmpty(_input))
                    {
                        ExecuteCommand(_input);
                        _input = "";
                        UGui.Event.current.Use();
                    }
                }

                if (UGui.GUILayout.Button("Execute", UGui.GUILayout.Width(80), UGui.GUILayout.Height(20)))
                {
                    ExecuteCommand(_input);
                    _input = "";
                }
                UGui.GUILayout.EndHorizontal();

                UGui.GUILayout.BeginHorizontal();
                if (UGui.GUILayout.Button("Clear", UGui.GUILayout.Width(80)))
                {
                    _logger.ClearLogs();
                }

                if (UGui.GUILayout.Button("Close", UGui.GUILayout.Width(80)))
                {
                    _showConsole = false;
                }

                UGui.GUILayout.EndHorizontal();
                UGui.GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DrawConsoleWindow: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }

            UGui.GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void ScrollToBottom()
        {
            _scrollPosition.y = float.MaxValue;
        }

        private void ExecuteCommand(string command)
        {
            string[] args = command.Split(' ');
            if (args.Length == 0)
                return;

            switch (args[0].ToLower())
            {
                case "?":
                    _logger.Log("Available commands: god, heal, clear");
                    break;
                case "god":
                    _cheats.ToggleGodMode();
                    break;
                case "heal":
                    _cheats.MaxHeal();
                    break;
                case "clear":
                    _logger.ClearLogs();
                    break;
                default:
                    _logger.LogWarning($"Unknown command: '{args[0]}'. Use command '?' for help");
                    break;
            }
        }
    }
}