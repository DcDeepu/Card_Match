using System.Collections;
using System.Collections.Generic;
using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.mystery_mist.uiview
{

    public class MenuViewController : ViewController
    {
        [SerializeField] private Button grid2x2Button;
        [SerializeField] private Button grid2x3Button;
        [SerializeField] private Button grid5x6Button;

        private void Start()
        {
            // Set up button listeners
            grid2x2Button.onClick.AddListener(() => OnGridSizeSelected(2, 2));
            grid2x3Button.onClick.AddListener(() => OnGridSizeSelected(2, 3));
            grid5x6Button.onClick.AddListener(() => OnGridSizeSelected(5, 6));
        }

        private void OnGridSizeSelected(int rows, int columns)
        {
            GameManager.s_Instance.ShowGameView(rows, columns);
        }
    }
}
