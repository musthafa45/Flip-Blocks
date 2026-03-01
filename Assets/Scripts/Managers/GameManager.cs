using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystem;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public static event Action OnGameStart;
    public event EventHandler<OnGameFinishedArgs> OnGameFinished;
    public event Action OnGameSaved;

    public event EventHandler OnCardsMismatched;
    public event EventHandler OnCardsMatched;

    public event EventHandler OnGameWin;
    public event EventHandler OnGameLose;

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
    [SerializeField] private bool autoSave = false;

    private List<Card> flippedCards = new List<Card>();

    public float CardInitialHideDelay => cardInitialHideDelay;
    public bool AutoSave => autoSave;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //SaveSystem.DeleteSave(); // Clear any existing save data for testing purposes
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
        OnGameLose?.Invoke(this, args);
    }

    public void InitializeSavedGame(GameSaveData gameSaveData) {
        gridSystem.LoadFromSave(gameSaveData);
        scoreCounter.SetScore(gameSaveData.score);
        turnCounter.SetTurnCount(gameSaveData.turns);
        turnCounter.SetTurnCountMax(gameSaveData.columns * gameSaveData.rows);
        gameTimer.SetElapsedTime(gameSaveData.elapsedTime);
        UiManager.Instance.UpdateToggleUi(new Vector2Int(gameSaveData.columns, gameSaveData.rows));
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

            first.SetActiveButton(false);  // Disable buttons to prevent further interaction with matched cards
            second.SetActiveButton(false);

            first.SetMatched(true); // Set matched state to true for both cards
            second.SetMatched(true);

            scoreCounter.AddScore(1); // Increment score for a successful match
            turnCounter.AddTurnCount(1); // Increment turn count for each pair of cards flipped

            OnCardsMatched?.Invoke(this,EventArgs.Empty); // Notify subscribers of a successful match (e.g., Audio)

            bool isWin = CheckforWin();
            if (isWin) {
                // Game win logic handled in CheckforWin method
                gameTimer.StopTimer();

                OnGameFinishedArgs args = new OnGameFinishedArgs {
                    finalTime = gameTimer.GetElapsedTime(),
                    finalScore = scoreCounter.GetScore(),
                    totalTurns = turnCounter.GetTurnCount(),
                    lossType = LossType.None
                };

                OnGameFinished?.Invoke(this, args);

                OnGameWin?.Invoke(this, args);
            }
            else {
                //Save game state after a successful match if autoSave is enabled
                if (autoSave)
                    SaveGameState();
            }
        }
        else {
            // No match, flip back after delay
            Debug.Log($"No match: {first.GetCardType()} & {second.GetCardType()}");
            
            StartCoroutine(FlipBackAnimWithDelay(first, second, flipBackDelay));

            turnCounter.AddTurnCount(1); // Increment turn count for each non pair of cards flipped

            OnCardsMismatched?.Invoke(this,EventArgs.Empty); // Notify subscribers of a mismatch (e.g., Audio)
        }

        flippedCards.Clear();
    }

    private bool CheckforWin() {
        if(gridSystem.GetTotalSlots() / 2 == scoreCounter.GetScore()) { // All pairs matched 

            Debug.Log($"You win! Time: {Mathf.RoundToInt(gameTimer.GetElapsedTime())} seconds, Turns: {turnCounter.GetTurnCount()}");

            return true;
        }
        return false;
    }

    private IEnumerator FlipBackAnimWithDelay(Card first, Card second,float delay) {
        yield return new WaitForSeconds(delay);
        first.FlipBackCard();
        second.FlipBackCard();
    }

    private void SaveGameState() {
        GameSaveData gameSaveData = SaveSystem.CreateSavableData(gridSystem.GetSlots(), gridSystem.GetGridSize().x,
                                                                 gridSystem.GetGridSize().y, scoreCounter.GetScore(),
                                                                turnCounter.GetTurnCount(),gameTimer.GetElapsedTime());

        SaveSystem.SaveGameData(gameSaveData);

        OnGameSaved?.Invoke();
    }

    public void ResetGame() {
        SaveSystem.DeleteSave();
        InitializeNewGame(gridSystem.GetGridSize());
    }

}