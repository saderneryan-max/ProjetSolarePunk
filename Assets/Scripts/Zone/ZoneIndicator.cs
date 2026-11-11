using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Affiche la zone survolée, son icône et ce qu'on peut y faire,
// avec une petite animation à chaque changement de zone.
public class ZoneIndicator : MonoBehaviour
{
    [SerializeField] private ZoneDetector zoneDetector;
    [SerializeField] private Inventory inventory;
    [SerializeField] private TMP_Text zoneText;
    [SerializeField] private Image zoneIcon;

    [Header("Icônes spéciales")]
    [SerializeField] private Sprite waterIcon;     // au-dessus d'une mare
    [SerializeField] private Sprite noPlantIcon;   // zone où rien ne pousse

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string changeStateName = "Zone_Change";

    private string lastText = "";

    private void Update()
    {
        Plant plant = zoneDetector.GetPlantToPlant(out string zoneName);

        string newText;
        Sprite newIcon;

        if (zoneDetector.IsAboveWater())
        {
            newText = $"{zoneName} : remplissage";
            newIcon = waterIcon;
        }
        else if (plant != null)
        {
            int seeds = inventory != null
                ? inventory.CountOf(plant.CropData, ItemKind.Seed, CropQuality.Bronze)
                : 0;

            newText = seeds > 0
                ? $"{zoneName} : {plant.DisplayName} (x{seeds})"
                : $"{zoneName} : plus de graine de {plant.DisplayName} !";
            newIcon = plant.Icon;
        }
        else
        {
            newText = $"{zoneName} : rien ne pousse ici";
            newIcon = noPlantIcon;
        }

        // On ne met à jour (et on n'anime) QUE si la zone a changé
        if (newText != lastText)
        {
            lastText = newText;
            zoneText.text = newText;

            if (zoneIcon != null)
            {
                zoneIcon.sprite = newIcon;
                zoneIcon.enabled = newIcon != null; // pas d'icône = on cache l'image (pas de carré blanc)
            }

            if (animator != null)
            {
                animator.Play(changeStateName, 0, 0f);
            }
        }
    }
}