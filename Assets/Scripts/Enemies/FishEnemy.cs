using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemy : MonoBehaviour, EnemyInterface {
    private static readonly float SPEED = 0.7f;
    private static readonly int MAX_HEALTH = 10;
    private static readonly float INSTABILITY = 0.4f;

    private int currentHealth = MAX_HEALTH;

    private float poofDelay;
    private float poofTimer = 0f;
    private bool hasPoofedIn = false;

    // Start is called before the first frame update
    void Start() {
        // Start TINY so that we appear invisible until the poof has a chance to play.
        this.transform.localScale = Vector3.zero;
        poofDelay = Random.Range(0.5f, 8f);
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        poofTimer += Time.deltaTime;

        if (!hasPoofedIn) {
            if (poofTimer > poofDelay) {
                GameObject poof = Instantiate(WaveManager.Instance.magicPoofPrefab);
                poof.transform.position = transform.position;
                hasPoofedIn = true;
            }
            return;
        }
        // Hacky quick scale up
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.one, 0.1f);

        // move closer to player character
        float step = SPEED * Time.deltaTime; // calculate distance to move
        var playerPosition = CharacterController.Instance.transform.position;
        this.transform.position = Vector3.MoveTowards(this.transform.position, playerPosition, step);
    }

    public void TakeDamage(int damageAmount) {
        currentHealth = currentHealth - damageAmount;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        Destroy(this.gameObject);
        WaveManager.Instance.OnEnemyDeath();
        AudioManager.Instance.PlayFishDeathSound();
    }

    private void OnTriggerStay2D(Collider2D collider) {
        // if object collided with is the player, increase their instability
        var player = collider.gameObject.GetComponent<CharacterController>();
        if (player != null) {
            player.TakeDamage(INSTABILITY);
        }
    }
}
