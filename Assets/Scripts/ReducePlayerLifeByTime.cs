using UnityEngine;

public class ReducePlayerLifeByTime : MonoBehaviour
{
    public float lifeReductionPerSecond = 1.0f;
    private Player player;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        if (GameManager.instance.paused) return;

        var life = player.life - lifeReductionPerSecond * Time.deltaTime;
        player.UpdateLife(life);
    }
}
