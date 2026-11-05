using UnityEngine;

// Caméra de l'écran titre : tourne lentement autour du potager pour donner vie au menu.
public class TitleCameraOrbit : MonoBehaviour
{
    [SerializeField] private Transform pivot;              // le point que la caméra regarde
    [SerializeField] private float distance = 14f;         // éloignement horizontal
    [SerializeField] private float height = 6f;            // hauteur au-dessus du pivot
    [SerializeField] private float degreesPerSecond = 4f;  // un tour complet en 90 secondes
    [SerializeField] private float startAngle = 0f;

    private float angle;

    private void Start()
    {
        angle = startAngle;
    }

    // LateUpdate : la caméra bouge après tout le reste
    private void LateUpdate()
    {
        angle += degreesPerSecond * Time.deltaTime;

        // Même principe que la caméra du jeu : un point qu'on fait tourner autour du pivot
        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, height, -distance);
        transform.position = pivot.position + offset;
        transform.LookAt(pivot); // regarde toujours le pivot
    }
}