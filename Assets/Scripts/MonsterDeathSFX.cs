using UnityEngine;

public class MonsterDeathSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] deathClips;

    void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Appelle cette fonction quand le perso meurt
    public void OnDeath()
    {
        if (audioSource == null || deathClips == null || deathClips.Length == 0) return;

        var clip = deathClips[Random.Range(0, deathClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
