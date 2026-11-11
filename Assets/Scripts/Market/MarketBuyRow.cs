using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Une ligne de la colonne "Acheter" : une graine, son prix, un bouton.
public class MarketBuyRow : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    private CropData crop;

    public Button BuyButton => buyButton;

    public void Setup(CropData cropData, MarketUI market)
    {
        crop = cropData;
        icon.sprite = crop.Icon;
        nameText.text = $"Graine de {crop.DisplayName}";
        priceText.text = $"{crop.SeedPrice} Punk";
        buyButton.onClick.AddListener(() => market.BuySeed(crop));
    }

    // Le bouton est grisé si on n'a pas assez d'argent
    public void RefreshAffordable(int punk)
    {
        buyButton.interactable = punk >= crop.SeedPrice;
    }
}