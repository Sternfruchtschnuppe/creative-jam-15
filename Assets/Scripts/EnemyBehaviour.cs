using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [System.Serializable]
    public enum EnemyState { Chase, Slow, Freeze }
    public EnemyState state = EnemyState.Chase;
    public float damage = 1f;

    public float speed = 3.5f;

    public float scaredTime = 0.5f;
    public float attackCooldown = 0.5f;

    public float life = 2f;

    private Transform playerTransform;
    private NavMeshAgent agent;

    private float lastAttacked = float.NegativeInfinity;
    private float lastEnteredVisionCone = float.NegativeInfinity;

    public Animator monsterAnimator;

    public bool isOperational = true;

    public bool isPlayerInTrigger;

    public PlayerManager player;

    public SkinnedMeshRenderer skinnedMeshRenderer;

    public float dissolveAmount = 0f;
    public Material dissolveMat;

    public int monsterID;

    public bool slowFlag = false;
    void Start()
    {
        
        isOperational = true;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;

        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        dissolveMat = skinnedMeshRenderer.material;
    }

    void Update()
    {
        if (isOperational)
        {
            if (playerTransform == null || agent == null) return;


            if (state != EnemyState.Freeze && Time.time > scaredTime + lastEnteredVisionCone)
            {
                if (state == EnemyState.Slow) state = EnemyState.Chase;
            }
            if (slowFlag)
                state = EnemyState.Slow;
            switch (state)
            {
                case EnemyState.Chase:
                    agent.isStopped = false;
                    agent.speed = speed;
                    agent.SetDestination(playerTransform.position);
                    break;

                case EnemyState.Slow:
                    agent.isStopped = false;
                    agent.speed = speed / 10f;
                    agent.SetDestination(playerTransform.position);
                    break;

                case EnemyState.Freeze:
                    agent.isStopped = true;
                    break;
            }
        }
        else
        {
            dissolveAmount += 0.5f * Time.deltaTime;

            dissolveMat.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOperational)
            {
                player = other.GetComponent<PlayerManager>();
                isPlayerInTrigger = true;
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
                player = other.GetComponent<PlayerManager>();
                isPlayerInTrigger = false;
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


    public void EnterVisionCone()
    {
        if (isOperational)
        {
            lastEnteredVisionCone = Time.time;
            state = EnemyState.Slow;
            slowFlag = true;
            if (!IsInvoking("OnEndSlowFlag"))
            {
                Invoke("OnEndSlowFlag", 4f);
            }
        }
    }
    void OnEndSlowFlag()
    {
        slowFlag = false;
    }

    public void UpdateLife(float life)
    {
        this.life = life;

        if (life <= 0f && playerTransform != null)
        {
            var pm = playerTransform.GetComponent<PlayerManager>();
            if (pm != null) pm.UpdateLife(pm.life + 2f);
            state = EnemyState.Freeze;
            agent.isStopped = true;
            isOperational = false;
            monsterAnimator.SetTrigger("Death");
            GameManager.instance.MonsterDead(monsterID);
            this.GetComponent<MonsterDeathSFX>().OnDeath();
            Invoke("DestroyItself", 2f);
         //   Destroy(gameObject);
        }
    }
    public void DestroyItself()
    {
        Destroy(gameObject);
    }
    public void MonsterAttack()
    {
        if (isOperational)
        {
            if (isPlayerInTrigger)
            {
                player.life -= damage;
                player.OnHit();
            }
        }
    }
}
