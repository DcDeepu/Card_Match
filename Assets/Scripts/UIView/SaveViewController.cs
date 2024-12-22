using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.mystery_mist.uiview
{

    public class SaveViewController : ViewController
    {
        [SerializeField] private Button m_SaveButton;
        [SerializeField] private Button m_DontSaveButton;

        private System.Action<bool> m_OnSaveDecision; // Callback for save decision

        public void Initialize(System.Action<bool> onSaveDecision)
        {
            m_OnSaveDecision = onSaveDecision;
        }

        private void OnEnable()
        {
            m_SaveButton.onClick.AddListener(SaveGame);
            m_DontSaveButton.onClick.AddListener(DontSaveGame);
        }

        private void OnDisable()
        {
            m_SaveButton.onClick.RemoveListener(SaveGame);
            m_DontSaveButton.onClick.RemoveListener(DontSaveGame);
        }

        private void SaveGame()
        {
            m_OnSaveDecision?.Invoke(true); // Notify the decision to save
            Close(); // Close the SaveViewController
        }

        private void DontSaveGame()
        {
            m_OnSaveDecision?.Invoke(false); // Notify the decision not to save
            Close(); // Close the SaveViewController
        }
    }
}
