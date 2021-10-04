using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    public AudioClip fireballCastSound;
    public AudioClip slimeDieSound;

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

    public void PlaySlimeDeathSound() {
        enemySfxAudioSource.PlayOneShot(slimeDieSound);
    }
}
