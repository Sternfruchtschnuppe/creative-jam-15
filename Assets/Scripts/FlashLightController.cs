using System;
using UnityEngine;
using Random = System.Random;

[ExecuteAlways]
public class FlashLightController : MonoBehaviour
{
    public float range = 5f;
    [Range(0, 180)] public float coneRadiusDegrees = 45f;
    
    public float offThreshhold = 1.0f;
    public float flickerThreshold = 2.5f;
    
    public float lightMaxIntensity = 1000f;
    public float lightMinIntensity = 200f;

    public float minRange = 3.0f;
    public float maxRange = 20.0f;
    
    public Light spotLight;
    
    private Transform playerTransform;
    private LayerMask environmentMask;
    
    
    private void Start()
    {
        playerTransform = transform.parent;
        UpdateCollider();
        environmentMask = LayerMask.GetMask("Environment");
    }
    
    private void OnValidate()
    {
        UpdateCollider();
    }
    
    private void UpdateCollider()
    {
        GetComponent<SphereCollider>().radius = range;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerTransform.GetComponent<PlayerManager>().isUsingFlashLight)
            return;
        if (!other.TryGetComponent<EnemyBehaviour>(out var enemy)) return;
        
        var enemyDir = (enemy.transform.position - playerTransform.position).normalized;
        if (Mathf.Acos(Vector3.Dot(enemyDir, playerTransform.forward)) < coneRadiusDegrees * Mathf.Deg2Rad)
        {
            if (!Physics.Raycast(other.transform.position + Vector3.up, 
                    playerTransform.position - other.transform.position + Vector3.up, 
                    out RaycastHit hit, range, environmentMask))
            {
                enemy.EnterVisionCone();
            }
        }
    }

    public void UpdateLife(float life)
    {
        range = Mathf.Clamp(life, minRange, maxRange);
        var range01 = (range - minRange) / (maxRange - minRange);
        UpdateCollider();

        if (life < offThreshhold)
        {
            spotLight.intensity = 0;
        }
        else if (life < flickerThreshold)
        {
            //flicker
            float t = Time.time;
            float v = Mathf.Sin(13.7f * t) 
                      + 0.7f * Mathf.Cos(5.3f * t) 
                      + 0.4f * Mathf.Sin(11.1f * t);
            spotLight.intensity = (v > 0.5f ? 1f : 0.05f) * lightMinIntensity;
        }
        else
        {
            spotLight.intensity = lightMinIntensity +  range01 * (lightMaxIntensity - lightMinIntensity);
        }
        spotLight.range = range * 1.5f;
    }
    
    private void OnDrawGizmos()
    {
        if (playerTransform == null) playerTransform = transform.parent;
        if (playerTransform == null) return;

        Gizmos.color = Color.red;
        Vector3 origin = playerTransform.position;

        int segments = 24;
        float angleStep = 360f / segments;
        Quaternion rotStep = Quaternion.AngleAxis(angleStep, playerTransform.forward);
        Vector3 coneDirection = Quaternion.AngleAxis(-coneRadiusDegrees, playerTransform.up) * playerTransform.forward;

        Vector3 prevPoint = origin + coneDirection * range;
        for (int i = 0; i <= segments; i++)
        {
            coneDirection = rotStep * coneDirection;
            Vector3 nextPoint = origin + coneDirection * range;
            Gizmos.DrawLine(origin, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
