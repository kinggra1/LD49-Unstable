﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnstableManager : MonoBehaviour {

    public static UnstableManager Instance;

    public Image uiMeter;

    [Range(0f, 0.1f)]
    public float percentageRecoveryPerSecond = 0.05f;

    private float normalizedUnstableLevel = 0.5f; // [0, 1]

    private void Awake() {
        if (Instance) {
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

        // Unstable-ness is constnatly decreasing by a small amount.
        normalizedUnstableLevel -= percentageRecoveryPerSecond * Time.deltaTime;
        normalizedUnstableLevel = Mathf.Clamp01(normalizedUnstableLevel);

        uiMeter.fillAmount = normalizedUnstableLevel;
    }

    public void AddInstability(float percentage) {
        normalizedUnstableLevel += percentage;
        normalizedUnstableLevel = Mathf.Clamp01(normalizedUnstableLevel);
    }

    public float UnstableValue() {
        return normalizedUnstableLevel;
    }
}
