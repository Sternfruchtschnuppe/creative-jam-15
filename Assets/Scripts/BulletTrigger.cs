using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    public float Dammage = 0.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyBehaviour>(out var enemy))
        {
            if (enemy.isOperational)
            {
                enemy.UpdateLife(enemy.life - Dammage);
            }
            else
            {
                return;
            }
            
        }
        Destroy(this.gameObject);
    }
}
