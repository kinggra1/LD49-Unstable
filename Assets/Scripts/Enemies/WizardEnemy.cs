using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemy : MonoBehaviour, EnemyInterface {

    private enum AttackState { TeleportCooldown, Attack, AttackCooldown }
    private AttackState attackState = AttackState.AttackCooldown;

    private static readonly float SPEED = 1.5f;
    private static readonly int MAX_HEALTH = 20;
    private static readonly float INSTABILITY = 0.4f;
    private static readonly float SPAWN_RADIUS = 4f;

    private static readonly float MIN_X_POS = -13.3f;
    private static readonly float MAX_X_POS = 13.3f;
    private static readonly float MIN_Y_POS = -7.3f;
    private static readonly float MAX_Y_POS = 6.6f;

    public GameObject enemyFireballPrefab;

    private int currentHealth = MAX_HEALTH;
    private float stateTimer = 0f;
    private float teleportCooldownTime;
    private float attackCooldownTime;
    private float attackTimer;
    private int attackCount;
    private int totalNumberOfAttacks;
    private float attackSpeed;

    private float movementRotation;

    // Start is called before the first frame update
    void Start() {
        movementRotation = Random.Range(50f, 80f) * Mathf.Sign(Random.Range(-1f, 1f));
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        stateTimer += Time.deltaTime;

        switch (attackState) {
            case AttackState.TeleportCooldown:
                if (stateTimer > teleportCooldownTime) {
                    ChangeState(AttackState.Attack);
                    break;
                }
                break;
            case AttackState.Attack:
                attackTimer += Time.deltaTime;
                if (attackTimer > attackSpeed) {
                    ShootFireball();
                    attackTimer = 0f;
                }
                if (attackCount > totalNumberOfAttacks) {
                    ChangeState(AttackState.AttackCooldown);
                    break;
                }
                break;
            case AttackState.AttackCooldown:
                if (stateTimer > attackCooldownTime) {
                    ChangeState(AttackState.TeleportCooldown);
                    break;
                }
                break;

        }

        // move closer to player character
        float step = SPEED * Time.deltaTime; // calculate distance to move
        var playerPosition = CharacterController.Instance.transform.position;
        Vector3 directionToplayer = playerPosition - this.transform.position;
        Vector3 targetDirection = Quaternion.Euler(0f, 0f, movementRotation) * directionToplayer;
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.transform.position + targetDirection, step);

        SetFacingBasedOnPlayer();
    }

    private void ChangeState(AttackState newState) {
        ExitState(attackState);
        EnterState(newState);
        attackState = newState;
        stateTimer = 0f;
    }

    private void ExitState(AttackState state) {
        switch (state) {
            case AttackState.TeleportCooldown:
                break;
            case AttackState.Attack:
                break;
            case AttackState.AttackCooldown:
                Teleport();
                break;
        }
    }

    private void EnterState(AttackState state) {
        switch (state) {
            case AttackState.TeleportCooldown:
                teleportCooldownTime = Random.Range(0.5f, 2f);
                break;
            case AttackState.Attack:
                attackCount = 0;
                attackSpeed = Random.Range(0.5f, 2f);
                totalNumberOfAttacks = Random.Range(3, 8);
                break;
            case AttackState.AttackCooldown:
                attackCooldownTime = Random.Range(0.5f, 3f);
                break;
        }
    }

    private void ShootFireball() {
        attackCount++;
        GameObject fireball = Instantiate(enemyFireballPrefab);
        Vector3 fireDirection = CharacterController.Instance.transform.position - this.transform.position;
        float aimRotation = Random.Range(-10f, 10f);
        fireDirection = Quaternion.Euler(0, 0, aimRotation) * fireDirection;
        fireball.transform.position = this.transform.position + fireDirection.normalized;

        EnemyWizardFireball fireballScript = fireball.GetComponent<EnemyWizardFireball>();
        fireballScript.SetDirection(fireDirection);
    }

    private void Teleport() {
        float randomAngle = Random.value * Mathf.PI * 2;
        Vector3 newPosition = CharacterController.Instance.transform.position 
            + new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * SPAWN_RADIUS;
        Vector3 clampedNewPosition = new Vector3(Mathf.Clamp(newPosition.x, MIN_X_POS, MAX_X_POS), Mathf.Clamp(newPosition.y, MIN_Y_POS, MAX_Y_POS), 0.0f);

        GameObject disappearPoof = Instantiate(WaveManager.Instance.magicPoofPrefab);
        disappearPoof.transform.position = this.transform.position;

        this.transform.position = newPosition;
        movementRotation = Random.Range(50f, 80f) * Mathf.Sign(Random.Range(-1f, 1f));

        GameObject appearPoof = Instantiate(WaveManager.Instance.magicPoofPrefab);
        appearPoof.transform.position = this.transform.position;
    }

    private void SetFacingBasedOnPlayer() {
        Vector3 playerPosition = CharacterController.Instance.transform.position;

        Vector3 localScale = this.transform.localScale;
        bool leftOfPlayer = playerPosition.x - this.transform.position.x > 0f;
        localScale.x = Mathf.Abs(localScale.x) * (leftOfPlayer ? -1 : 1);

        this.transform.localScale = localScale;
    }

    public void TakeDamage(int damageAmount) {
        currentHealth = currentHealth - damageAmount;
        AudioManager.Instance.PlayEnemyWizardHitSound();
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        Destroy(this.gameObject);
        GameManager.Instance.WinGame();
    }

    private void OnTriggerStay2D(Collider2D collider) {
        // if object collided with is the player, increase their instability
        var player = collider.gameObject.GetComponent<CharacterController>();
        if (player != null) {
            player.TakeDamage(INSTABILITY);
        }
    }
}
