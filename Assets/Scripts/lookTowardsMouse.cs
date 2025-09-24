using UnityEngine;

public class lookTowardsMouse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;

		Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

		// Calculate rotation in world space
		Quaternion lookRot = Quaternion.LookRotation(worldMousePos - transform.position);

		// Apply world rotation (ignoring parent's rotation)
		transform.rotation = lookRot;
	}
}
