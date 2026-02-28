using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public static event Action OnGameStart;
    public event EventHandler<OnGameFinishedArgs> OnGameFinished;

    public class OnGameFinishedArgs : EventArgs {
        public float finalTime;
        public int finalScore;
        public int totalTurns;
        public LossType lossType;
    }

    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Timer gameTimer;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TurnCounter turnCounter;

    [SerializeField] private float cardInitialHideDelay = 3f; // delay before cards are initially hidden at game start
    [SerializeField] private float flipBackDelay = 1f; // delay before flipping back unmatched cards

    private List<Card> flippedCards = new List<Card>();

    public float CardInitialHideDelay => cardInitialHideDelay;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        gameTimer?.StartTimer();

        Card.OnAnyCardButtonPressed += Card_OnAnyCardButtonPressed;
    }

    public void InitializeNewGame(Vector2Int gridSize) {
        // initialize grid, 
        gridSystem.InitializeGrid(gridSize);

        // reset timer
        gameTimer.ResetTimer();

        // reset score
        gameTimer.StartTimer();

        // reset score
        scoreCounter.ResetScore();

        // reset turns
        turnCounter.ResetTurn(gridSize.x * gridSize.y);

        // Notify subscribers that a new game has started
        OnGameStart?.Invoke();
    }

    public void GameOver(LossType lossType) {
        gameTimer.StopTimer();
        Debug.Log($"You win! Time: {Mathf.RoundToInt(gameTimer.GetElapsedTime())} seconds, Turns: {turnCounter.GetTurnCount()}");

        OnGameFinishedArgs args = new OnGameFinishedArgs {
            finalTime = gameTimer.GetElapsedTime(),
            finalScore = scoreCounter.GetScore(),
            totalTurns = turnCounter.GetTurnCount(),
            lossType = lossType
        };

        OnGameFinished?.Invoke(this, args);
    }


    private void OnDisable() {
        Card.OnAnyCardButtonPressed -= Card_OnAnyCardButtonPressed;
    }

    private void OnDestroy() {
        OnGameStart = null; // Clear event subscribers to prevent memory leaks
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

            first.SetActiveButton(false);
            second.SetActiveButton(false);

            scoreCounter.AddScore(1); // Increment score for a match

            CheckforWin();
        }
        else {
            // No match, flip back after delay
            Debug.Log($"No match: {first.GetCardType()} & {second.GetCardType()}");
            
            StartCoroutine(FlipBackAnimWithDelay(first, second, flipBackDelay));

            turnCounter.AddTurnCount(1); // Decrement turns for a mismatch
        }

        flippedCards.Clear();
    }

    private void CheckforWin() {
        if(gridSystem.GetTotalSlots() / 2 == scoreCounter.GetScore()) { // All pairs matched 
            gameTimer.StopTimer();
            Debug.Log($"You win! Time: {Mathf.RoundToInt(gameTimer.GetElapsedTime())} seconds, Turns: {turnCounter.GetTurnCount()}");

            OnGameFinishedArgs args = new OnGameFinishedArgs {
                finalTime = gameTimer.GetElapsedTime(),
                finalScore = scoreCounter.GetScore(),
                totalTurns = turnCounter.GetTurnCount(),
                lossType = LossType.None
            };

            OnGameFinished?.Invoke(this, args);
        }
    }

    private IEnumerator FlipBackAnimWithDelay(Card first, Card second,float delay) {
        yield return new WaitForSeconds(delay);
        first.FlipBackCard();
        second.FlipBackCard();
    }

}