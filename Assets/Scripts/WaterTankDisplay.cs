using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Affiche la jauge du réservoir : niveau d'eau, couleur d'alerte et message d'état.
public class WaterTankDisplay : MonoBehaviour
{
    [SerializeField] private Image fillImage;       // une Image en mode "Filled"
    [SerializeField] private TMP_Text statusText;

    [Header("Couleurs")]
    [SerializeField] private Color normalColor = new Color(0.3f, 0.75f, 1f);   // bleu
    [SerializeField] private Color lowColor = new Color(1f, 0.4f, 0.3f);       // rouge-orangé
    [SerializeField][Range(0f, 1f)] private float lowThreshold = 0.25f;       // alerte sous 25 %

    // fillRatio : 0 = vide, 1 = plein
    public void Show(float fillRatio, bool isRefilling)
    {
        fillImage.fillAmount = fillRatio;
        fillImage.color = fillRatio <= lowThreshold ? lowColor : normalColor;

        if (isRefilling)
        {
            statusText.text = "Remplissage...";
        }
        else if (fillRatio <= 0f)
        {
            statusText.text = "Réservoir vide ! Va à une mare";
        }
        else
        {
            statusText.text = $"Eau : {Mathf.RoundToInt(fillRatio * 100f)} %";
        }
    }
}