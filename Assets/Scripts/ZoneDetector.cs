using UnityEngine;

// Détecte dans quelle zone se trouve le sol sous le drone,
// et en déduit quel légume on peut y planter.
public class ZoneDetector : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GroundSensor groundSensor;

    [Header("Détection")]
    [SerializeField] private LayerMask zoneLayer;            // le calque "Zone"
    [SerializeField] private float detectionRadius = 0.2f;

    [Header("En dehors de toute zone")]
    [SerializeField] private string defaultZoneName = "Plaine";
    [SerializeField] private Plant defaultPlantPrefab;       // la carotte

    // Renvoie le légume à planter sous le drone (null = rien ne pousse ici),
    // et donne le nom de la zone dans zoneName.
    public Plant GetPlantToPlant(out string zoneName)
    {
        PlantingZone zone = GetZoneBelow();

        if (zone != null)
        {
            zoneName = zone.ZoneName;
            return zone.PlantPrefab; // peut être null si la zone interdit de planter
        }

        zoneName = defaultZoneName;
        return defaultPlantPrefab;
    }

    // Le drone survole-t-il un point d'eau (mare) ?
    public bool IsAboveWater()
    {
        PlantingZone zone = GetZoneBelow();
        return zone != null && zone.IsWaterSource;
    }

    // Cherche une PlantingZone au point du sol juste sous le drone
    private PlantingZone GetZoneBelow()
    {
        if (!groundSensor.TryGetGroundPoint(transform.position, out Vector3 groundPoint))
        {
            return null;
        }

        Collider[] hits = Physics.OverlapSphere(groundPoint, detectionRadius,
                                                zoneLayer, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            PlantingZone zone = hit.GetComponent<PlantingZone>();
            if (zone != null)
            {
                return zone; // première zone trouvée
            }
        }

        return null; // aucune zone : on est en plaine
    }
}