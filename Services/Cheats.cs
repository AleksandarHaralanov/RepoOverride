using RepoOverride.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

namespace RepoOverride.Services
{
    public class Cheats : ICheats
    {
        private readonly IPlayerFinder _playerFinder;
        private readonly Interfaces.ILogger _logger;
        private const float INVINCIBLE_DURATION = float.MaxValue;
        private readonly List<GameObject> _activeMarkers = new List<GameObject>();

        public bool GodModeEnabled { get; private set; } = false;
        public bool MapEnemiesEnabled { get; private set; } = false;

        public Cheats(IPlayerFinder playerFinder, Interfaces.ILogger logger)
        {
            _playerFinder = playerFinder;
            _logger = logger;
        }

        public void Update()
        {
            if (_playerFinder.PlayerHealthInstance == null)
                return;

            if (GodModeEnabled)
                GodMode(true);
        }

        public void ToggleGodMode()
        {
            GodModeEnabled = !GodModeEnabled;

            if (_playerFinder.PlayerHealthInstance == null)
            {
                _logger.LogWarning("Cannot toggle god mode - player not found");
                return;
            }

            GodMode(GodModeEnabled);
            _logger.Log($"God Mode {(GodModeEnabled ? "enabled" : "disabled")}");
        }

        public void ToggleMapEnemies()
        {
            MapEnemiesEnabled = !MapEnemiesEnabled;

            if (MapEnemiesEnabled)
            {
                _logger.Log($"Map Enemies {(MapEnemiesEnabled ? "enabled" : "disabled")}");
                MapEnemies();
            }
            else
            {
                ClearEnemyMarkers();
            }
        }

        public void GodMode(bool enable)
        {
            if (_playerFinder.PlayerHealthInstance == null)
            {
                _logger.LogWarning("Cannot apply god mode - player not found");
                return;
            }

            try
            {
                var godModeField = _playerFinder.PlayerHealthInstance.GetType().GetField("godMode",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (godModeField != null)
                {
                    godModeField.SetValue(_playerFinder.PlayerHealthInstance, enable);
                    return;
                }

                _logger.LogWarning("godMode field not found in playerHealth");

                var invincibleSetMethod = _playerFinder.PlayerHealthInstance.GetType().GetMethod("InvincibleSet");
                if (invincibleSetMethod == null)
                {
                    _logger.LogWarning("InvincibleSet method not found in playerHealth");
                    return;
                }

                invincibleSetMethod.Invoke(_playerFinder.PlayerHealthInstance, new object[] { enable ? INVINCIBLE_DURATION : 0f });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error applying god mode: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        public void MaxHeal()
        {
            if (_playerFinder.PlayerHealthInstance == null)
            {
                _logger.LogWarning("Cannot apply max health - player not found");
                return;
            }

            try
            {
                var healthField = _playerFinder.PlayerHealthInstance.GetType().GetField("health",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                var maxHealthField = _playerFinder.PlayerHealthInstance.GetType().GetField("maxHealth",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (healthField == null || maxHealthField == null)
                {
                    _logger.LogWarning("Could not find health or maxHealth fields");
                    return;
                }

                int maxHealth = (int)maxHealthField.GetValue(_playerFinder.PlayerHealthInstance);
                healthField.SetValue(_playerFinder.PlayerHealthInstance, maxHealth);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error applying max health: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        public void MapEnemies()
        {
            try
            {
                ClearEnemyMarkers();

                Enemy[] enemyObjects = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
                _logger.Log($"Found {enemyObjects?.Length ?? 0} enemies");

                if (enemyObjects == null || enemyObjects.Length == 0)
                    return;

                Map mapInstance = Map.Instance;

                if (mapInstance == null)
                {
                    _logger.LogWarning("Could not find map instance");
                    return;
                }

                    mapInstance.Active = true;
                mapInstance.ActiveSet(true);
                mapInstance.ActiveParent?.SetActive(true);

                foreach (Enemy enemy in enemyObjects)
                {
                    GameObject marker = GameObject.Instantiate(
                        mapInstance.CustomObject ?? mapInstance.ValuableObject,
                        mapInstance.OverLayerParent);

                    marker.name = $"Enemy_{enemy.gameObject.name}_Marker";

                    Vector3 enemyPos = enemy.transform.position;
                    marker.transform.position = enemyPos * mapInstance.Scale + mapInstance.OverLayerParent.position;
                    marker.transform.localPosition = new Vector3(
                        marker.transform.localPosition.x,
                        0f,
                        marker.transform.localPosition.z);

                    SpriteRenderer renderer = marker.GetComponent<SpriteRenderer>() ?? marker.AddComponent<SpriteRenderer>();
                    if (renderer.sprite == null)
                    {
                        Sprite anySprite = mapInstance.CustomObject?.GetComponent<SpriteRenderer>()?.sprite;
                        if (anySprite == null && mapInstance.ValuableObject != null)
                        {
                            anySprite = mapInstance.ValuableObject.GetComponent<SpriteRenderer>()?.sprite;
                        }

                        renderer.sprite = anySprite;
                    }

                    renderer.enabled = true;
                    marker.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

                    EnemyPositionTracker tracker = marker.AddComponent<EnemyPositionTracker>();
                    tracker.enemy = enemy;
                    tracker.mapScale = mapInstance.Scale;
                    tracker.mapParent = mapInstance.OverLayerParent;

                    _activeMarkers.Add(marker);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating enemy map markers: {ex.Message}");
                _logger.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void ClearEnemyMarkers()
        {
            foreach (var marker in _activeMarkers)
            {
                if (marker != null)
                    GameObject.Destroy(marker);
            }
            _activeMarkers.Clear();
        }
    }
}