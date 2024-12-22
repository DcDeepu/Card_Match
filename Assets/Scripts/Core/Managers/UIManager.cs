using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.mystery_mist.core
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager s_Instance { get; private set; }

        private Dictionary<string, ViewController> m_viewControllers = new Dictionary<string, ViewController>();
        private GameObject mainCanvas;

        // Field to hold the default view controller key
        [SerializeField]
        private string defaultViewControllerKey = "MenuViewController"; // Example default view controller

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            mainCanvas = GameObject.Find("UICanvas");

            if (mainCanvas != null)
            {
                InitializeViewControllers();
                OpenDefaultViewController();
            }
            else
            {
                Debug.LogError("MainCanvas not found in the scene. Please ensure it is instantiated by GameManager.");
            }
        }

        private void InitializeViewControllers()
        {
            // Get all ViewControllers in the mainCanvas
            ViewController[] controllers = mainCanvas.GetComponentsInChildren<ViewController>(true);

            foreach (var controller in controllers)
            {
                controller.Initialize(this);
                RegisterViewController(controller.GetType().Name, controller);
            }
        }

        private void RegisterViewController(string key, ViewController controller)
        {
            if (!m_viewControllers.ContainsKey(key))
            {
                m_viewControllers.Add(key, controller);
            }
        }

        private void OpenDefaultViewController()
        {
            if (!string.IsNullOrEmpty(defaultViewControllerKey))
            {
                OpenViewController(defaultViewControllerKey);
            }
            else
            {
                Debug.LogWarning("Default view controller key is null or empty.");
            }
        }

        public void OpenViewController(string key)
        {
            if (m_viewControllers.ContainsKey(key))
            {
                m_viewControllers[key].Open();
            }
            else
            {
                Debug.LogWarning("No ViewController found with key: " + key);
            }
        }

        public void CloseViewController(string key)
        {
            if (m_viewControllers.ContainsKey(key))
            {
                m_viewControllers[key].Close();
            }
            else
            {
                Debug.LogWarning("No ViewController found with key: " + key);
            }
        }

        public void SendDataToViewController(string key, object data)
        {
            if (m_viewControllers.ContainsKey(key))
            {
                m_viewControllers[key].ReceiveData(data);
            }
            else
            {
                Debug.LogWarning("No ViewController found with key: " + key);
            }
        }

        // New generic method to get the reference of a ViewController
        public T GetViewController<T>() where T : ViewController
        {
            string key = typeof(T).Name;
            if (m_viewControllers.ContainsKey(key))
            {
                return m_viewControllers[key] as T;
            }
            else
            {
                Debug.LogWarning("No ViewController found with key: " + key);
                return null;
            }
        }
        // Generic method to get a ViewController by string key
        public ViewController GetViewController(string viewControllerName)
        {
            if (m_viewControllers.ContainsKey(viewControllerName))
            {
                return m_viewControllers[viewControllerName];
            }
            else
            {
                Debug.LogWarning($"ViewController {viewControllerName} not found.");
                return null;
            }
        }

    }
}
