using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour {
    private static readonly float MAX_DISTORTION = 0.05f;
    public Material EffectMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dst) {
        if (EffectMaterial != null) {
            float distortionValue = UnstableManager.Instance.UnstableValue() * MAX_DISTORTION;
            EffectMaterial.SetFloat("_Magnitude", distortionValue);
            Graphics.Blit(src, dst, EffectMaterial);
        }
    }
}
