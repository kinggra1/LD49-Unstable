using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Wizard/Level", order = 51)]
public class Level : ScriptableObject {

    [System.Serializable]
    public class EnemyGroup {
        public WaveManager.EnemyType type;
        public int count;
    }

    [System.Serializable]
    public class Wave {
        [SerializeField]
        public List<EnemyGroup> spawnedEnemies;
    }


    [SerializeField]
    public Conversation introConversation;
    [SerializeField]
    public Wave[] waves;
}
