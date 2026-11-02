using UnityEngine;
using UnityEngine.InputSystem;

// Gère l'arrosage : tant que le bouton est maintenu, le drone fait couler de l'eau
// et arrose toutes les plantations situées dans un cercle sous lui.
public class DroneWaterer : MonoBehaviour
{
    [Header("Arrosage")]
    [SerializeField] private float wateringRadius = 1f;   // taille de la zone arrosée
    [SerializeField] private ParticleSystem waterSpray;   // le jet d'eau sous le drone
    [SerializeField] private GroundSensor groundSensor;
    [SerializeField] private WaterTank waterTank;

    [Header("Input")]
    [SerializeField] private InputActionReference waterAction; // Gameplay/Water

    private void OnEnable()
    {
        waterAction.action.Enable();
    }

    private void OnDisable()
    {
        waterAction.action.Disable();
    }

    private void Update()
    {
        // On arrose seulement si le bouton est maintenu ET qu'il reste de l'eau
        bool wantsToWater = waterAction.action.IsPressed();
        bool isWatering = wantsToWater && waterTank.HasWater;

        UpdateSpray(isWatering);

        if (isWatering)
        {
            waterTank.UseWater(Time.deltaTime);
            WaterPlantsBelow();
        }
    }

    // Allume ou éteint le jet de particules
    private void UpdateSpray(bool isWatering)
    {
        if (waterSpray == null)
        {
            return;
        }

        if (isWatering && !waterSpray.isPlaying)
        {
            waterSpray.Play();
        }
        else if (!isWatering && waterSpray.isPlaying)
        {
            waterSpray.Stop();
        }
    }

    // Cherche toutes les plantations dans la zone sous le drone et les arrose
    private void WaterPlantsBelow()
    {
        // On récupère tous les colliders dans une sphère au sol, sous le drone
        // (QueryTriggerInteraction.Collide = on inclut aussi les Triggers)
        Collider[] hits = Physics.OverlapSphere(GetWateringPoint(), wateringRadius,
                                                Physics.AllLayers, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            // Le sol a aussi un collider : on ne garde que ce qui porte un script Plant
            Plant plant = hit.GetComponent<Plant>();
            if (plant != null)
            {
                plant.Water();
            }
        }
    }

    // Le point au sol juste sous le drone
    // Le point au sol juste sous le drone
    private Vector3 GetWateringPoint()
    {
        if (groundSensor != null && groundSensor.TryGetGroundPoint(transform.position, out Vector3 groundPoint))
        {
            return groundPoint;
        }
        return transform.position; // secours : sous le drone
    }

    // Dessine la zone d'arrosage dans la vue Scene (uniquement quand le drone est sélectionné)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetWateringPoint(), wateringRadius);
    }
}