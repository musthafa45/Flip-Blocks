using UnityEngine;
using UnityEngine.UI;

public class MainMenuUi : MonoBehaviour {
    public const string GAME_SCENE = "02_Game_Scene";

    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button musicMuteButton;
    [SerializeField] private Image musicImage;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;

    private bool isMusicMuted;
    private bool isSettingsOpen;

    private void Awake() {
        // Load saved mute state
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        UpdateMusicVisual();

        playButton.onClick.AddListener(() => {
            MainMenuManager.Instance.LoadScene(GAME_SCENE);
        });

        settingsButton.onClick.AddListener(ToggleSettings);

        musicMuteButton.onClick.AddListener(ToggleMusic);

        musicMuteButton.gameObject.SetActive(false);
    }

    private void ToggleSettings() {
        isSettingsOpen = !isSettingsOpen;
        musicMuteButton.gameObject.SetActive(isSettingsOpen);
    }

    private void ToggleMusic() {
        isMusicMuted = !isMusicMuted;

        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (PersistAmbientMusic.Instance != null) {
            PersistAmbientMusic.Instance.SetAmbiencePlay(!isMusicMuted);
        }

        UpdateMusicVisual();
    }

    private void UpdateMusicVisual() {
        musicImage.sprite = isMusicMuted ? musicOff : musicOn;
    }
}