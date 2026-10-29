using UnityEngine;
using UnityEngine.InputSystem;

// Déplace le drone au-dessus du potager à partir de l'action "Move",
// dans la direction où regarde la caméra. Fonctionne au clavier comme à la manette.
public class DroneMovement : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flightHeight = 3f;
    [SerializeField] private float turnSpeed = 8f;        // vitesse à laquelle le drone s'oriente

    [Header("Références")]
    [SerializeField] private Transform cameraTransform;   // la caméra qui sert de repère

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 direction = GetCameraRelativeDirection(input);

        // Déplacement
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        // Le drone se tourne doucement vers sa direction de déplacement
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Hauteur constante (sera remplacée par le suivi du relief à l'étape suivante)
        Vector3 position = transform.position;
        position.y = flightHeight;
        transform.position = position;
    }

    // Transforme l'input (haut/bas/gauche/droite) en direction dans le monde, selon la caméra
    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (cameraTransform == null)
        {
            return new Vector3(input.x, 0f, input.y); // sans caméra : axes du monde
        }

        // "Devant" et "à droite" de la caméra, mais aplatis au sol (y = 0)
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        return forward * input.y + right * input.x;
    }
}