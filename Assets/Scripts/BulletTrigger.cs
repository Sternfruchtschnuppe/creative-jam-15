using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    public float Dammage = 0.5f;
    public GameObject gfx;
    
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
        Invoke("DestroyItself", 2f);
        DisableBullet();
        
    }
    void DestroyItself()
    {
        Destroy(gameObject, 0.1f);
    }
    public void DisableBullet()
    {
        gfx.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }
}

