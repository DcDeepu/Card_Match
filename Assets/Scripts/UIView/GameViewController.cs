using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.UI;

public class GameViewController : ViewController
{
    [SerializeField] private Transform gridContainer; // Parent for the grid
    [SerializeField] private Text scoreText;          // UI element for score
    [SerializeField] private Button restartButton;    // Restart button
    [SerializeField] private GameObject cardPrefab;   // Card prefab with front and back images

    private int currentRows;
    private int currentColumns;

    public override void Open()
    {
        base.Open();
        // Reset UI state when opened
        UpdateScore(0);
    }

    public override void ReceiveData(object data)
    {
        if (data is GridData gridData)
        {
            currentRows = gridData.Rows;
            currentColumns = gridData.Columns;

            GenerateGrid(currentRows, currentColumns);
        }
    }

    private void GenerateGrid(int rows, int columns)
    {
        // Ensure GridContainer has a GridLayoutGroup
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError("GridContainer must have a GridLayoutGroup component.");
            return;
        }

        // Clear existing children
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // Set GridLayoutGroup properties
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;

        // Calculate cell size based on container size
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        float containerWidth = gridRect.rect.width;
        float containerHeight = gridRect.rect.height;

        float cellWidth = containerWidth / columns;
        float cellHeight = containerHeight / rows;

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.spacing = new Vector2(5, 5); // Adjust as needed

        // Instantiate cards
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                GameObject card = Instantiate(cardPrefab, gridContainer);
                card.name = $"Card_{r}_{c}";

                Card cardComponent = card.GetComponent<Card>();
                cardComponent.Initialize(r * columns + c); // Assign unique ID
            }
        }
    }


    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
