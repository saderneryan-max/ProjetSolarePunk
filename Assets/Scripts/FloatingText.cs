using TMPro;
using UnityEngine;

// Texte flottant ("+3", "-1") au-dessus d'un légume : il fait face à la caméra, s'estompe, puis se détruit.
// Le mouvement (jaillissement + montée) est fait par l'Animator ; la couleur et le fondu par ce script.
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float lifetime = 1.2f;   // durée de vie totale
    [SerializeField] private float fadeStart = 0.6f;  // le fondu commence à partir de ce moment

    private float age = 0f;
    private Camera mainCamera;

    // Appelée par la plante juste après l'Instantiate
    public void Setup(string text, Color color)
    {
        label.text = text;
        label.color = color;
    }

    private void Start()
    {
        mainCamera = Camera.main; // on la récupère une seule fois, pas à chaque image
        Destroy(gameObject, lifetime);
    }

    // LateUpdate : après le déplacement de la caméra, pour être parfaitement face à elle
    private void LateUpdate()
    {
        // Même orientation que la caméra : le texte est toujours lisible, jamais de travers
        if (mainCamera != null)
        {
            transform.rotation = mainCamera.transform.rotation;
        }

        // Fondu : opaque jusqu'à fadeStart, puis disparaît progressivement jusqu'à lifetime
        age += Time.deltaTime;
        float fade = Mathf.InverseLerp(fadeStart, lifetime, age); // 0 → 1
        label.alpha = 1f - fade;
    }
}