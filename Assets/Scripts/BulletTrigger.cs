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
        if(other.tag == "Enemy")
        {
            other.GetComponent<EnemyBehaviour>().UpdateLife(other.GetComponent<EnemyBehaviour>().life - Dammage);
            Destroy(this.gameObject);
        }
        if(other.tag == "Environment")
        {
            Destroy(this.gameObject);
        }
    }
}
