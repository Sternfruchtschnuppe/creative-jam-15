using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timeToDestroy = 2f;
    void Start()
    {
        Invoke("DestroyItself", timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DestroyItself()
    {
        Destroy(this.gameObject);
    }
}
