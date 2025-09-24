using UnityEngine;

public class ReducePlayerLifeByTime : MonoBehaviour
{
    public float lifeReductionPerSecond = 1.0f;
    private PlayerManager player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
    }

    private void Update()
    {
        if (!player.ennemiesNearby) return;

        var life = player.life - lifeReductionPerSecond * Time.deltaTime;
        player.UpdateLife(life);
    }
}
