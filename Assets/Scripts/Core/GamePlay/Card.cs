using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private GameObject frontImage; // Reference to the front image
    [SerializeField] private GameObject backImage;  // Reference to the back image

    public int CardId { get; private set; }
    private bool isFlipped = false;

    public void Initialize(int id)
    {
        CardId = id;
        isFlipped = false;

        // Ensure only the back image is visible initially
        frontImage.SetActive(false);
        backImage.SetActive(true);
    }

    public void Flip()
    {
        if (isFlipped) return;

        isFlipped = true;

        // Flip logic: Show the front image and hide the back image
        frontImage.SetActive(true);
        backImage.SetActive(false);

        // TODO: Add animation or trigger event
    }

    public void FlipBack()
    {
        if (!isFlipped) return;

        isFlipped = false;

        // Flip logic: Show the back image and hide the front image
        frontImage.SetActive(false);
        backImage.SetActive(true);

        // TODO: Add animation or trigger event
    }
}
