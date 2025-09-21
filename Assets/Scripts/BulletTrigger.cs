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
        Invoke("DestroyItself", 2f);
        DisableBullet();
        
    }
    void DestroyItself()
    {
        Destroy(this.gameObject);
    }
    public void DisableBullet()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<Collider>().enabled = false;
    }
}

