using System;
using System.Reflection;
using UnityEngine;
using RepoOverride.Interfaces;

namespace RepoOverride.Services
{
    public class PlayerFinder : IPlayerFinder
    {
        public object PlayerHealthInstance { get; private set; }
        public object PlayerAvatarInstance { get; private set; }
        private readonly Interfaces.ILogger _logger;

        public PlayerFinder(Interfaces.ILogger logger)
        {
            _logger = logger;
        }

        public PlayerFinder()
        {
            _logger = Services.Logger.Instance;
        }

        public bool FindPlayerInstances()
        {
            var playerControllerType = Type.GetType("PlayerController, Assembly-CSharp");
            if (playerControllerType == null)
            {
                _logger.LogWarning("PlayerController type not found");
                TryFindingDirectly();
                return PlayerHealthInstance != null;
            }

            var playerControllerInstance = FindFirstObjectByType(playerControllerType);
            if (playerControllerInstance == null)
            {
                _logger.LogWarning("PlayerController instance not found");
                TryFindingDirectly();
                return PlayerHealthInstance != null;
            }

            var playerAvatarScriptField = playerControllerInstance.GetType().GetField("playerAvatarScript",
                BindingFlags.Public | BindingFlags.Instance);

            if (playerAvatarScriptField == null)
            {
                _logger.LogWarning("PlayerAvatarScript field not found");
                TryAlternativeAvatarFinding(playerControllerInstance);
                return PlayerHealthInstance != null;
            }

            PlayerAvatarInstance = playerAvatarScriptField.GetValue(playerControllerInstance);
            var playerHealthField = PlayerAvatarInstance.GetType().GetField("playerHealth",
                BindingFlags.Public | BindingFlags.Instance);

            if (playerHealthField == null)
            {
                _logger.LogWarning("PlayerHealth field not found");
                TryAlternativeHealthFinding();
                return PlayerHealthInstance != null;
            }

            PlayerHealthInstance = playerHealthField.GetValue(PlayerAvatarInstance);
            return true;
        }

        private void TryFindingDirectly()
        {
            _logger.Log("Trying to find PlayerAvatar directly...");

            var playerAvatarType = Type.GetType("PlayerAvatar, Assembly-CSharp");
            if (playerAvatarType == null)
            {
                _logger.LogWarning("PlayerAvatar type not found");
                TryFindHealthDirectly();
                return;
            }

            var playerAvatarObject = FindFirstObjectByType(playerAvatarType);
            if (playerAvatarObject == null)
            {
                _logger.LogWarning("PlayerAvatar instance not found directly");
                TryFindHealthDirectly();
                return;
            }

            _logger.Log("PlayerAvatar instance found directly");
            PlayerAvatarInstance = playerAvatarObject;

            var playerHealthField = playerAvatarType.GetField("playerHealth",
                BindingFlags.Public | BindingFlags.Instance);

            if (playerHealthField == null)
            {
                _logger.LogWarning("playerHealth field not found in PlayerAvatar");
                TryAlternativeHealthFinding();
                return;
            }

            _logger.Log("playerHealth field found");
            PlayerHealthInstance = playerHealthField.GetValue(PlayerAvatarInstance);
        }

        private void TryFindHealthDirectly()
        {
            _logger.Log("Trying to find PlayerHealth directly...");

            var playerHealthType = Type.GetType("PlayerHealth, Assembly-CSharp");
            if (playerHealthType == null)
            {
                _logger.LogWarning("PlayerHealth type not found. All methods failed");
                return;
            }

            _logger.Log("PlayerHealth type found");
            var playerHealthObject = FindFirstObjectByType(playerHealthType);
            if (playerHealthObject == null)
            {
                _logger.LogWarning("PlayerHealth instance not found directly");
                return;
            }

            _logger.Log("PlayerHealth instance found directly!");
            PlayerHealthInstance = playerHealthObject;
        }

        private void TryAlternativeAvatarFinding(object playerControllerInstance)
        {
            _logger.Log("Trying alternative methods to find PlayerAvatar...");

            var alternativeFields = new string[] { "avatarScript", "avatar", "playerAvatar", "player" };

            foreach (var fieldName in alternativeFields)
            {
                var field = playerControllerInstance.GetType().GetField(fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    _logger.Log($"Found alternative avatar field: {fieldName}");
                    PlayerAvatarInstance = field.GetValue(playerControllerInstance);

                    if (PlayerAvatarInstance != null)
                    {
                        TryAlternativeHealthFinding();
                        break;
                    }
                }
            }

            if (PlayerAvatarInstance == null)
            {
                TryFindingDirectly();
            }
        }

        private void TryAlternativeHealthFinding()
        {
            if (PlayerAvatarInstance == null)
                return;

            _logger.Log("Trying alternative methods to find PlayerHealth...");

            var alternativeFields = new string[] { "health", "healthComponent", "healthScript" };

            foreach (var fieldName in alternativeFields)
            {
                var field = PlayerAvatarInstance.GetType().GetField(fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    _logger.Log($"Found alternative health field: {fieldName}");
                    PlayerHealthInstance = field.GetValue(PlayerAvatarInstance);

                    if (PlayerHealthInstance != null)
                    {
                        break;
                    }
                }
            }

            if (PlayerHealthInstance == null)
            {
                TryFindHealthDirectly();
            }
        }

        private object FindFirstObjectByType(Type type)
        {
            try
            {
                var findFirstMethod = typeof(UnityEngine.Object).GetMethod("FindFirstObjectByType", new Type[] { typeof(Type) });

                if (findFirstMethod != null)
                {
                    return findFirstMethod.Invoke(null, new object[] { type });
                }

                var findMethod = typeof(UnityEngine.Object).GetMethod("FindObjectOfType", new Type[] { typeof(Type) });

                if (findMethod != null)
                {
                    return findMethod.Invoke(null, new object[] { type });
                }

                _logger.Log("Using manual fallback to find object of type: " + type.Name);
                GameObject[] allGameObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

                foreach (var go in allGameObjects)
                {
                    var component = go.GetComponent(type);
                    if (component != null)
                    {
                        _logger.Log($"Found {type.Name} on GameObject: {go.name}");
                        return component;
                    }
                }

                _logger.LogWarning($"Could not find object of type {type.Name}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error finding object of type {type.Name}: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
                return null;
            }
        }
    }
}