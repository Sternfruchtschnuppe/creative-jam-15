using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // public float speed = 5f;
    private Transform player;
    public float smoothTime = 0.02f;
    Vector3 velocity;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.position, ref velocity, smoothTime);
    }
}