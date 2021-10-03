using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public Conversation introConversation;

    private bool paused = false;

    void Start() {
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
    public bool IsPaused() {
        return paused;
    }
}
