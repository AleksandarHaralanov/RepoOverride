using System;
using UnityEngine;

namespace RepoOverride
{
    public class Loader
    {
        private static GameObject _gameObject;

        public static void Init()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                if (_gameObject == null)
                {
                    _gameObject = new GameObject(Hax.NAME);
                    _gameObject.AddComponent<Hax>();
                    GameObject.DontDestroyOnLoad(_gameObject);
                }
            }
            catch (Exception) { }
        }

        public static void Unload()
        {
            try
            {
                if (_gameObject != null)
                {
                    GameObject.Destroy(_gameObject);
                    _gameObject = null;
                }
            }
            catch (Exception) { }
        }
    }
}