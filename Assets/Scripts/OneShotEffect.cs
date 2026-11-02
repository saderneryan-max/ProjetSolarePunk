using UnityEngine;

// Effet ponctuel (son, et plus tard particules) : se joue à sa création, puis se détruit tout seul.
// Permet d'entendre un son même quand l'objet qui l'a déclenché (le légume) vient d'être détruit.
public class OneShotEffect : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float lifetime = 2f;                            // durée avant destruction
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);   // légère variation de hauteur

    private void Start()
    {
        if (audioSource != null)
        {
            // Une hauteur un peu différente à chaque fois : le son ne paraît jamais répétitif
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.Play();
        }

        // Destroy avec un délai : l'objet disparaîtra dans "lifetime" secondes
        Destroy(gameObject, lifetime);
    }
}