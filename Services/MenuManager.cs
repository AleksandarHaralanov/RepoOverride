extern alias UIMGUI;

using System;
using UnityEngine;
using RepoOverride.Interfaces;
using UGui = UIMGUI::UnityEngine;

namespace RepoOverride.Services
{
    public class MenuManager : IWindowManager
    {
        private readonly ICheats _cheats;
        private readonly IPlayerFinder _playerFinder;
        private readonly Interfaces.ILogger _logger;

        private bool _showMenu = false;
        private Rect _windowRect = new Rect(50, 50, 200, 400);
        private Vector2 _scrollPosition;

        private const string MENU_TITLE = Hax.NAME + " v" + Hax.VERSION;

        public bool IsVisible => _showMenu;

        public MenuManager(ICheats cheats, IPlayerFinder playerFinder, Interfaces.ILogger logger)
        {
            _cheats = cheats;
            _playerFinder = playerFinder;
            _logger = logger;
        }

        public void Toggle()
        {
            _showMenu = !_showMenu;
            _logger.Log("Menu " + (_showMenu ? "opened" : "closed"));
        }

        public void OnGUI()
        {
            if (!_showMenu)
                return;

            try
            {
                _windowRect = UGui.GUI.Window(0, _windowRect, DrawMenuWindow, MENU_TITLE);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error drawing menu window: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void DrawMenuWindow(int windowID)
        {
            _scrollPosition = UGui.GUILayout.BeginScrollView(_scrollPosition);

            UGui.GUILayout.Label("[Controls]", UGui.GUI.skin.box);

            bool newGodModeState = UGui.GUILayout.Toggle(_cheats.GodModeEnabled, "God Mode");
            if (newGodModeState != _cheats.GodModeEnabled)
            {
                _cheats.ToggleGodMode();
            }

            bool newDrawEnemiesState = UGui.GUILayout.Toggle(_cheats.MapEnemiesEnabled, "Map Enemies");
            if (newDrawEnemiesState != _cheats.MapEnemiesEnabled)
            {
                _cheats.ToggleMapEnemies();
            }

            UGui.GUILayout.Label("[Debug]", UGui.GUI.skin.box);
            if (UGui.GUILayout.Button("Find Player References"))
            {
                bool found = _playerFinder.FindPlayerInstances();
                _logger.Log(found ? "Player found successfully" : "Could not find player");
            }

            UGui.GUILayout.EndScrollView();

            UGui.GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}