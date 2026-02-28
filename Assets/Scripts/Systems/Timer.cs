using System;
using UnityEngine;

public class Timer : MonoBehaviour {
    [Header("Timer Settings")]
    [SerializeField] private float duration = 900f; //=> 15 Minutes
    [SerializeField] private bool autoStart = true;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    public float Duration => duration;  
    public float ElapsedTime => elapsedTime;

    private void Start() {
        if (autoStart)
            StartTimer();
    }

    private void Update() {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= duration) {
            isRunning = false;
            elapsedTime = duration;
        }
    }

    /// <summary>
    /// Start or restart the timer.
    /// </summary>
    public void StartTimer() {
        elapsedTime = 0f;
        isRunning = true;
    }

    /// <summary>
    /// Stop/pause the timer.
    /// </summary>
    public void StopTimer() {
        isRunning = false;
    }

    /// <summary>
    /// Reset timer without starting it.
    /// </summary>
    public void ResetTimer() {
        elapsedTime = 0f;
        isRunning = false;
    }

    /// <summary>
    /// Set a new duration for the timer.
    /// </summary>
    public void SetDuration(float newDuration) {
        duration = newDuration;
        ResetTimer();
    }
}