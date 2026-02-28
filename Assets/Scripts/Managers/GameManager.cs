using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Timer gameTimer;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TurnCounter turnCounter;

    [SerializeField] private float flipBackDelay = 1f; // delay before flipping back unmatched cards

    private List<Card> flippedCards = new List<Card>();

    private void Awake() {
        Instance = this;    
    }

    private void Start() {
        gameTimer?.StartTimer();
    }

    public void InitializeNewGame(Vector2Int gridSize) {
        gridSystem.InitializeGrid(gridSize);
        gameTimer.ResetTimer();
        gameTimer.StartTimer();
        scoreCounter.ResetScore();
        turnCounter.ResetTurn();
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
            Debug.Log($"Matched: {first.GetCardType()} & {second.GetCardType()}");

            first.DisableButton();
            second.DisableButton();

            scoreCounter.AddScore(1); // Increment score for a match

            CheckforWin();
        }
        else {
            // No match, flip back after delay
            Debug.Log($"No match: {first.GetCardType()} & {second.GetCardType()}");
            
            StartCoroutine(FlipBackAnimWithDelay(first, second, 0.5f ));
        }

        flippedCards.Clear();
    }

    private void CheckforWin() {
        if(gridSystem.GetTotalSlots() / 2 == scoreCounter.GetScore()) { // All pairs matched 
            gameTimer.StopTimer();
            Debug.Log($"You win! Time: {Mathf.RoundToInt(gameTimer.ElapsedTime)} seconds, Turns: {turnCounter.GetTurnCount()}");
        }
    }

    private IEnumerator FlipBackAnimWithDelay(Card first, Card second,float delay) {
        yield return new WaitForSeconds(delay);
        first.FlipBackCard();
        second.FlipBackCard();
    }
}