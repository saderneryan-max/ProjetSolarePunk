using UnityEngine;
using UnityEngine.InputSystem;

// Caméra qui suit le drone et tourne autour de lui (souris / stick droit),
// en restant toujours au-dessus, en vue plongeante.
public class CameraOrbit : MonoBehaviour
{
    [Header("Cible")]
    [SerializeField] private Transform target;          // le drone
    [SerializeField] private float distance = 12f;      // distance caméra <-> drone

    [Header("Angles (en degrés)")]
    [SerializeField] private float startYaw = 0f;       // rotation horizontale de départ
    [SerializeField] private float startPitch = 55f;    // inclinaison de départ
    [SerializeField] private float minPitch = 35f;      // jamais plus à plat que ça
    [SerializeField] private float maxPitch = 80f;      // jamais plus vertical que ça

    [Header("Sensibilité")]
    [SerializeField] private float mouseSensitivity = 0.15f; // degrés par pixel de souris
    [SerializeField] private float stickSensitivity = 120f;  // degrés par seconde au stick
    [SerializeField] private bool invertY = false;

    [Header("Input")]
    [SerializeField] private InputActionReference lookAction; // Gameplay/Look

    private float yaw;
    private float pitch;

    private void Awake()
    {
        yaw = startYaw;
        pitch = startPitch;
    }

    private void OnEnable()
    {
        lookAction.action.Enable();

        // On cache et bloque le curseur au centre pour pouvoir tourner librement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        lookAction.action.Disable();

        // On rend le curseur (pause, changement de scène...)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // LateUpdate s'exécute APRÈS tous les Update :
    // le drone a déjà bougé, la caméra le suit donc sans retard ni tremblement.
    private void LateUpdate()
    {
        if (target == null) return;

        ReadLookInput();

        // On transforme les deux angles en une rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Un point à "distance" mètres derrière, qu'on fait tourner autour du drone
        transform.position = target.position + rotation * new Vector3(0f, 0f, -distance);

        // La caméra regarde dans la même direction : droit vers le drone
        transform.rotation = rotation;
    }

    private void ReadLookInput()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>();

        // La souris donne un DÉPLACEMENT (en pixels) déjà mesuré sur l'image : pas de deltaTime.
        // Le stick donne une POSITION (-1 à 1) : il faut la multiplier par deltaTime.
        bool fromMouse = lookAction.action.activeControl != null
                         && lookAction.action.activeControl.device is Mouse;

        float speed = fromMouse ? mouseSensitivity : stickSensitivity * Time.deltaTime;

        yaw += look.x * speed;

        float vertical = invertY ? -look.y : look.y;
        pitch -= vertical * speed;

        // On bloque l'inclinaison : la caméra reste toujours en plongée
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}