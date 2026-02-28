using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinOrLossUi : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image BgImage;
    [SerializeField] private Transform winLossUiParent;
    [SerializeField] private Button okButton;

    private void Awake() {
        okButton.onClick.AddListener(()=> {
            Hide();
        });

        Hide();
    }

    private void Start() {
        GameManager.Instance.OnGameFinished += (sender, args) =>
        {
            ShowWinOrLoss(args.lossType, args.finalScore, args.totalTurns, args.finalTime);
        };
    }

    private void ShowWinOrLoss(LossType lossType, int score, int turns, float time) {
        resultText.text = lossType == LossType.None ? "You Win" : "You Lose";
        scoreText.text = $"Score: {score}";
        turnText.text = $"Turns: {turns}";

        var (minutes, seconds) = FormatTime(time);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";

        Show();
    }

    private (int minutes, int seconds) FormatTime(float time) {
        float clampedTime = Mathf.Max(0f, time);

        int minutes = Mathf.FloorToInt(clampedTime / 60f);
        int seconds = Mathf.FloorToInt(clampedTime % 60f);

        return (minutes, seconds);
    }

    public void Hide() {
        winLossUiParent.gameObject.SetActive(false);
        BgImage.enabled = false;
        canvasGroup.enabled = false;
    }

    public void Show() {
        winLossUiParent.gameObject.SetActive(true);
        BgImage.enabled = true;
        canvasGroup.enabled = true;
    }

}

public enum LossType {
    None,
    TimeUp,
    TooManyTurns
}
