using UnityEngine;

// Trouve le point du sol situé sous une position, grâce à un rayon lancé depuis le ciel.
// Utilisé par le déplacement, la plantation et l'arrosage du drone.
public class GroundSensor : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;        // le rayon ne touche QUE ce calque
    [SerializeField] private float rayStartHeight = 100f;  // hauteur de départ du rayon

    // Renvoie true s'il y a du sol sous "position", et donne le point d'impact dans groundPoint.
    public bool TryGetGroundPoint(Vector3 position, out Vector3 groundPoint)
    {
        Vector3 rayOrigin = new Vector3(position.x, rayStartHeight, position.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit,
                            rayStartHeight * 2f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            groundPoint = hit.point;
            return true;
        }

        // Pas de sol trouvé (en dehors du terrain)
        groundPoint = position;
        return false;
    }
}