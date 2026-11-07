using UnityEngine;

// Fiche d'identité d'un légume : nom, icône, prix.
// C'est un ScriptableObject : un FICHIER de données qu'on crée dans le Project (pas un objet de la scène).
[CreateAssetMenu(fileName = "Crop_", menuName = "Potager/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("Identité")]
    [SerializeField] private string displayName = "Carotte";
    [SerializeField] private Sprite icon;

    [Header("Prix de vente (en Punk)")]
    [SerializeField] private int minSellPrice = 1;   // qualité Bronze
    [SerializeField] private int maxSellPrice = 3;   // qualité Or

    [Header("Graine")]
    [SerializeField] private int seedPrice = 1;      // prix d'achat d'une graine (bloc 2)

    // "=>" est une écriture courte de { get { return ...; } }
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public int SeedPrice => seedPrice;

    // Prix de vente selon la qualité : Bronze = min, Or = max, Argent = entre les deux
    public int GetSellPrice(CropQuality quality)
    {
        float t = QualityUtils.GetPriceFactor(quality);
        return Mathf.RoundToInt(Mathf.Lerp(minSellPrice, maxSellPrice, t));
    }
}