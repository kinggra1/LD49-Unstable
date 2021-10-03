using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour {

    Collider2D areaOfEffect;

    // Start is called before the first frame update
    void Start() {
        areaOfEffect = GetComponentInChildren<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
