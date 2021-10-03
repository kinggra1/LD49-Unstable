using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UnstableManager : Singleton<UnstableManager> {

    private static readonly float MUSIC_FADE_IN_TIME = 1.5f;
    private static readonly float MUSIC_FADE_OUT_TIME = 1.3f;

    // EEEE SSSSS LLLL UUUUUUUUU

    private static readonly float EXTRA_STABLE_THRESHOLD = 0.2f;
    private static readonly float STABLE_THRESHOLD = 0.45f;
    private static readonly float LESS_STABLE_THRESHOLD = 0.66f;


    // Minimum time that must be spent in a given state before transitioning to avoid whiplash
    private static readonly float MINIMUM_STATE_TIME = 2f;
    public enum StabilityLevel { ExtraStable, Stable, LessStable, Unstable }
    private static StabilityLevel stabilityLevel = StabilityLevel.ExtraStable;

    public AudioMixer musicMixer;
    public Image uiMeter;

    [Range(0f, 0.1f)]
    public float percentageRecoveryPerSecond = 0.1f;

    private float normalizedUnstableLevel = 0f; // [0, 1]
    private float timeInCurrentState = 0f;

    // Start is called before the first frame update
    void Start() {
        EnterStabilityLevel(stabilityLevel);
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.IsPaused()) {
            return;
        }

        timeInCurrentState += Time.deltaTime;

        // Unstable-ness is constnatly decreasing by a small amount.
        normalizedUnstableLevel -= percentageRecoveryPerSecond * Time.deltaTime;
        normalizedUnstableLevel = Mathf.Clamp01(normalizedUnstableLevel);

        uiMeter.fillAmount = normalizedUnstableLevel;

        CheckUpdateStabilityLevel();
    }

    private void CheckUpdateStabilityLevel() {
        if (timeInCurrentState < MINIMUM_STATE_TIME) {
            return;
        }

        StabilityLevel newStability;
        if (normalizedUnstableLevel < EXTRA_STABLE_THRESHOLD) {
            newStability = StabilityLevel.ExtraStable;
        }
        else if (normalizedUnstableLevel < STABLE_THRESHOLD) {
            newStability = StabilityLevel.Stable;
        }
        else if (normalizedUnstableLevel < LESS_STABLE_THRESHOLD) {
            newStability = StabilityLevel.LessStable;
        }
        else {
            newStability = StabilityLevel.Unstable;
        }

        if (newStability != stabilityLevel) {
            ExitStabilityLevel(stabilityLevel);
            EnterStabilityLevel(newStability);
            stabilityLevel = newStability;
            timeInCurrentState = 0f;
        }
    }

    private void ExitStabilityLevel(StabilityLevel stability) {
        switch (stability) {
            case StabilityLevel.ExtraStable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "ExtraStableVolume", MUSIC_FADE_OUT_TIME, 0f));
                break;
            case StabilityLevel.Stable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "StableVolume", MUSIC_FADE_OUT_TIME, 0f));
                break;
            case StabilityLevel.LessStable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "LessStableVolume", MUSIC_FADE_OUT_TIME, 0f));
                break;
            case StabilityLevel.Unstable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "UnstableVolume", MUSIC_FADE_OUT_TIME, 0f));
                break;
        }
    }

    private void EnterStabilityLevel(StabilityLevel stability) {
        switch (stability) {
            case StabilityLevel.ExtraStable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "ExtraStableVolume", MUSIC_FADE_IN_TIME, 1f));
                break;
            case StabilityLevel.Stable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "StableVolume", MUSIC_FADE_IN_TIME, 1f));
                break;
            case StabilityLevel.LessStable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "LessStableVolume", MUSIC_FADE_IN_TIME, 1f));
                break;
            case StabilityLevel.Unstable:
                StartCoroutine(FadeMixerGroup.StartFade(musicMixer, "UnstableVolume", MUSIC_FADE_IN_TIME, 1f));
                break;
        }
    }

    public void AddInstability(float percentage) {
        normalizedUnstableLevel += percentage;
        normalizedUnstableLevel = Mathf.Clamp01(normalizedUnstableLevel);
    }

    public float UnstableValue() {
        return normalizedUnstableLevel;
    }
}
