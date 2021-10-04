﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatProjectile : MonoBehaviour, EnemyProjectileInterface {
    private static readonly float SPEED = 5f;
    private static readonly float INSTABILITY = 0.2f;

    private Vector3 direction;

    public void SetDirection(Vector3 direction) {
        this.direction = direction.normalized;
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
        // if object collided with is player, inflict damage
        var player = collider.gameObject.GetComponent<CharacterController>();
        if (player != null) {
            player.TakeDamage(INSTABILITY);
        }
        Destroy(this.gameObject);
    }
}
