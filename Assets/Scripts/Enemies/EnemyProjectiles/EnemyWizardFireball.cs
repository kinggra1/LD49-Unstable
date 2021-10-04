using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizardFireball : MonoBehaviour, EnemyProjectileInterface {
    private static readonly float SPEED = 5f;
    private static readonly float INSTABILITY = 0.2f;

    private Vector3 direction;

    public void SetDirection(Vector3 direction) {
        this.direction = direction.normalized;
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 180);
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsGameOver()) {
            Destroy(gameObject);
        }
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        this.transform.position += direction * SPEED * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        if (collider.GetComponent<ProjectileInterface>() != null
            || collider.GetComponent<EnemyProjectileInterface>() != null) {
            return;
        }

        // if object collided with is player, inflict damage
        var player = collider.gameObject.GetComponent<CharacterController>();
        if (player != null) {
            player.TakeDamage(INSTABILITY);
        }
        Destroy(this.gameObject);
    }
}
