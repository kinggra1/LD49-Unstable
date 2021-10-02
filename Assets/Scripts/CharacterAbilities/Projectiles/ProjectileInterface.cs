using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ProjectileInterface {
    void SetDirection(Vector3 direction);
    void ImpactEnemy();
    void ImpactWall();
}
