using TMPro;
using UnityEngine;

// Affiche à l'écran la zone survolée et le légume qu'on peut y planter.
public class ZoneIndicator : MonoBehaviour
{
    [SerializeField] private ZoneDetector zoneDetector;
    [SerializeField] private TMP_Text zoneText;

    private void Update()
    {
        Plant plant = zoneDetector.GetPlantToPlant(out string zoneName);

        if (zoneDetector.IsAboveWater())
        {
            zoneText.text = $"{zoneName} : remplissage du réservoir";
        }
        else if (plant != null)
        {
            zoneText.text = $"{zoneName} : {plant.DisplayName}";
        }
        else
        {
            zoneText.text = $"{zoneName} : rien ne pousse ici";
        }
    }
}