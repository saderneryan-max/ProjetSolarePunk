using UnityEngine;
using UnityEngine.InputSystem;

// Déplace le drone dans la direction où regarde la caméra,
// en restant toujours à la même hauteur au-dessus du relief, sans jamais sortir du terrain.
public class DroneMovement : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 8f;

    [Header("Altitude")]
    [SerializeField] private float flightHeight = 3f;      // hauteur au-dessus du sol
    [SerializeField] private float heightSmoothing = 10f;  // rapidité d'adaptation au relief

    [Header("Références")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GroundSensor groundSensor;

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
        Vector3 movement = direction * moveSpeed * Time.deltaTime;

        // On ne bouge que s'il y a du sol à l'endroit où on va : le drone ne sort jamais du terrain
        if (groundSensor.TryGetGroundPoint(transform.position + movement, out Vector3 groundPoint))
        {
            transform.Translate(movement, Space.World);
        }

        FollowGround();
        RotateTowards(direction);
    }

    // Garde le drone à flightHeight au-dessus du sol, en douceur
    private void FollowGround()
    {
        if (groundSensor.TryGetGroundPoint(transform.position, out Vector3 groundPoint))
        {
            float targetHeight = groundPoint.y + flightHeight;

            Vector3 position = transform.position;
            // Lerp = on se rapproche progressivement de la hauteur cible (pas de saut brutal)
            position.y = Mathf.Lerp(position.y, targetHeight, heightSmoothing * Time.deltaTime);
            transform.position = position;
        }
    }

    // Oriente doucement le drone vers sa direction de déplacement
    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    // Transforme l'input en direction dans le monde, selon l'orientation de la caméra
    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (cameraTransform == null)
        {
            return new Vector3(input.x, 0f, input.y);
        }

        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0f;
        right.Normalize();

        return forward * input.y + right * input.x;
    }
}