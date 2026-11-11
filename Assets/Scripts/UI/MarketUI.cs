using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Fenêtre du marché : vendre ses récoltes, acheter des graines.
// L'ouverture / fermeture est gérée par la classe de base GameWindow.
public class MarketUI : GameWindow
{
    [Header("Données")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Wallet wallet;
    [SerializeField] private CropData[] seedsForSale;

    [Header("Interface")]
    [SerializeField] private Transform sellList;
    [SerializeField] private MarketSellRow sellRowPrefab;
    [SerializeField] private GameObject emptySellText;
    [SerializeField] private Transform buyList;
    [SerializeField] private MarketBuyRow buyRowPrefab;
    [SerializeField] private TMP_Text punkText;
    [SerializeField] private TMP_Text feedbackText;

    private readonly List<MarketBuyRow> buyRows = new List<MarketBuyRow>();

    protected override void Start()
    {
        base.Start(); // on garde le comportement de GameWindow (bouton Fermer, panneau caché)

        foreach (CropData crop in seedsForSale)
        {
            MarketBuyRow row = Instantiate(buyRowPrefab, buyList);
            row.Setup(crop, this);
            buyRows.Add(row);
        }
    }

    public void Open()
    {
        OpenWindow();
    }

    protected override void OnOpened()
    {
        feedbackText.text = "";
        inventory.OnChanged += Refresh;
        wallet.OnChanged += OnWalletChanged;
    }

    protected override void OnClosed()
    {
        inventory.OnChanged -= Refresh;
        wallet.OnChanged -= OnWalletChanged;
    }

    public void Sell(CropData crop, CropQuality quality, int amount)
    {
        if (inventory.TryRemove(crop, ItemKind.Harvest, quality, amount))
        {
            int gain = crop.GetSellPrice(quality) * amount;
            wallet.Add(gain);
            feedbackText.text = $"+{gain} Punk !";
        }
    }

    public void BuySeed(CropData crop)
    {
        if (wallet.Punk < crop.SeedPrice)
        {
            feedbackText.text = "Pas assez de Punk...";
            return;
        }
        if (!inventory.TryAdd(crop, ItemKind.Seed, CropQuality.Bronze))
        {
            feedbackText.text = "Inventaire plein !";
            return;
        }

        wallet.TrySpend(crop.SeedPrice);
        feedbackText.text = $"Graine de {crop.DisplayName} achetée !";
    }

    private void OnWalletChanged(int punk) => Refresh();

    protected override void Refresh()
    {
        ClearChildren(sellList);

        MarketSellRow firstRow = null;
        foreach (InventorySlot slot in inventory.Slots)
        {
            if (slot.Kind != ItemKind.Harvest) continue;

            MarketSellRow row = Instantiate(sellRowPrefab, sellList);
            row.Setup(slot, this);
            if (firstRow == null) firstRow = row;
        }

        emptySellText.SetActive(firstRow == null);
        punkText.text = $"{wallet.Punk} Punk";

        foreach (MarketBuyRow row in buyRows)
        {
            row.RefreshAffordable(wallet.Punk);
        }

        GameObject preferred = firstRow != null ? firstRow.FirstButton.gameObject : closeButton.gameObject;
        EnsureSelection(preferred, sellList);
    }
}