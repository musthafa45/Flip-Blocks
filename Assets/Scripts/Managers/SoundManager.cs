using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private SoundData[] soundatas;

    private Dictionary<SoundEffect, AudioClip> soundDictionary;

    private void Awake() {
        Instance = this;

        InitializeDictionary();
    }

    private void OnEnable() {
        Card.OnAnyCardButtonPressed += HandleCardFlip;

        if (GameManager.Instance != null) {
            GameManager.Instance.OnCardsMismatched += HandleMismatch;
            GameManager.Instance.OnCardsMatched += HandleMatch;
            GameManager.Instance.OnGameWin += HandleWin;
            GameManager.Instance.OnGameLose += HandleLose;
        }
    }

    private void OnDisable() {
        Card.OnAnyCardButtonPressed -= HandleCardFlip;

        if (GameManager.Instance != null) {
            GameManager.Instance.OnCardsMismatched -= HandleMismatch;
            GameManager.Instance.OnCardsMatched -= HandleMatch;
            GameManager.Instance.OnGameWin -= HandleWin;
            GameManager.Instance.OnGameLose -= HandleLose;
        }
    }

    private void HandleCardFlip(object sender, System.EventArgs e) {
        Play(SoundEffect.CardFlip);
    }

    private void HandleMismatch(object sender, System.EventArgs e) {
        Play(SoundEffect.Mismatch);
    }

    private void HandleMatch(object sender, System.EventArgs e) {
        Play(SoundEffect.Match);
    }

    private void HandleWin(object sender, System.EventArgs e) {
        Play(SoundEffect.GameWin);
    }

    private void HandleLose(object sender, System.EventArgs e) {
        Play(SoundEffect.GameLose);
    }


    private void InitializeDictionary() {
        soundDictionary = new Dictionary<SoundEffect, AudioClip>();

        foreach (var entry in soundatas) {
            if (!soundDictionary.ContainsKey(entry.effect)) {
                soundDictionary.Add(entry.effect, entry.clip);
            }
        }
    }

    public void Play(SoundEffect effect,float volume = 1f) {
        if (soundDictionary.TryGetValue(effect, out AudioClip clip)) {
            sfxSource.PlayOneShot(clip,volume);
        }
        else {
            Debug.LogWarning($"Sound {effect} not found!");
        }
    }
}

[System.Serializable]
public class SoundData {
    public SoundEffect effect;
    public AudioClip clip;
}
public enum SoundEffect {
    CardFlip,
    Match,
    Mismatch,
    GameWin,
    GameLose
}