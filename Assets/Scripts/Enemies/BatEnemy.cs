using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour, EnemyInterface {
    private static readonly float SPEED = 1f;
    private static readonly int MAX_HEALTH = 1;
    private static readonly float INSTABILITY = 0.2f;
    private static readonly float SHOOT_TIME = 2f;

    private int currentHealth = MAX_HEALTH;
    private float shootCooldown = 0f;
    private float movementRotation = 0f;

    public GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start() {
        // Each bat has a random offset path that it follows 
        // so that it isn't moving *directly* towards the player
        movementRotation = Random.Range(50f, 80f) * Mathf.Sign(Random.Range(-1f, 1f));
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        // move closer to player character
        float step = SPEED * Time.deltaTime; // calculate distance to move
        var playerPosition = CharacterController.Instance.transform.position;
        Vector3 directionToplayer = playerPosition - this.transform.position;
        Vector3 targetDirection = Quaternion.Euler(0f, 0f, movementRotation) * directionToplayer;
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + targetDirection, step);

        shootCooldown -= Time.deltaTime;
        if (shootCooldown <= 0) {
            shootCooldown = SHOOT_TIME;
            Shoot();
        }
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
    }

    private void OnTriggerStay2D(Collider2D collider) {
        // if object collided with is the player, increase their instability
        var player = collider.gameObject.GetComponent<CharacterController>();
        if (player != null) {
            player.TakeDamage(INSTABILITY);
        }
    }
    private void Shoot() {
        var playerPosition = CharacterController.Instance.transform.position;
        Vector3 projectileDirection = (playerPosition - this.transform.position).normalized;
        
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = this.transform.position + projectileDirection.normalized;
        EnemyProjectileInterface projectileScript = projectile.GetComponent<EnemyProjectileInterface>();
        projectileScript.SetDirection(projectileDirection);
    }
}
