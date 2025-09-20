using System;
using UnityEngine;

[ExecuteAlways]
public class FXManager : MonoBehaviour
{
    public static FXManager instance;

    public Transform playerTransform;
    public Color toonColor = Color.white;
    public float coneCutoutRadius = 0.1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
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
    }
}
