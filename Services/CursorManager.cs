using System;
using UnityEngine;
using RepoOverride.Interfaces;

namespace RepoOverride.Services
{
    public class CursorManager : ICursorManager
    {
        private CursorLockMode _originalCursorLockState;
        private bool _originalCursorVisible;
        private bool _cursorStateChanged = false;
        private readonly Interfaces.ILogger _logger;

        public CursorManager(Interfaces.ILogger logger)
        {
            _logger = logger;
            SaveState();
        }

        public void Update(bool menuActive)
        {
            if (!menuActive && _cursorStateChanged)
            {
                RestoreState();
                _cursorStateChanged = false;
                return;
            }

            if (menuActive && !_cursorStateChanged)
            {
                SaveState();
                FreeCursor();
                _cursorStateChanged = true;
            }
        }

        private void SaveState()
        {
            try
            {
                _originalCursorLockState = Cursor.lockState;
                _originalCursorVisible = Cursor.visible;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving cursor state: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        public void RestoreState()
        {
            try
            {
                Cursor.lockState = _originalCursorLockState;
                Cursor.visible = _originalCursorVisible;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error restoring cursor state: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void FreeCursor()
        {
            try
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error freeing cursor: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }
    }
}