using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    private static readonly float MAX_X_MOVE_SPEED = 6f;
    private static readonly float MAX_Y_MOVE_SPEED = 6f;

    [Range(0, 1)]
    public float maxUnstableInfluence = 0.5f;

    public GameObject projectilePrefab;

    private Rigidbody2D rb;

    // Inputs from Update used in FixedUpdate.
    private float xInput;
    private float yInput;
    private bool fireProjectilePressed;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        if (!fireProjectilePressed) {
            fireProjectilePressed = Input.GetMouseButtonDown(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate() {

        Debug.Log(fireProjectilePressed);
        if (fireProjectilePressed) {
            FireProjectile();
            fireProjectilePressed = false;
        }

        Vector3 positionChange = 
            xInput * MAX_X_MOVE_SPEED * Vector3.right 
            + yInput * MAX_Y_MOVE_SPEED * Vector3.up;

        Vector3 newPosition = transform.position + positionChange * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void FireProjectile() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector3 projectileDirection = (mouseWorldPos - this.transform.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = this.transform.position + projectileDirection.normalized;
        ProjectileInterface projectileScript = projectile.GetComponent<ProjectileInterface>();
        projectileScript.SetDirection(projectileDirection);

        UnstableManager.Instance.AddInstability(0.1f);
    }
}
