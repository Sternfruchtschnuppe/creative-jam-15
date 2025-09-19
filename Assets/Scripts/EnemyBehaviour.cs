using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState { Chase, Flee, Freeze }
    public EnemyState state = EnemyState.Chase;
    public float damage = 1f;
    
    public float scaredTime = 0.5f;
    public float attackCooldown = 0.5f;

    
    private Transform playerTransform;
    private NavMeshAgent agent;

    private float lastAttacked = float.NegativeInfinity;
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

    private void OnCollisionEnter(Collision other)
    {
        TryAttackPlayer(other);
    }
    
    private void OnCollisionStay(Collision other)
    {
        TryAttackPlayer(other);
    }
    
    private void TryAttackPlayer(Collision other)
    {
        if (Time.time < lastAttacked + attackCooldown) return;
        
        if (other.gameObject.TryGetComponent<Player>(out var player))
        {
            lastAttacked = Time.time;
            player.life -= damage;
        }
    }

    public void EnterVisionCone()
    {
        lastEnteredVisionCone = Time.time;
        state = EnemyState.Flee;
    }
}