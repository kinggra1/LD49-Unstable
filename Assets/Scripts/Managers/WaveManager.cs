using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager> {

    private static readonly float SPAWN_RADIUS = 7f;
    public enum EnemyType { Slime, Bat, Fish, Boss }

    public GameObject enemyParentObject;
    public GameObject slimePrefab;
    public GameObject batPrefab;
    public GameObject fishPrefab;
    public GameObject bossPrefab;

    public GameObject magicPoofPrefab;

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
            GameManager.Instance.WinGame();
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
                Vector3 position = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * SPAWN_RADIUS;

                switch (enemies.type) {
                    case EnemyType.Slime:
                        GameObject slime = Instantiate(slimePrefab);
                        slime.transform.parent = enemyParentObject.transform;
                        slime.transform.position = position;
                        break;
                    case EnemyType.Bat:
                        GameObject bat = Instantiate(batPrefab);
                        bat.transform.position = position;
                        bat.transform.parent = enemyParentObject.transform;
                        break;
                    case EnemyType.Fish:
                        GameObject fish = Instantiate(fishPrefab);
                        fish.transform.position = position;
                        fish.transform.parent = enemyParentObject.transform;
                        break;
                    case EnemyType.Boss:
                        GameObject boss = Instantiate(bossPrefab);
                        boss.transform.position = new Vector3(0f, 8f, 0f);
                        boss.transform.parent = enemyParentObject.transform;
                        break;
                }
            }
        }
    }
}
