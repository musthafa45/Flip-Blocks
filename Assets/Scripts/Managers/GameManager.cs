using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [SerializeField] private Timer gameTimer;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private float flipBackDelay = 1f; // delay before flipping back unmatched cards

    private List<Card> flippedCards = new List<Card>();

    private void Awake() {
        Instance = this;    
    }

    private void Start() {
        gameTimer?.StartTimer();
    }

    public void SetGridSizeSelected(Vector2Int gridSize) {
        gridSystem.InitializeGrid(gridSize);
    }

    private void OnEnable() {
        Card.OnAnyCardButtonPressed += Card_OnAnyCardButtonPressed;
    }


    private void OnDisable() {
        Card.OnAnyCardButtonPressed -= Card_OnAnyCardButtonPressed;
    }

    private void Card_OnAnyCardButtonPressed(object sender, EventArgs e) {

        Card pressedCard = (Card)sender;

        // Ignore already flipped cards
        if (flippedCards.Contains(pressedCard)) return;

        // Show card (your card UI logic could be handled inside Card class)
        pressedCard.GetCardType();

        flippedCards.Add(pressedCard);

        if (flippedCards.Count == 2) {
           CheckMatch();
        }
    }

    private void CheckMatch() {
        Card first = flippedCards[0];
        Card second = flippedCards[1];

        if (first.GetCardType() == second.GetCardType()) {
            // Match found
            Debug.Log($"Matched: {first.GetCardType()}");
            first.DisableButton();
            second.DisableButton();
            // Optionally mark cards as matched so they cannot be clicked again
        }
        else {
            // No match, flip back after delay
            Debug.Log($"No match: {first.GetCardType()} & {second.GetCardType()}");
            // Reset cards (you could have a FlipBack() method in Card)
            //first.FlipCard();
            //second.InitializeCard(CardType.None);
        }

        flippedCards.Clear();
    }
}