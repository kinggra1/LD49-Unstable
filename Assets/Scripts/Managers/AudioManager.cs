﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    public AudioClip fireballCastSound;
    public AudioClip playerHitSound;
    public AudioClip slimeDieSound;
    public AudioClip batDieSound;
    public AudioClip fishDieSound;
    public AudioClip enemyWizardHitSound;

    private AudioSource playerSfxAudioSource;
    private AudioSource enemySfxAudioSource;

    private void Awake() {
        playerSfxAudioSource = gameObject.AddComponent<AudioSource>();
        enemySfxAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayFireballCast() {
        playerSfxAudioSource.volume = (Random.Range(0.6f, 1f));
        playerSfxAudioSource.pitch = (Random.Range(0.6f, 1.1f));
        playerSfxAudioSource.PlayOneShot(fireballCastSound);
    }

    public void PlayPlayerHitSound() {
        playerSfxAudioSource.volume = .6f;
        playerSfxAudioSource.pitch = (Random.Range(0.9f, 1.1f));
        playerSfxAudioSource.PlayOneShot(playerHitSound);
    }

    public void PlaySlimeDeathSound() {
        enemySfxAudioSource.volume = 0.4f; // Duck audio a bit for loud clip (hacky).
        enemySfxAudioSource.pitch = (Random.Range(0.9f, 1.1f));
        enemySfxAudioSource.PlayOneShot(slimeDieSound);
    }

    public void PlayBatDeathSound() {
        enemySfxAudioSource.volume = .4f;
        enemySfxAudioSource.pitch = (Random.Range(0.9f, 1.1f));
        enemySfxAudioSource.PlayOneShot(batDieSound);
    }

    public void PlayFishDeathSound() {
        enemySfxAudioSource.volume = 0.6f;
        enemySfxAudioSource.pitch = (Random.Range(0.9f, 1.1f));
        enemySfxAudioSource.PlayOneShot(fishDieSound);
    }

    public void PlayEnemyWizardHitSound() {
        enemySfxAudioSource.volume = 0.6f;
        enemySfxAudioSource.pitch = (Random.Range(0.9f, 1.1f));
        enemySfxAudioSource.PlayOneShot(enemyWizardHitSound);
    }
}
