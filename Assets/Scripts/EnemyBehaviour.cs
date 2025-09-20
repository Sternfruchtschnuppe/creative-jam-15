using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [System.Serializable]
    public enum EnemyState { Chase, Flee, Freeze }
    public EnemyState state = EnemyState.Chase;
    public float damage = 1f;

    public float scaredTime = 0.5f;
    public float attackCooldown = 0.5f;

    public float life = 2f;

    private Transform playerTransform;
    private NavMeshAgent agent;

    private float lastAttacked = float.NegativeInfinity;
    private float lastEnteredVisionCone = float.NegativeInfinity;

    public Animator monsterAnimator;

    public bool isOperational = true;

    void Start()
    {
        isOperational = true;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (playerTransform == null || agent == null) return;

        
        if (state != EnemyState.Freeze && Time.time > scaredTime + lastEnteredVisionCone)
        {
            if (state == EnemyState.Flee) state = EnemyState.Chase;
        }

        switch (state)
        {
            case EnemyState.Chase:
                agent.isStopped = false;                           
                agent.SetDestination(playerTransform.position);
                break;

            case EnemyState.Flee:
                agent.isStopped = false;                           
                Vector3 dirToPlayer = transform.position - playerTransform.position;
                Vector3 newPos = transform.position + dirToPlayer;
                agent.SetDestination(newPos);
                break;

            case EnemyState.Freeze:
                agent.isStopped = true;                            
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOperational)
            {
                TryAttackPlayer(other);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOperational)
            {
                state = EnemyState.Freeze;
                if (agent != null) agent.isStopped = true;

                if (monsterAnimator != null)
                {
                    monsterAnimator.SetBool("Attack", true);
                    monsterAnimator.SetBool("Move", false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOperational)
            {
                state = EnemyState.Chase;
                if (agent != null)
                {
                    agent.isStopped = false;
                    agent.SetDestination(playerTransform.position);
                }

                if (monsterAnimator != null)
                {
                    monsterAnimator.SetBool("Attack", false);
                    monsterAnimator.SetBool("Move", true);
                }
            }
        }
    }

    private void TryAttackPlayer(Collider other)
    {
        if (Time.time < lastAttacked + attackCooldown) return;

        if (other.gameObject.TryGetComponent<PlayerManager>(out var player))
        {
            lastAttacked = Time.time;
            player.life -= damage;
            player.OnHit();
        }
    }

    public void EnterVisionCone()
    {
        if (isOperational)
        {
            lastEnteredVisionCone = Time.time;
            state = EnemyState.Flee;
        }
    }

    public void UpdateLife(float life)
    {
        this.life = life;

        if (life <= 0f && playerTransform != null)
        {
            var pm = playerTransform.GetComponent<PlayerManager>();
            if (pm != null) pm.UpdateLife(pm.life + 2f);
            state = EnemyState.Freeze;
            isOperational = false;
            monsterAnimator.SetTrigger("Death");
            Invoke("DestroyItself", 2f);
         //   Destroy(gameObject);
        }
    }
    public void DestroyItself()
    {
        Destroy(gameObject);
    }
}
