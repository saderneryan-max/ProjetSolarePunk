using UnityEngine;

// Rend le vol du drone réaliste :
// - les hélices démarrent progressivement et accélèrent quand le drone se déplace (via l'Animator)
// - le drone s'incline dans le sens de son déplacement, comme un vrai quadricoptère.
// À placer sur DroneVisual (l'objet qui porte l'Animator), enfant de Drone.
public class DroneRotorController : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform droneRoot;            // l'objet Drone (celui que le code déplace)

    [Header("Vitesse des hélices (multiplicateur de l'animation)")]
    [SerializeField] private float idleRotorSpeed = 1f;      // vol stationnaire
    [SerializeField] private float maxRotorSpeed = 2.2f;     // à pleine vitesse
    [SerializeField] private float spinUpDuration = 1.5f;    // temps de démarrage des moteurs
    [SerializeField] private float rotorResponsiveness = 3f; // rapidité des changements de régime

    [Header("Inclinaison")]
    [SerializeField] private float maxMoveSpeed = 5f;        // mettre la même valeur que Move Speed du drone
    [SerializeField] private float maxTiltAngle = 12f;       // inclinaison maximum en degrés
    [SerializeField] private float tiltSmoothing = 5f;

    [Header("Son des moteurs")]
    [SerializeField] private AudioSource motorSound;
    [SerializeField] private Vector2 motorPitchRange = new Vector2(0.8f, 1.4f); // grave au ralenti, aigu à fond
    [SerializeField] private float motorMaxVolume = 0.5f;

    // On convertit le nom du paramètre en nombre une seule fois : plus rapide et sans faute de frappe
    private static readonly int RotorSpeedParam = Animator.StringToHash("RotorSpeed");

    private Vector3 lastPosition;
    private float currentRotorSpeed = 0f; // les moteurs démarrent à l'arrêt
    private float spinUpTimer = 0f;

    private void Start()
    {
        lastPosition = droneRoot.position;
    }

    private void Update()
    {
        // Jeu en pause (timeScale = 0) : on ne calcule rien, sinon on diviserait par 0
        if (Time.deltaTime <= 0f) return;

        Vector3 velocity = GetHorizontalVelocity();
        float speedRatio = Mathf.Clamp01(velocity.magnitude / maxMoveSpeed); // 0 = immobile, 1 = pleine vitesse

        UpdateRotors(speedRatio);
        UpdateTilt(velocity);
    }

    // Vitesse réelle du drone, calculée à partir de son déplacement depuis l'image précédente
    private Vector3 GetHorizontalVelocity()
    {
        Vector3 velocity = (droneRoot.position - lastPosition) / Time.deltaTime;
        lastPosition = droneRoot.position;
        velocity.y = 0f; // on ignore la montée/descente due au relief
        return velocity;
    }

    private void UpdateRotors(float speedRatio)
    {
        // Démarrage des moteurs : le régime maximum autorisé monte de 0 à 1 en spinUpDuration secondes
        spinUpTimer += Time.deltaTime;
        float startupFactor = Mathf.Clamp01(spinUpTimer / spinUpDuration);

        float targetSpeed = Mathf.Lerp(idleRotorSpeed, maxRotorSpeed, speedRatio) * startupFactor;

        // Le régime suit la cible progressivement (un moteur n'accélère pas instantanément)
        currentRotorSpeed = Mathf.Lerp(currentRotorSpeed, targetSpeed, rotorResponsiveness * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat(RotorSpeedParam, currentRotorSpeed);
        }

        // Le son suit le régime des hélices : plus elles tournent vite, plus c'est aigu et fort
        if (motorSound != null)
        {
            float rpm = Mathf.InverseLerp(0f, maxRotorSpeed, currentRotorSpeed); // 0 à 1
            motorSound.pitch = Mathf.Lerp(motorPitchRange.x, motorPitchRange.y, rpm);
            motorSound.volume = rpm * motorMaxVolume;
        }
    }

    private void UpdateTilt(Vector3 velocity)
    {
        // Direction du mouvement vue depuis le drone : z = vers l'avant, x = vers la droite
        Vector3 localVelocity = droneRoot.InverseTransformDirection(velocity) / maxMoveSpeed;

        // Un drone s'incline vers l'avant pour avancer, sur le côté pour glisser latéralement
        float pitch = Mathf.Clamp(localVelocity.z, -1f, 1f) * maxTiltAngle;
        float roll = Mathf.Clamp(-localVelocity.x, -1f, 1f) * maxTiltAngle;

        Quaternion targetTilt = Quaternion.Euler(pitch, 0f, roll);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetTilt, tiltSmoothing * Time.deltaTime);
    }
}