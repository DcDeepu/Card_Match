// using System.Collections.Generic;

// namespace com.mystery_mist.utils
// {

//     public class GameModel
//     {
//         public int Rows { get; private set; }
//         public int Columns { get; private set; }
//         public int Score { get; private set; }

//         private List<Card> flippedCards = new List<Card>();

//         public void Initialize(int rows, int columns)
//         {
//             Rows = rows;
//             Columns = columns;
//             Score = 0;
//             flippedCards.Clear();
//         }

//         public void HandleCardSelection(Card card)
//         {
//             if (flippedCards.Contains(card) || card.IsMatched)
//                 return;

//             flippedCards.Add(card);

//             if (flippedCards.Count == 2)
//             {
//                 CheckMatch();
//             }
//         }

//         private void CheckMatch()
//         {
//             if (flippedCards[0].CardId == flippedCards[1].CardId)
//             {
//                 flippedCards[0].SetMatched();
//                 flippedCards[1].SetMatched();
//                 Score += 10;
//             }
//             else
//             {
//                 flippedCards[0].FlipBack();
//                 flippedCards[1].FlipBack();
//                 Score -= 5;
//             }
//             flippedCards.Clear();
//         }
//     }
// }
