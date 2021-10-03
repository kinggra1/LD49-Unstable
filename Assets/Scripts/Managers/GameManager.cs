using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public Conversation introConversation;

    void Start() {
        DialogueSystem.Instance.StartConversation(introConversation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
