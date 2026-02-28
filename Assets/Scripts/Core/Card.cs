using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    public static event EventHandler OnAnyCardButtonPressed;

    public class OnAnyCardButtonPressedArgs : EventArgs {
        public Card card;
    }

    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Button button;

    private CardType cardType;
    private Coroutine flipRoutine;

    private bool isFlipped = false;
    private bool isMatched = false;

    private void Awake() {
        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnDisable() {
        button.onClick.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed() {
        if (isFlipped || isMatched) return;

        FlipCard();
        OnAnyCardButtonPressed?.Invoke(this, new OnAnyCardButtonPressedArgs { card = this });
    }

    public void InitializeCard(CardType type = CardType.None) {
        cardType = type;
        UpdateUi(type);
        isFlipped = false;
        isMatched = false;
        numberText.gameObject.SetActive(false); // hide number initially
        transform.localScale = Vector3.one; // reset scale
    }

    public CardType GetCardType() => cardType;

    private void UpdateUi(CardType type) {
        numberText.text = ((int)type).ToString();
    }

    public void FlipCard() {
        if (flipRoutine != null) StopCoroutine(flipRoutine);
        flipRoutine = StartCoroutine(FlipAnimation(showNumber: true));
        isFlipped = true;
    }

    public void FlipBackCard(float delay = 0.5f) {
        if (flipRoutine != null) StopCoroutine(flipRoutine);
        flipRoutine = StartCoroutine(FlipBackAnimation(delay));
        isFlipped = false;
    }

    public void SetMatched() {
        isMatched = true;
    }

    private IEnumerator FlipAnimation(bool showNumber) {
        float duration = 0.25f;
        Vector3 startScale = transform.localScale;
        Vector3 midScale = new Vector3(0f, startScale.y, startScale.z);

        // Flip to middle (scale X = 0)
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, midScale, t);
            yield return null;
        }

        // Reveal number at middle
        numberText.gameObject.SetActive(showNumber);

        // Flip back to normal (scale X = 1)
        elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(midScale, startScale, t);
            yield return null;
        }

        transform.localScale = startScale;
    }

    private IEnumerator FlipBackAnimation(float delay) {
        yield return new WaitForSeconds(delay);
        yield return FlipAnimation(showNumber: false); // hide number when flipping back
    }
}

public enum CardType {
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9
}
