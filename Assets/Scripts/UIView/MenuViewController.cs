using System.Collections;
using System.Collections.Generic;
using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.mystery_mist.uiview
{

    public class MenuViewController : ViewController
    {
        [SerializeField] private Button m_Grid2x2Button;
        [SerializeField] private Button m_Grid2x3Button;
        [SerializeField] private Button m_Grid5x6Button;

        private void Start()
        {
            // Set up button listeners
            m_Grid2x2Button.onClick.AddListener(() => OnGridSizeSelected(2, 2));
            m_Grid2x3Button.onClick.AddListener(() => OnGridSizeSelected(2, 3));
            m_Grid5x6Button.onClick.AddListener(() => OnGridSizeSelected(5, 6));
        }

        private void OnGridSizeSelected(int rows, int columns)
        {
            GameManager.s_Instance.ShowGameView(rows, columns);
        }
    }
}
