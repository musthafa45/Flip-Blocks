using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private SoundData[] soundatas;
    [SerializeField] private AudioSource gameAmbienceSource;
    [SerializeField] private float ambienceMusicVolume = 0.5f;

    private Dictionary<SoundEffect, AudioClip> soundDictionary;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDictionary();
    }

    private void Start() {
        Card.OnAnyCardButtonPressed += (sender, args) => Play(SoundEffect.CardFlip);
        GameManager.Instance.OnCardsMismatched += (sender, args) => Play(SoundEffect.Mismatch);
        GameManager.Instance.OnCardsMatched += (sender, args) => Play(SoundEffect.Match);
        GameManager.Instance.OnGameWin += (sender, args) => Play(SoundEffect.GameWin);
        GameManager.Instance.OnGameLose += (sender, args) => Play(SoundEffect.GameLose);

        SetAmbienceVolume(ambienceMusicVolume);
        SetAmbiencePlay(true);
    }

    public void SetAmbiencePlay(bool play) {
        if (play) {
            gameAmbienceSource.Play();
        }
        else {
            gameAmbienceSource.Stop();
        }
    }

    public void SetAmbienceVolume(float volume) {
        ambienceMusicVolume = Mathf.Clamp01(volume);
        gameAmbienceSource.volume = ambienceMusicVolume;
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