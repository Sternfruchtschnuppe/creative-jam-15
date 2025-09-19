using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemies;


    [Header("Spawn Settings")]
    public float minInterval = 2;
    public float maxInterval = 5;
    public float minDist = 10;
    public float maxDist = 25;
    public int attempts = 10;
    [Header("NavMesh Settings")]
    public float navRadius = 4;

    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            Spawn();
        }
    }

    void Spawn()
    {
        if (FindPos(out var pos))
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)], pos, Quaternion.identity);
        }
    }

    bool FindPos(out Vector3 pos)
    {
        for (int i = 0; i < attempts; i++)
        {
            var dir = Random.insideUnitSphere.normalized * Random.Range(minDist, maxDist);
            if (NavMesh.SamplePosition(player.position + new Vector3(dir.x, 0, dir.z), out var hit, navRadius, NavMesh.AllAreas))
            {
                var path = new NavMeshPath();
                if (NavMesh.CalculatePath(hit.position, player.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    pos = hit.position;
                    return true;
                }
            }
        }
        pos = Vector3.zero;
        return false;
    }
}

