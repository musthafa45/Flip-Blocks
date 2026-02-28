using System;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    private int score = 0;

    public void AddScore(int score) {
        this.score += score;
    }
    public int GetScore() {
        return score;
    }

    public void ResetScore() {
        score = 0;
    }
}
