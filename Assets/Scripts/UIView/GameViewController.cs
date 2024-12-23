using System;
using System.Collections;
using System.Collections.Generic;
using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.mystery_mist.uiview
{

    public class GameViewController : ViewController
    {
        [SerializeField] private Transform m_GridContainer; // Parent for the grid
        [SerializeField] private Text m_ScoreText;          // UI element for score
        [SerializeField] private Button m_RestartButton, m_MenuButton;    // Restart and Menu buttons
        [SerializeField] private GameObject m_CardPrefab;   // Card prefab with front and back images
        private List<CardData> m_CardDataList = new List<CardData>();

        private bool m_IsGameLoaded = false; // Tracks if the game was loaded


        private List<Card> m_FlippedCards = new List<Card>();

        private int m_CurrentRows;
        private int m_CurrentColumns;
        private int m_Score = 0; // Track the player's score

        private void OnEnable()
        {
            Card.OnCardTapped += HandleCardTapped; // Subscribe to the Card event
            m_RestartButton.onClick.AddListener(RestartGame); // Attach RestartGame to Restart button
            m_MenuButton.onClick.AddListener(GoToMenu); // Attach GoToMenu to Menu button
        }

        private void OnDisable()
        {
            Card.OnCardTapped -= HandleCardTapped; // Unsubscribe to prevent memory leaks
            m_RestartButton.onClick.RemoveListener(RestartGame); // Detach listeners
            m_MenuButton.onClick.RemoveListener(GoToMenu);
        }

        public override void ReceiveData(object data)
        {
            if (data is GridData gridData)
            {
                m_CurrentRows = gridData.Rows;
                m_CurrentColumns = gridData.Columns;

                if (IsSavedGameAvailable() && IsSavedGridSizeMatching(m_CurrentRows, m_CurrentColumns))
                {
                    LoadGame();
                }
                else
                {
                    GenerateCardData(m_CurrentRows, m_CurrentColumns);
                    GenerateGrid(m_CurrentRows, m_CurrentColumns);
                    StartCoroutine(PreGameReveal()); // Show cards before the game starts
                }
            }
        }

        private IEnumerator PreGameReveal()
        {
            Debug.Log("Revealing all cards before game starts...");
            // Flip all cards
            foreach (Transform child in m_GridContainer)
            {
                Card card = child.GetComponent<Card>();
                if (card != null)
                {
                    card.ResetCard();
                }
            }

            yield return new WaitForSeconds(3f); // Delay to show the cards

            // Flip all cards back
            foreach (Transform child in m_GridContainer)
            {
                Card card = child.GetComponent<Card>();
                if (card != null)
                {
                    card.ResetCard();
                }
            }

            Debug.Log("Game starts now!");
        }
        private void LoadGame()
        {
            string json = PlayerPrefs.GetString(Constants.k_SaveGame);
            SavedGameData saveData = JsonUtility.FromJson<SavedGameData>(json);

            m_CurrentRows = saveData.Rows;
            m_CurrentColumns = saveData.Columns;
            m_Score = saveData.Score;
            UpdateScore(m_Score);

            m_CardDataList = saveData.Cards.ConvertAll(savedCard => new CardData
            {
                Id = savedCard.Id,
                DataType = savedCard.DataType,
                Value = savedCard.Value
            });

            GenerateGrid(m_CurrentRows, m_CurrentColumns);

            foreach (Transform child in m_GridContainer)
            {
                Card card = child.GetComponent<Card>();
                if (card != null && saveData.Cards.Exists(savedCard => savedCard.IsFlipped && savedCard.Id == card.CardData.Id))
                {
                    card.Flip();
                }
            }
            m_IsGameLoaded = true;
            Debug.Log("Game loaded successfully!");
        }

        private bool IsSavedGameAvailable()
        {
            return PlayerPrefs.HasKey(Constants.k_SaveGame);
        }
        private void SaveGame()
        {
            SavedGameData saveData = new SavedGameData
            {
                Rows = m_CurrentRows,
                Columns = m_CurrentColumns,
                Score = m_Score,
                Cards = m_CardDataList.ConvertAll(cardData => new SavedCardData
                {
                    Id = cardData.Id, // Save the unique Id
                    DataType = cardData.DataType,
                    Value = cardData.Value,
                    IsFlipped = IsCardFlipped(cardData)
                })
            };

            string json = JsonUtility.ToJson(saveData);
            Debug.Log(json);
            PlayerPrefs.SetString(Constants.k_SaveGame, json);
            PlayerPrefs.Save();

            Debug.Log("Game saved successfully!");
        }


        private void GenerateCardData(int rows, int columns)
        {
            m_CardDataList.Clear();

            int totalCards = rows * columns;

            if (totalCards % 2 != 0)
            {
                Debug.LogError("Grid size must result in an even number of cards.");
                return;
            }

            int pairs = totalCards / 2;

            Color[] colors = {
        Color.red, Color.green, Color.blue, Color.yellow,
        Color.cyan, Color.magenta, Color.gray,
        new Color(1.0f, 0.5f, 0.0f),  // Orange
        new Color(0.5f, 0.0f, 0.5f),  // Purple
        new Color(0.5f, 0.5f, 0.5f),  // Dark Gray
        new Color(0.0f, 0.5f, 1.0f),  // Sky Blue
        new Color(1.0f, 0.8f, 0.2f),  // Gold
        new Color(0.0f, 1.0f, 0.5f),  // Spring Green
        new Color(0.5f, 1.0f, 0.5f),  // Light Green
        new Color(1.0f, 0.0f, 0.5f),  // Rose
        new Color(0.5f, 0.2f, 0.0f)   // Brown
    };

            if (pairs > colors.Length)
            {
                Debug.LogError("Not enough colors to generate pairs for the grid.");
                return;
            }

            for (int i = 0; i < pairs; i++)
            {
                string id1 = Guid.NewGuid().ToString();
                string id2 = Guid.NewGuid().ToString();

                m_CardDataList.Add(new CardData { Id = id1, DataType = "Color", Value = colors[i] });
                m_CardDataList.Add(new CardData { Id = id2, DataType = "Color", Value = colors[i] });
            }

            // Shuffle the card data randomly
            for (int i = 0; i < m_CardDataList.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, m_CardDataList.Count);
                (m_CardDataList[i], m_CardDataList[randomIndex]) = (m_CardDataList[randomIndex], m_CardDataList[i]);
            }
        }


        private void GenerateGrid(int rows, int columns)
        {
            GridLayoutGroup gridLayout = m_GridContainer.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                Debug.LogError("GridContainer must have a GridLayoutGroup component.");
                return;
            }

            // Clear existing children
            foreach (Transform child in m_GridContainer)
            {
                Destroy(child.gameObject);
            }

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;

            RectTransform gridRect = m_GridContainer.GetComponent<RectTransform>();
            float containerWidth = gridRect.rect.width;
            float containerHeight = gridRect.rect.height;

            float cellWidth = containerWidth / columns;
            float cellHeight = containerHeight / rows;

            gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayout.spacing = new Vector2(5, 5); // Adjust as needed

            int index = 0; // Index to track the current card data
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (index >= m_CardDataList.Count)
                    {
                        Debug.LogError("CardDataList does not contain enough data for the grid.");
                        return;
                    }

                    GameObject cardObj = Instantiate(m_CardPrefab, m_GridContainer);
                    cardObj.name = $"Card_{r}_{c}";

                    Card cardComponent = cardObj.GetComponent<Card>();
                    cardComponent.Initialize(m_CardDataList[index]);

                    index++;
                }
            }
        }

        private void HandleCardTapped(Card card)
        {
            Debug.Log($"Card tapped: {card.name}");

            if (m_FlippedCards.Contains(card))
            {
                Debug.Log("Card is already flipped.");
                return;
            }

            if (card.IsFlipped)
            {
                Debug.Log("Card is already flipped and displayed.");
                return;
            }
            m_FlippedCards.Add(card);
            Debug.Log($"Card added to flippedCards. Total flipped cards: {m_FlippedCards.Count}");

            if (m_FlippedCards.Count == 2)
            {
                StartCoroutine(CheckMatch());
            }
        }

        private IEnumerator CheckMatch()
        {
            GameManager.s_Instance.AllowFlipping = false; // Disable flipping
            yield return new WaitForSeconds(1f); // Wait to show both cards

            if (m_FlippedCards[0].CardData.Value.Equals(m_FlippedCards[1].CardData.Value))
            {
                Debug.Log("Match Found!");
                // Only update score if the game was not loaded from a save
                if (!m_IsGameLoaded)
                {
                    m_Score++; // Increment score for a new match
                    UpdateScore(m_Score); // Update score UI
                }
                m_FlippedCards.Clear();
                AudioManager.s_Instance.PlaySoundEffect(Constants.k_Matched);

                // Check if all cards are matched
                if (AreAllPairsMatched())
                {
                    Debug.Log("All pairs matched! Opening GameEndViewController.");
                    GameEndViewController saveView = UIManager.s_Instance.GetViewController<GameEndViewController>();
                    saveView.Initialize(OnSaveDecision);

                    UIManager.s_Instance.OpenViewController(Constants.k_GameEndViewController);
                }
            }
            else
            {
                Debug.Log("No Match!");
                foreach (var card in m_FlippedCards)
                {
                    card.ResetCard();
                }
                m_FlippedCards.Clear();
            }
            m_IsGameLoaded = false;
            GameManager.s_Instance.AllowFlipping = true; // Disable flipping
        }

        private bool AreAllPairsMatched()
        {
            foreach (Transform child in m_GridContainer)
            {
                Card card = child.GetComponent<Card>();
                if (card != null && !card.IsFlipped)
                {
                    return false; // If any card is not flipped, the game is not complete
                }
            }
            return true; // All cards are flipped
        }
        public void UpdateScore(int score)
        {
            m_ScoreText.text = $"Score: {score}";
        }

        public void RestartGame()
        {
            AudioManager.s_Instance.PlaySoundEffect(Constants.k_ClickButton);

            m_Score = 0; // Reset score
            UpdateScore(m_Score);

            m_FlippedCards.Clear(); // Clear flipped cards
            GenerateCardData(m_CurrentRows, m_CurrentColumns); // Regenerate card data
            GenerateGrid(m_CurrentRows, m_CurrentColumns); // Recreate the grid

            Debug.Log("Game restarted!");
        }

        private void GoToMenu()
        {
            Debug.Log("Opening SaveViewController...");
            AudioManager.s_Instance.PlaySoundEffect(Constants.k_ClickButton);

            // Open SaveViewController
            SaveViewController saveView = UIManager.s_Instance.GetViewController<SaveViewController>();
            saveView.Initialize(OnSaveDecision);

            UIManager.s_Instance.OpenViewController(Constants.k_SaveViewController);
        }

        private void OnSaveDecision(bool save)
        {
            if (save)
            {
                SaveGame(); // Save the game state
            }
            else
            {
                foreach (Transform child in m_GridContainer)
                {
                    Card card = child.GetComponent<Card>();
                    if (card != null)
                    {
                        card.ResetCard();
                    }
                }
                ClearSaveGameData();
            }

            // Clear current state and navigate to the menu
            m_FlippedCards.Clear();
            m_Score = 0;
            UpdateScore(m_Score);

            Debug.Log("Returning to the main menu...");
            UIManager.s_Instance.CloseViewController(Constants.k_GameViewController);
            UIManager.s_Instance.OpenViewController(Constants.k_MenuViewController);
        }


        private void ClearSaveGameData()
        {
            if (PlayerPrefs.HasKey(Constants.k_SaveGame))
            {
                PlayerPrefs.DeleteKey(Constants.k_SaveGame);
                PlayerPrefs.Save();
                Debug.Log("Save game data cleared successfully.");
            }
            else
            {
                Debug.Log("No save game data found to clear.");
            }
        }
        private bool IsCardFlipped(CardData cardData)
        {
            foreach (Transform child in m_GridContainer)
            {
                Card card = child.GetComponent<Card>();
                if (card != null && card.CardData == cardData && card.IsFlipped)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSavedGridSizeMatching(int rows, int columns)
        {
            string json = PlayerPrefs.GetString(Constants.k_SaveGame);
            if (string.IsNullOrEmpty(json)) return false;

            SavedGameData saveData = JsonUtility.FromJson<SavedGameData>(json);

            // Compare the saved grid size with the current grid size
            return saveData.Rows == rows && saveData.Columns == columns;
        }

    }




    [Serializable]
    public class SavedGameData
    {
        public int Rows;
        public int Columns;
        public int Score;
        public List<SavedCardData> Cards;
    }

    [Serializable]
    public class SavedCardData
    {
        public string Id; // Unique identifier for the card
        public string DataType;
        public Color Value;
        public bool IsFlipped;
    }
}
