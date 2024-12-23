using System.Collections;
using com.mystery_mist.core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject frontImage; // Reference to the front image
    [SerializeField] private GameObject backImage;  // Reference to the back image
    [SerializeField] private Text frontText; // Optional, for text data
    public CardData CardData { get; private set; }
    public int CardId { get; private set; }
    public bool IsFlipped { get; private set; } = false; // Track whether the card is flipped

    private bool m_IsAnimating = false; // Prevent multiple animations at once
    private Coroutine m_CardAnimCoroutine;
    public delegate void CardTappedHandler(Card card);
    public static event CardTappedHandler OnCardTapped;


    public void Initialize(CardData cardData)
    {
        CardData = cardData;
        IsFlipped = false;

        // Set the card's front content based on the data type
        if (cardData.DataType == "Color" && cardData.Value is Color color)
        {
            frontImage.GetComponent<Image>().color = color; // Assign the color to the Image
        }
        else
        {
            Debug.LogError($"Unsupported CardData type: {cardData.DataType}");
        }

        // Ensure the back side is initially visible
        frontImage.SetActive(false);
        backImage.SetActive(true);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // OnCardTapped();
        // Prevent flipping if animating or flipping is not allowed
        if (m_IsAnimating || !GameManager.s_Instance.AllowFlipping || IsFlipped) return;

        Flip();

    }
    public void ResetCard()
    {
        // Flip();
        StartCoroutine(FlipCardSilently());
    }

    public void Flip()
    {
        if (m_CardAnimCoroutine != null)
            StopCoroutine(m_CardAnimCoroutine);
        m_CardAnimCoroutine = StartCoroutine(FlipCard(true));
    }

    private IEnumerator FlipCard(bool IsScored)
    {
        m_IsAnimating = true;

        // Flip animation: Rotate the card 90 degrees on Y-axis
        float duration = 0.3f; // Duration of the flip animation
        float halfDuration = duration / 2f;
        float elapsedTime = 0f;

        // First half of the animation (rotate to 90 degrees)
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(0, 90, elapsedTime / halfDuration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }

        // Swap the visible side
        OnCardTapped?.Invoke(this); // Notify listeners
        AudioManager.s_Instance.PlaySoundEffect(Constants.k_CardFlip);
        IsFlipped = !IsFlipped;
        frontImage.SetActive(IsFlipped);
        backImage.SetActive(!IsFlipped);

        // Second half of the animation (rotate back to 0 degrees)
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(90, 0, elapsedTime / halfDuration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }

        m_IsAnimating = false;
    }
    private IEnumerator FlipCardSilently()
    {
        m_IsAnimating = true;

        // Flip animation: Rotate the card 90 degrees on Y-axis
        float duration = 0.3f; // Duration of the flip animation
        float halfDuration = duration / 2f;
        float elapsedTime = 0f;

        // First half of the animation (rotate to 90 degrees)
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(0, 90, elapsedTime / halfDuration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }

        // Swap the visible side
        AudioManager.s_Instance.PlaySoundEffect(Constants.k_CardFlip);
        IsFlipped = !IsFlipped;
        frontImage.SetActive(IsFlipped);
        backImage.SetActive(!IsFlipped);

        // Second half of the animation (rotate back to 0 degrees)
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(90, 0, elapsedTime / halfDuration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }

        m_IsAnimating = false;
    }

}
