using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public GameObject defeatUI;

    private bool paused = false;
    private bool gameOver = false;

    void Start() {
        defeatUI.SetActive(false);
        WaveManager.Instance.BeginFirstWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause() {
        paused = true;
    }

    public void Play() {
        paused = false;
    }

    public void GameOver() {
        gameOver = true;
        Pause();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }
        defeatUI.SetActive(true);
    }

    public void RestartLevel() {
        gameOver = false;
        defeatUI.SetActive(false);
        UnstableManager.Instance.Reset();
        WaveManager.Instance.RestartLevel();
        CharacterController.Instance.transform.position = Vector3.zero;
    }

    public bool IsPaused() {
        return paused;
    }

    public bool IsGameOver() {
        return gameOver;
    }
}
