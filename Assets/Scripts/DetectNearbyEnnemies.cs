using UnityEngine;

public class DetectNearbyEnnemies : MonoBehaviour
{

	private PlayerManager player;
	public float DetectRange;
	public LayerMask ennemiesLayer;

	private void Start()
	{
		player = GetComponent<PlayerManager>();
	}
	private void Update()
	{

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectRange, ennemiesLayer);
		if (hitColliders.Length > 0)
		{
			player.ennemiesNearby = true;
		}
		else
		{
			player.ennemiesNearby= false;
		}
	}
}
