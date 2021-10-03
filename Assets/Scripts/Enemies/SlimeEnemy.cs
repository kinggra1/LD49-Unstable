using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour, EnemyInterface {

    private static readonly float SPEED = 0.7f;
    private static readonly int MAX_HEALTH = 1;

    private static int currentHealth = MAX_HEALTH;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // move closer to player character
        float step = SPEED * Time.deltaTime; // calculate distance to move
        var playerPosition = CharacterController.Instance.transform.position;
        this.transform.position = Vector3.MoveTowards(this.transform.position, playerPosition, step);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth = currentHealth - damageAmount;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }    
}
