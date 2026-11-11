using TMPro;
using UnityEngine;

// Zone de détection d'un panneau de tuto : affiche son texte en grand à l'écran
// tant que le drone est dedans. À placer sur l'InteractZone (Box Collider en Trigger).
[RequireComponent(typeof(Collider))]
public class InfoBoardZone : MonoBehaviour
{
    [SerializeField] private TMP_Text sourceText;   // le texte écrit sur le panneau 3D
    [SerializeField] private InfoPanelUI infoPanel;

    // Appelée par Unity quand on AJOUTE le composant : on coche "Is Trigger" automatiquement
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DroneMovement>() == null) return; // seul le drone nous intéresse
        infoPanel.Show(this, sourceText.text);
    }

    // Le duo inséparable : ce qu'on affiche à l'entrée, on le cache à la sortie
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DroneMovement>() == null) return;
        infoPanel.Hide(this);
    }

    // Sécurité : si la zone disparaît (changement de scène...), on ne laisse pas le panneau affiché
    private void OnDisable()
    {
        if (infoPanel != null) infoPanel.Hide(this);
    }
}