using System;
using UnityEngine;

public class TurnCounter : MonoBehaviour {
    private int turnCount = 0;
    private int turnCountMax = 0;

    public void ResetTurn(int totalSlots) {
        turnCount = 0;

        int pairs = totalSlots / 2; // Each pair consists of 2 cards, so total pairs is half the total slots
        turnCountMax = pairs * 2; // Max turns is 2 per pair (one for each card in the pair)
    }

    public int GetTurnCountMax() {
        return turnCountMax;
    }

    public void AddTurnCount(int turnCount) {
        this.turnCount++;

        CheckForGameOver();
    }

    public int GetTurnCount() {
        return turnCount;
    }

    private void CheckForGameOver() {
        if(turnCount >= turnCountMax) {
            // Trigger game over due to too many turns
            GameManager.Instance.GameOver(LossType.TooManyTurns);
        }
    }

   
}