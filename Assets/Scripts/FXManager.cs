using System;
using System.Collections;
using UnityEngine;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class FXManager : MonoBehaviour
{
    public static FXManager instance;

    public Transform playerTransform;
    public Color toonColor = Color.white;
    public float coneCutoutRadius = 2.0f;
    public float coneCutoutOffset = 4.0f;

    private Volume volume;
    private Vignette vignette;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        volume = FindAnyObjectByType<Volume>();
        volume.profile.TryGet(out vignette);
    }

    private void OnValidate()
    {
        UpdateShaderGlobals();
    }

    private void Update()
    {
        UpdateShaderGlobals();
    }

    private void UpdateShaderGlobals()
    {
        if (playerTransform == null) return;
        Shader.SetGlobalVector("_PlayerPos", playerTransform.position);
        Shader.SetGlobalFloat("_ConeCutoutRadius", coneCutoutRadius);
        Shader.SetGlobalFloat("_ConeCutoutOffset", coneCutoutOffset);
    }
    
    public void ShowVignette() => StartCoroutine(FadeVignette());

    IEnumerator FadeVignette()
    {
        float t=0;
        while (t < 0.5f)
        {
            vignette.intensity.value = Mathf.Lerp(0, 0.55f, t / 0.25f);
            t += Time.deltaTime;
            yield return null;
        }

        t=0;
        while (t < 0.5f)
        {
            vignette.intensity.value = Mathf.Lerp(0.55f, 0, t / 0.25f);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
