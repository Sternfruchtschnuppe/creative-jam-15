using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] AudioSource source;        // sur le personnage (ou sur le pied)
    [SerializeField] AudioClip[] footstepClips; // banque de sons
    [SerializeField, Range(0f, 1f)] float volume = 0.6f;
    [SerializeField] Vector2 pitchJitter = new Vector2(0.95f, 1.05f);

    // Appelée depuis l'événement d’animation
    public void Footstep()
    {
        if (footstepClips.Length == 0 || source == null) return;
        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        source.pitch = Random.Range(pitchJitter.x, pitchJitter.y);
        source.PlayOneShot(clip, volume);

    }
}
