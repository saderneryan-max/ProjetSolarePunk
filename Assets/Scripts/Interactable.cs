using UnityEngine;

// Classe de BASE de tout ce avec quoi le drone peut interagir (marché, caisse de livraison...).
// "abstract" : on ne peut pas l'utiliser seule, il faut créer une classe qui en hérite.
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private string promptText = "Interagir";  // texte de l'invite
    [SerializeField] private Transform promptAnchor;            // où afficher l'invite (au-dessus de l'objet)

    public string PromptText => promptText;
    public Vector3 PromptPosition => promptAnchor != null ? promptAnchor.position : transform.position + Vector3.up * 2f;

    // Chaque objet décide de ce qui se passe quand on interagit avec lui
    public abstract void Interact();
}