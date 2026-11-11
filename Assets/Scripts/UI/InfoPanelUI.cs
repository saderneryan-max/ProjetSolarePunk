using TMPro;
using UnityEngine;

// Grand panneau d'information à l'écran : affiche le texte d'un panneau de la base
// tant que le drone se trouve dans sa zone de détection.
// À placer sur HUD (toujours actif) ; "panel" est l'objet affiché / caché.
public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text infoText;

    private InfoBoardZone currentZone; // la zone qui a demandé l'affichage

    private void Start()
    {
        panel.SetActive(false);
    }

    public void Show(InfoBoardZone zone, string text)
    {
        currentZone = zone;
        infoText.text = text;

        // Désactiver puis réactiver relance l'animation d'ouverture, même si un panneau était déjà affiché
        panel.SetActive(false);
        panel.SetActive(true);
    }

    public void Hide(InfoBoardZone zone)
    {
        // Si une AUTRE zone a pris le relais entre-temps, on ne cache pas son panneau
        if (zone != currentZone) return;

        currentZone = null;
        panel.SetActive(false);
    }
}