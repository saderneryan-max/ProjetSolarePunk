using TMPro;
using UnityEngine;

// Affiche la zone survolée et ce qu'on peut y faire, avec une petite animation à chaque changement.
public class ZoneIndicator : MonoBehaviour
{
    [SerializeField] private ZoneDetector zoneDetector;
    [SerializeField] private TMP_Text zoneText;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string changeStateName = "Zone_Change";

    private string lastText = "";

    private void Update()
    {
        string newText = BuildText();

        // On ne touche au texte (et on n'anime) QUE s'il a changé
        if (newText != lastText)
        {
            zoneText.text = newText;
            lastText = newText;

            if (animator != null)
            {
                animator.Play(changeStateName, 0, 0f);
            }
        }
    }

    // Construit le message selon la zone survolée
    private string BuildText()
    {
        Plant plant = zoneDetector.GetPlantToPlant(out string zoneName);

        if (zoneDetector.IsAboveWater())
        {
            return $"{zoneName} : remplissage du réservoir";
        }
        if (plant != null)
        {
            return $"{zoneName} : {plant.DisplayName}";
        }
        return $"{zoneName} : rien ne pousse ici";
    }
}