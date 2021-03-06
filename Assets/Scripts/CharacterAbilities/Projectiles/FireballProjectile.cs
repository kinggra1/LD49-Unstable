using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour, ProjectileInterface {

    private static readonly float SPEED = 8f;
    private static readonly int DAMAGE = 1;

    public GameObject miniPoofPrefab;

    private Vector3 direction;
    public void ImpactEnemy() {
        throw new System.NotImplementedException();
    }

    public void ImpactWall() {
        throw new System.NotImplementedException();
    }

    public void SetDirection(Vector3 direction) {
        this.direction = direction.normalized;
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 180);
    }

    // Start is called before the first frame update
    void Start(){
        
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

        // if object collided with is an enemy, inflict damage on it
        var impactedEnemy = collider.gameObject.GetComponent<EnemyInterface>();
        if (impactedEnemy != null)
        {
            impactedEnemy.TakeDamage(DAMAGE);
            AudioManager.Instance.PlayFireballHitEnemy();
        }

        GameObject miniPoof = Instantiate(miniPoofPrefab);
        Vector3 hitPosition = this.transform.position + (collider.transform.position - this.transform.position) / 2f;
        miniPoof.transform.position = hitPosition;
        Destroy(this.gameObject);
    }
}
