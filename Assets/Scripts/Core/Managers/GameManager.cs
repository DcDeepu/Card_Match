using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.mystery_mist.core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager s_Instance;
        public ScenePrefabConfiguration m_ScenePrefabsConfig;
        // Managers
        private UIManager m_UIManager;
        private AudioManager m_AudioManager;


        private bool m_AllowFlipping = true;

        public bool AllowFlipping
        {
            get => m_AllowFlipping;
            set
            {
                m_AllowFlipping = value;
                Debug.Log($"AllowFlipping set to: {m_AllowFlipping}");
            }
        }
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                InstantiateScenePrefabs();
                InitializeManagers();
            }
        }

        private void InstantiateScenePrefabs()
        {
            if (m_ScenePrefabsConfig == null)
            {
                Debug.LogWarning("SceneSettings is not assigned.");
                return;
            }

            foreach (var prefab in m_ScenePrefabsConfig.PrefabsToInstantiate)
            {
                if (prefab != null)
                {
                    Instantiate(prefab).name = prefab.name;
                }
                else
                {
                    Debug.LogWarning("Prefab in SceneSettings is null.");
                }
            }
        }
        private void InitializeManagers()
        {
            m_UIManager = FindOrCreateManager<UIManager>();
            m_AudioManager = FindOrCreateManager<AudioManager>();

        }

        private T FindOrCreateManager<T>() where T : MonoBehaviour
        {
            T manager = FindObjectOfType<T>();
            if (manager == null)
            {
                GameObject managerObject = new GameObject(typeof(T).Name);
                manager = managerObject.AddComponent<T>();
                RegisterManager(manager);
            }
            return manager;
        }

        private void RegisterManager(MonoBehaviour manager)
        {
            if (manager is UIManager uiManager) m_UIManager = uiManager;
            else if (manager is AudioManager audioManager) m_AudioManager = audioManager;
        }


        public void ShowGameView(int rows, int columns)
        {
            m_UIManager.CloseViewController(Constants.k_MenuViewController);
            m_UIManager.OpenViewController(Constants.k_GameViewController);
            // Notify the UIManager to open the GameViewController with grid size
            m_UIManager.SendDataToViewController(Constants.k_GameViewController, new GridData(rows, columns));
        }

    }

    public class GridData
    {
        public int Rows { get; }
        public int Columns { get; }

        public GridData(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }
    }

    [Serializable]
    public class CardData
    {
        public string Id; // Unique identifier
        public string DataType; // Data type (e.g., "Color")
        public Color Value; // The actual data (e.g., a Color)
    }




}

