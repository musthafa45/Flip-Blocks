using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistAmbientMusic : MonoBehaviour
{
    public static PersistAmbientMusic Instance { get; private set; }

    [SerializeField] private AudioSource gameAmbienceSource;
    [SerializeField] private float ambienceMusicVolume = 0.5f;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
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
}
