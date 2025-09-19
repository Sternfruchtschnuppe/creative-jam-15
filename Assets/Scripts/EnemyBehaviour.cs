using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState { Chase, Flee, Freeze }
    public EnemyState state = EnemyState.Chase;
    public float scaredTime = 0.5f;
        
    private Transform playerTransform;
    private NavMeshAgent agent;

    private float lastEnteredVisionCone = float.NegativeInfinity;
    
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (Time.time > scaredTime + lastEnteredVisionCone)
        {
            state = EnemyState.Chase;
        }
        
        if (state == EnemyState.Chase)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
        }
        else if (state == EnemyState.Flee)
        {
            agent.isStopped = false;
            Vector3 dirToPlayer = transform.position - playerTransform.position;
            Vector3 newPos = transform.position + dirToPlayer;
            agent.SetDestination(newPos);
        }
        else if (state == EnemyState.Freeze)
        {
            agent.isStopped = true;
        }
    }

    public void EnterVisionCone()
    {
        lastEnteredVisionCone = Time.time;
        state = EnemyState.Flee;
    }
}