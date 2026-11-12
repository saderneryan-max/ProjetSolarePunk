using UnityEngine;
using UnityEngine.InputSystem;

// Repère l'objet interactif près duquel se trouve le drone, affiche l'invite, et déclenche l'interaction.
public class DroneInteractor : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;   // Gameplay/Interact
    [SerializeField] private InteractionPromptUI prompt;

    private Interactable current; // l'objet interactif tout proche (null = aucun)

    private void OnEnable()
    {
        interactAction.action.Enable();
        if (current != null) prompt.Show(current); // on revient d'un menu : on réaffiche l'invite
    }

    private void OnDisable()
    {
        interactAction.action.Disable();

        // Sécurité : à la fermeture de la scène, le HUD peut avoir été détruit avant le drone
        if (prompt != null)
        {
            prompt.Hide();
        }
    }

    private void Update()
    {
        if (current != null && interactAction.action.WasPressedThisFrame())
        {
            current.Interact(); // le drone ne sait pas CE QUE c'est : chaque objet fait sa propre action
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // GetComponentInParent : la zone de détection est un ENFANT de l'objet interactif
        Interactable interactable = other.GetComponentInParent<Interactable>();
        if (interactable != null)
        {
            current = interactable;
            if (enabled) prompt.Show(current);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Le duo inséparable : ce qu'on a noté à l'entrée, on l'efface à la sortie
        Interactable interactable = other.GetComponentInParent<Interactable>();
        if (interactable != null && interactable == current)
        {
            current = null;
            prompt.Hide();
        }
    }
}