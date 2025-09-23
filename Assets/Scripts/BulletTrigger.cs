using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    public float Dammage = 1;
    public GameObject gfx;
    private AudioSource bulletSource;
    public AudioClip[] audioClips;

	private void Awake()
	{
		bulletSource = GetComponent<AudioSource>();
        bulletSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length - 1)]);
	}

	private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyBehaviour>(out var enemy))
        {
            
            if (enemy.isOperational)
            {
                enemy.UpdateLife(enemy.life - Dammage);
                Destroy(gameObject);
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

