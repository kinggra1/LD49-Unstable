using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour {

    private static readonly float FINAL_RADIUS = 8f;
    private static readonly float GROWTH_RATE = 8f;
    private static readonly float DESTRUCTION_CHECK_INTERVAL = 0.24f;

    Collider2D areaOfEffect;
    Collider2D[] affectedColliders;
    ContactFilter2D contactFilter = new ContactFilter2D();

    private float destructionTimer = 0f;

    // Start is called before the first frame update
    void Start() {
        areaOfEffect = GetComponentInChildren<Collider2D>();
        this.transform.localScale = Vector3.zero;

        affectedColliders = new Collider2D[100];
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsGameOver()) {
            Destroy(gameObject);
        }
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        destructionTimer += Time.deltaTime;
        this.transform.localScale += Vector3.one * GROWTH_RATE * Time.deltaTime;

        if (this.transform.localScale.x > FINAL_RADIUS) {
            Destroy(this.gameObject);
            return;
        }

        if (destructionTimer > DESTRUCTION_CHECK_INTERVAL) {
            destructionTimer = 0f;

            int colliderCount = Physics2D.OverlapCircleNonAlloc(this.transform.position, this.transform.localScale.x / 2f, affectedColliders);
            for (int i = 0; i < colliderCount; i++) {
                Collider2D collider = affectedColliders[i];
                EnemyInterface enemy = collider.GetComponentInParent<EnemyInterface>();
                if (enemy != null) {
                    enemy.TakeDamage(1);
                }
            }
        }
    }
}
