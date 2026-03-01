using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    private void Awake() {
        Instance = this;

        CleanUp();
    }

    private void CleanUp() {
        if(GameManager.Instance != null) {
            Destroy(GameManager.Instance.gameObject);
        }

        if(SoundManager.Instance != null) {
            Destroy(SoundManager.Instance.gameObject);
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
