using System;
using UnityEngine;

[ExecuteAlways]
public class FlashLightController : MonoBehaviour
{
    public float range = 5f;
    [Range(0, 180)] public float coneRadiusDegrees = 45f;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = transform.parent;
        UpdateCollider();
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
        if (!other.TryGetComponent<EnemyBehaviour>(out var enemy)) return;
        
        var enemyDir = (enemy.transform.position - playerTransform.position).normalized;;
        if (Mathf.Acos(Vector3.Dot(enemyDir, playerTransform.forward)) < coneRadiusDegrees * Mathf.Deg2Rad)
        {
            enemy.EnterVisionCone();
        }
    }

    public void UpdateLife(float life)
    {
        range = life;
        UpdateCollider();
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
