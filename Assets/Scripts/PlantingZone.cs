using UnityEngine;

// Une zone du potager (colline, creux, mare...) qui décide quel légume peut y pousser.
// À poser sur un objet avec un BoxCollider en Trigger, sur le calque "Zone".
[RequireComponent(typeof(BoxCollider))]
public class PlantingZone : MonoBehaviour
{
    [SerializeField] private string zoneName = "Colline";
    [SerializeField] private Plant plantPrefab;   // laisser VIDE = rien ne peut pousser ici
    [SerializeField] private Color gizmoColor = new Color(1f, 0.8f, 0f, 0.3f);

    public string ZoneName { get { return zoneName; } }
    public Plant PlantPrefab { get { return plantPrefab; } }

    // Dessine la zone en couleur dans la vue Scene (invisible en jeu)
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Gizmos.color = gizmoColor;
        // Applique la position, la rotation et la taille de l'objet au dessin
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(box.center, box.size);
    }
}