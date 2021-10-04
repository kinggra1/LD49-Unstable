using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour, EnemyInterface {
    private static readonly float SPEED = 1f;
    private static readonly int MAX_HEALTH = 5;
    private static readonly float INSTABILITY = 0.2f;
    private static readonly float MIN_SHOOT_COOLDOWN = 1.5f;
    private static readonly float MAX_SHOOT_COOLDOWN = 2.5f;

    private static readonly float MIN_X_POS = -13.3f;
    private static readonly float MAX_X_POS = 13.3f;
    private static readonly float MIN_Y_POS = -7.3f;
    private static readonly float MAX_Y_POS = 6.6f;

    private int currentHealth = MAX_HEALTH;
    private float shootCooldown;
    private float movementRotation = 0f;
    private bool hasPoofedIn = false;
    private float poofDelay;
    private float poofTimer = 0f;

    public GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start() {
        // Each bat has a random offset path that it follows 
        // so that it isn't moving *directly* towards the player
        movementRotation = Random.Range(50f, 80f) * Mathf.Sign(Random.Range(-1f, 1f));

        shootCooldown = Random.Range(MIN_SHOOT_COOLDOWN, MAX_SHOOT_COOLDOWN);

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

        // move closer to player character, but keep within screen constraints
        float step = SPEED * Time.deltaTime; // calculate distance to move
        var playerPosition = CharacterController.Instance.transform.position;
        Vector3 directionToplayer = playerPosition - this.transform.position;
        Vector3 targetDirection = Quaternion.Euler(0f, 0f, movementRotation) * directionToplayer;
        Vector3 newPosition = Vector3.MoveTowards(this.transform.position, this.transform.position + targetDirection, step);
        this.transform.position = new Vector3(Mathf.Clamp(newPosition.x, MIN_X_POS, MAX_X_POS), Mathf.Clamp(newPosition.y, MIN_Y_POS, MAX_Y_POS), 0.0f);

        shootCooldown -= Time.deltaTime;
        if (shootCooldown <= 0) {
            shootCooldown = Random.Range(MIN_SHOOT_COOLDOWN, MAX_SHOOT_COOLDOWN);
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
        AudioManager.Instance.PlayBatDeathSound();
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
