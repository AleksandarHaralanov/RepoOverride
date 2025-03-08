extern alias UInput;

using System;
using UnityEngine;
using UISystem = UInput::UnityEngine;

namespace RepoOverride
{
    public class Hax : MonoBehaviour
    {
        #region Information
        public const string NAME = "RepoOverride";
        public const string VERSION = "0.1.0.0";
        public const string COMPANY = "Aleksandar Haralanov";
        public const string BUILD_DATE = "2025-03-08";
        #endregion

        #region Components
        private Interfaces.ICheats _cheats;
        private Interfaces.ICursorManager _cursorManager;
        private Interfaces.ILogger _logger;
        private Interfaces.IPlayerFinder _playerFinder;
        private Interfaces.IWindowManager _consoleManager;
        private Interfaces.IWindowManager _menuManager;
        #endregion

        #region Private Variables
        private float _lastKeyPressTime = 0f;
        private const float KEY_COOLDOWN = 0.3f;
        #endregion

        public void Start()
        {
            try
            {
                InitializeComponents();
                _logger.Log($"{NAME} v{VERSION} initialized");
            }
            catch (Exception) { }
        }

        private void InitializeComponents()
        {
            _logger = Services.Logger.Instance;
            _playerFinder = new Services.PlayerFinder(_logger);
            _cheats = new Services.Cheats(_playerFinder, _logger);
            _cursorManager = new Services.CursorManager(_logger);
            _consoleManager = new Services.ConsoleManager(_cheats, _logger);
            _menuManager = new Services.MenuManager(_cheats, _playerFinder, _logger);

            _playerFinder.FindPlayerInstances();
        }

        public void Update()
        {
            try
            {
                HandleKeyboardInput();

                bool showingWindow = _menuManager.IsVisible || _consoleManager.IsVisible;
                _cursorManager.Update(showingWindow);
                _cheats.Update();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Update: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void HandleKeyboardInput()
        {
            bool cooldownElapsed = (Time.time - _lastKeyPressTime) > KEY_COOLDOWN;
            if (!cooldownElapsed)
                return;

            if (UISystem.Input.GetKeyDown(KeyCode.F1))
            {
                _lastKeyPressTime = Time.time;
                _menuManager.Toggle();

                if (_playerFinder.PlayerHealthInstance == null)
                    _playerFinder.FindPlayerInstances();
            }

            if (UISystem.Input.GetKeyDown(KeyCode.F2))
            {
                _lastKeyPressTime = Time.time;
                _consoleManager.Toggle();
            }
        }

        private void OnGUI()
        {
            try
            {
                _menuManager.OnGUI();
                _consoleManager.OnGUI();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in OnGUI: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void OnDestroy()
        {
            try
            {
                _cursorManager.RestoreState();
                _logger.Log($"{NAME} unloaded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in OnDestroy: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }
    }
}