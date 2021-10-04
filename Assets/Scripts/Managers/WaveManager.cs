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

public class WaveManager : Singleton<WaveManager> {

    private static readonly float SPAWN_RADIUS = 7f;
    public enum EnemyType { Slime, Bat, Fish }

    public GameObject enemyParentObject;
    public GameObject slimePrefab;
    public GameObject batPrefab;

    public List<Level> levels;

    private int levelIndex = -1;
    private Level currentLevel;
    private int waveIndex = -1;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
            TriggerNextWave();
        }
    }

    public void BeginFirstWave() {
        levelIndex = -1;
        TriggerNextLevel();
    }

    public void OnEnemyDeath() {
        // Something about Destroy() timing probably? Need to check that this was the last enemy.
        //if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
        //    TriggerNextWave();
        //}
    }

    public void RestartLevel() {
        levelIndex--;
        TriggerNextLevel();
    }

    private void TriggerNextWave() {
        waveIndex++;
        if (waveIndex >= currentLevel.waves.Length) {
            TriggerNextLevel();
        } else {
            SpawnWave(currentLevel.waves[waveIndex]);
        }
    }

    private void TriggerNextLevel() {
        levelIndex++;
        waveIndex = -1;
        if (levelIndex >= levels.Count) {
            // GG? Start boss fight? No more waves.
            return;
        }

        currentLevel = levels[levelIndex];
        DialogueSystem.Instance.StartConversation(currentLevel.introConversation);

        // We should probably trigger this from end of conversation to avoid enemies appearing early
        TriggerNextWave();
    }

    private void SpawnWave(Level.Wave wave) {
        foreach (Level.EnemyGroup enemies in wave.spawnedEnemies) {
            for (int i = 0; i < enemies.count; i++) {
                float randomAngle = Random.value * Mathf.PI * 2;
                switch (enemies.type) {
                    case EnemyType.Slime:
                        GameObject slime = Instantiate(slimePrefab);
                        slime.transform.position = 
                            new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * SPAWN_RADIUS;
                        break;
                    case EnemyType.Bat:
                        GameObject bat = Instantiate(batPrefab);
                        bat.transform.position = 
                            new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * SPAWN_RADIUS;
                        break;
                    case EnemyType.Fish:
                        break;
                }
            }
        }
    }
}
