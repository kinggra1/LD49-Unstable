using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour, ProjectileInterface {

    private static readonly float SPEED = 8f;

    private Vector3 direction;
    public void ImpactEnemy() {
        throw new System.NotImplementedException();
    }

    public void ImpactWall() {
        throw new System.NotImplementedException();
    }

    public void SetDirection(Vector3 direction) {
        this.direction = direction.normalized;
    }

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update() {
        this.transform.position += direction * SPEED * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(this.gameObject);
    }
}
