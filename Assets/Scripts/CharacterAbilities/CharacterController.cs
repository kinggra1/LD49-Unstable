using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : Singleton<CharacterController> {

    private static readonly float MAX_X_MOVE_SPEED = 3f;
    private static readonly float MAX_Y_MOVE_SPEED = 3f;
    private static readonly float MAX_OVERALL_MOVE_SPEED = 3f;
    private static readonly float MAX_IFRAMES = 1f;

    private static readonly float MAX_AIM_VARIANCE = 35f; // Degrees to either side of aimed point.

    [Range(0, 1)]
    public float maxUnstableInfluence = 0.4f;

    public GameObject projectilePrefab;
    public GameObject firestormPrefab;

    private Rigidbody2D rb;

    // Inputs from Update used in FixedUpdate.
    private float xInput;
    private float yInput;
    private bool fireProjectilePressed;
    private bool firestormPressed;

    private float perlinMovementStart = 80000f;
    private float currentWanderAngle = 0f;

    private float currentIFrames = MAX_IFRAMES;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        if (!fireProjectilePressed) {
            fireProjectilePressed = Input.GetMouseButtonDown(0);
        }

        if (!firestormPressed) {
            // firestormPressed = Input.GetMouseButton(1);
        }

        if (currentIFrames > 0) {
            currentIFrames -= Time.deltaTime;
        }

        SetFacingBasedOnMouse();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        if (fireProjectilePressed) {
            FireProjectile();
            fireProjectilePressed = false;
        }

        if (firestormPressed) {
            CreateFirestorm();
            firestormPressed = false;
        }

        Vector3 inputPositionChange = 
            xInput * MAX_X_MOVE_SPEED * Vector3.right 
            + yInput * MAX_Y_MOVE_SPEED * Vector3.up;

        // Oscillate how quickly we move through Perlin noise over time for 2nd order randomness. From 0f to 2f
        float perlinTimeScale = Mathf.Sin(Time.time * 0.5f) * + 1f;
        // Random steering from -0.5f to 0.5f
        float perlinRotationChange = Mathf.PerlinNoise(perlinMovementStart, Time.time * perlinTimeScale) - 0.5f;
        currentWanderAngle += perlinRotationChange * 25f * Time.deltaTime;
        currentWanderAngle %= (Mathf.PI * 2);

        Vector3 perlinPositionChange = new Vector3(Mathf.Cos(currentWanderAngle), Mathf.Sin(currentWanderAngle), 0f) * MAX_OVERALL_MOVE_SPEED;

        float unstableInfluence = UnstableManager.Instance.UnstableValue() * maxUnstableInfluence;

        Vector3 positionChange = inputPositionChange * (1f - unstableInfluence) 
                               + perlinPositionChange * unstableInfluence;

        // Clamp combined input + random Unstable movement
        if (positionChange.magnitude > MAX_OVERALL_MOVE_SPEED) {
            positionChange = positionChange.normalized * MAX_OVERALL_MOVE_SPEED;
        }

        Vector3 newPosition = transform.position + positionChange * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void SetFacingBasedOnMouse() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 localScale = this.transform.localScale;
        bool leftOfPlayer = mouseWorldPos.x - this.transform.position.x < 0f;
        localScale.x = Mathf.Abs(localScale.x) * (leftOfPlayer ? -1 : 1);

        this.transform.localScale = localScale;
    }

    private void FireProjectile() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 projectileDirection = (mouseWorldPos - this.transform.position).normalized;

        // Augment projectile direction based on Unstable offset
        float maxAimRotation = UnstableManager.Instance.UnstableValue() * MAX_AIM_VARIANCE;
        float aimRotation = Random.Range(-maxAimRotation, maxAimRotation);
        projectileDirection = Quaternion.Euler(0, 0, aimRotation) * projectileDirection;

        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = this.transform.position + projectileDirection.normalized;
        ProjectileInterface projectileScript = projectile.GetComponent<ProjectileInterface>();
        projectileScript.SetDirection(projectileDirection);

        AudioManager.Instance.PlayFireballCast();
        UnstableManager.Instance.AddInstability(0.1f);
    }

    private void CreateFirestorm() {
        GameObject firestorm = Instantiate(firestormPrefab);
        firestorm.transform.position = this.transform.position;

        AudioManager.Instance.PlayFirestormCast();
        // UnstableManager.Instance.AddInstability(0.33f);
    }

    public void TakeDamage(float instability) {
        if (currentIFrames <= 0) {
            AudioManager.Instance.PlayPlayerHitSound();
            currentIFrames = 1;
            UnstableManager.Instance.AddInstability(instability);
        }
    }
}
