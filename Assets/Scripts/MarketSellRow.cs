using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Une ligne de la colonne "Vendre" : une pile de récoltes, son prix, et deux boutons.
public class MarketSellRow : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image qualityBadge;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button sellOneButton;
    [SerializeField] private Button sellAllButton;

    public Button FirstButton => sellOneButton; // pour la sélection à la manette

    public void Setup(InventorySlot slot, MarketUI market)
    {
        CropData crop = slot.Crop;
        CropQuality quality = slot.Quality;
        int unitPrice = crop.GetSellPrice(quality);

        icon.sprite = crop.Icon;
        qualityBadge.color = QualityUtils.GetColor(quality);
        nameText.text = $"{crop.DisplayName} {QualityUtils.GetName(quality)}  x{slot.Count}";
        priceText.text = $"{unitPrice} Punk / u";

        // On branche les boutons PAR LE CODE (les lignes sont créées pendant le jeu)
        int count = slot.Count;
        sellOneButton.onClick.AddListener(() => market.Sell(crop, quality, 1));
        sellAllButton.onClick.AddListener(() => market.Sell(crop, quality, count));
    }
}