using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.mystery_mist.core
{

    public abstract class ViewController : MonoBehaviour
    {
        protected UIManager uiManager;

        // Optional: Assign any dependencies here
        public void Initialize(UIManager manager)
        {
            uiManager = manager;
            OnInitialized();
        }

        protected virtual void OnInitialized()
        {
            // Override to perform additional setup
        }

        // Method to open the view
        public virtual void Open()
        {
            gameObject.SetActive(true);
            OnOpened();
        }

        // Method to close the view
        public virtual void Close()
        {
            gameObject.SetActive(false);
            OnClosed();
        }

        // Override this method to handle data passed to the view controller
        public virtual void ReceiveData(object data)
        {
            // Handle data
        }

        protected virtual void OnOpened()
        {
            // Override to handle logic when the view is opened
        }

        protected virtual void OnClosed()
        {
            // Override to handle logic when the view is closed
        }
    }
}