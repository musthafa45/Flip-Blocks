using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }



    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TurnCounter turnCounter;
    [SerializeField] private Timer gameTimer;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Grid Size Toggles")]
    [SerializeField] private Toggle toggle_2x2;
    [SerializeField] private Toggle toggle_3x3;
    [SerializeField] private Toggle toggle_5x6;
    [SerializeField] private Toggle toggle_7x7;
    [SerializeField] private Toggle toggle_8x8;

    public event Action<Vector2Int> OnGridSizeSelected; // Event to notify selection

    private List<Toggle> toggles = new List<Toggle>();

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // Add all toggles to list
        toggles.Add(toggle_2x2);
        toggles.Add(toggle_3x3);
        toggles.Add(toggle_5x6);
        toggles.Add(toggle_7x7);
        toggles.Add(toggle_8x8);

        foreach (Toggle t in toggles) {
            t.onValueChanged.AddListener((isOn) => OnToggleChanged(t, isOn));
        }

        InitializeSelectedToggleAtStart();
    }

    private void OnToggleChanged(Toggle changedToggle, bool isOn) {
        if (!isOn) return; // Ignore when toggled off

        // Deselect all other toggles
        foreach (Toggle t in toggles) {
            if (t != changedToggle) t.isOn = false;
        }

        // Invoke event with corresponding grid size
        Vector2Int gridSize = GetGridSizeForToggle(changedToggle);
        
        GameManager.Instance.InitializeNewGame(gridSize);

        Debug.Log($"Selected Grid Size: {gridSize.x} x {gridSize.y}");
    }

    private Vector2Int GetGridSizeForToggle(Toggle toggle) {
        if (toggle == toggle_2x2) return new Vector2Int(2, 2);
        if (toggle == toggle_3x3) return new Vector2Int(3, 3);
        if (toggle == toggle_5x6) return new Vector2Int(5, 6);
        if (toggle == toggle_7x7) return new Vector2Int(7, 7);
        if (toggle == toggle_8x8) return new Vector2Int(8, 8);
        return new Vector2Int(2, 2); // default fallback
    }

    private void InitializeSelectedToggleAtStart() {
        Toggle selectedToggle = null;

        // Find selected toggle
        foreach (Toggle t in toggles) {
            if (t.isOn) {
                selectedToggle = t;
                break;
            }
        }

        // If none selected, select default (2x2)
        if (selectedToggle == null) {
            selectedToggle = toggle_2x2;
            selectedToggle.isOn = true;
        }

        Vector2Int gridSize = GetGridSizeForToggle(selectedToggle);

        GameManager.Instance.InitializeNewGame(gridSize);

        Debug.Log($"Initial Grid Size: {gridSize.x} x {gridSize.y}");
    }

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