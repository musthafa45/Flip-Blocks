using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TurnCounter turnCounter;
    [SerializeField] private Timer gameTimer;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update() {
        UpdateTimerUi();

        UpdateScoreUi();

        UpdateTurnUi();

    }

    private void UpdateTimerUi() {
        if (gameTimer != null) {
            float remainingTime = Mathf.Max(0f, gameTimer.ElapsedTime);
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else {
            timerText.text = "00:00";
        }
    }

    private void UpdateScoreUi() {
        if (scoreCounter != null) {
            scoreText.text = $"Score: {scoreCounter.GetScore()}";
        }
        else {
            scoreText.text = "Score: 0";
        }
    }

    private void UpdateTurnUi() {
        if (turnCounter != null) {
            turnText.text = $"Turn: {turnCounter.GetTurnCount()}";
        }
        else {
            turnText.text = "Turn: 0";
        }
    }
}