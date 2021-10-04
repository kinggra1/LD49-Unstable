﻿using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

    public GameObject defeatUI;
    public GameObject victoryUI;

    private bool paused = false;
    private bool gameOver = false;

    void Start() {
        defeatUI.SetActive(false);
        victoryUI.SetActive(false);
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
     
    public void RestartGame() {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void WinGame() {
        gameOver = true;
        Pause();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }
        victoryUI.SetActive(true);
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
