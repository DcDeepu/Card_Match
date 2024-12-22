using System.Collections;
using System.Collections.Generic;
using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.mystery_mist.uiview
{
    public class GameEndViewController : ViewController
    {
        // Start is called before the first frame update
        [SerializeField] private Button m_MenuButton;
        private System.Action<bool> m_OnSaveDecision; // Callback for save decision

        public void Initialize(System.Action<bool> onSaveDecision)
        {
            m_OnSaveDecision = onSaveDecision;
        }
        private void OnEnable()
        {
            m_MenuButton.onClick.AddListener(GotoMenu);
        }

        private void OnDisable()
        {
            m_MenuButton.onClick.RemoveListener(GotoMenu);
        }
        private void GotoMenu()
        {
            AudioManager.s_Instance.PlaySoundEffect(Constants.k_ClickButton);

            m_OnSaveDecision?.Invoke(false); // Notify the decision not to save
            Close(); // Close the SaveViewController
        }
    }
}