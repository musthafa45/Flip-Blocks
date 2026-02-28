using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public static event EventHandler OnAnyCardButtonPressed;

    public class OnAnyCardButtonPressedArgs : EventArgs {
        public Card card;
    }

    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Button button;

    private CardType cardType;

    public void InitializeCard(CardType type = CardType.None) {
        cardType = type;
        UpdateUi(type);
    }
    public CardType GetCardType() {
        return cardType;
    }

    private void UpdateUi(CardType type) {
        numberText.text = ((int)type).ToString();
    }

    private void Awake() {
        button.onClick.AddListener(() => {
            OnAnyCardButtonPressed?.Invoke(this, new OnAnyCardButtonPressedArgs { card = this });
        });
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
