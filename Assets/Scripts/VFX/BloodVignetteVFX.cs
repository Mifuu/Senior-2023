using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodVignetteVFX : MonoBehaviour
{
    public static BloodVignetteVFX instance;

    UnityEngine.Rendering.VolumeProfile volumeProfile;

    [Header("Settings")]
    public float vignetteSmoothnessActivated = 0.9f;
    public float vignetteSmoothnessActivatedTime = 0.6f;
    public Color vignetteColorActivated = Color.red;
    public float vignetteColorActivatedTime = 0.6f;
    public float vignetteIntensityActivated = 0.6f;
    public float vignetteIntensityActivatedTime = 0.6f;

    Vignette vignette;
    bool hasVignette = false;
    float vignetteSmoothnessInit = 0;
    float vignetteSmoothnessTweenTime = 0;
    Color vignetteColorInit = Color.white;
    Color vignetteColorTweenTime = Color.white;
    float vignetteIntensityInit = 0;
    float vignetteIntensityTweenTime = 0;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;

        volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;

        if (volumeProfile.TryGet(out Vignette vignette))
        {
            this.vignette = vignette;
            hasVignette = true;

            vignetteSmoothnessInit = vignette.smoothness.value;
            vignetteColorInit = vignette.color.value;
            vignetteIntensityInit = vignette.intensity.value;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SimpleBloodVignette();
        }
    }

    public static void SimpleBloodVignette()
    {
        if (instance != null)
            instance.SimpleBloodVignetteInstance();
    }

    void SimpleBloodVignetteInstance()
    {
        Debug.Log("SimpleBloodVignetteInstance");
        if (!hasVignette)
            return;

        LeanTween.cancel(gameObject);

        vignette.smoothness.Override(vignetteSmoothnessInit);
        vignette.color.Override(vignetteColorInit);
        vignette.intensity.Override(vignetteIntensityInit);

        LeanTween.value(gameObject, vignetteSmoothnessInit, vignetteSmoothnessActivated, vignetteSmoothnessActivatedTime)
            .setEasePunch()
            .setOnUpdate((float value) =>
            {
                vignette.smoothness.Override(value);
            });

        LeanTween.color(gameObject, vignetteColorActivated, vignetteColorActivatedTime)
            .setEasePunch()
            .setOnUpdate((Color value) =>
            {
                vignette.color.Override(value);
            });

        LeanTween.value(gameObject, vignetteIntensityInit, vignetteIntensityActivated, vignetteIntensityActivatedTime)
            .setEasePunch()
            .setOnUpdate((float value) =>
            {
                vignette.intensity.Override(value);
            });
    }
}
