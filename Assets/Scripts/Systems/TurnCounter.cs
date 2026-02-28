using System;
using UnityEngine;

public class TurnCounter : MonoBehaviour {
    private int turnCount = 0;

    private Card previousPressedCard;

    private void OnEnable() {
        Card.OnAnyCardButtonPressed += Card_OnAnyCardButtonPressed;
    }

    private void OnDisable() {
        Card.OnAnyCardButtonPressed -= Card_OnAnyCardButtonPressed;
    }

    private void Card_OnAnyCardButtonPressed(object sender, EventArgs e) {
        Card pressedCard = (Card)sender;

        // Ignore if the same card is pressed twice in a row
        if (previousPressedCard == pressedCard)
            return;

        // Increment turn count
        turnCount++;

        // Update previous card
        previousPressedCard = pressedCard;

        //Debug.Log("Turn Count: " + turnCount);
    }

   
    public void ResetTurn() {
        previousPressedCard = null;
        turnCount = 0;
    }

    public int GetTurnCount() {
        return turnCount;
    }
}