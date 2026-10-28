using UnityEngine;
using UnityEngine.InputSystem;

// Déplace le drone au-dessus du potager à partir de l'action "Move".
// Fonctionne au clavier comme à la manette, car on lit l'ACTION, pas un périphérique.
public class DroneMovement : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 5f;      // vitesse réglable dans l'Inspector
    [SerializeField] private float flightHeight = 3f;   // hauteur de vol constante

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction; // glisser Gameplay/Move ici

    // Cycle de vie Unity : on active l'action quand le composant s'active...
    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    // ...et on la désactive quand il se désactive.
    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        // Lecture de la direction : (x, y) entre -1 et 1
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // En vue de dessus, le "haut" du stick correspond à l'axe Z du monde
        Vector3 direction = new Vector3(input.x, 0f, input.y);

        // Time.deltaTime rend la vitesse indépendante du nombre d'images par seconde
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        // On force la hauteur pour que le drone ne monte ni ne descende jamais
        Vector3 position = transform.position;
        position.y = flightHeight;
        transform.position = position;
    }
}