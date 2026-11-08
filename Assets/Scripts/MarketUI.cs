using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Fenêtre du marché : vendre ses récoltes, acheter des graines.
// À placer sur le Canvas (PAS sur MarketPanel, qui est désactivé quand la fenêtre est fermée).
public class MarketUI : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Wallet wallet;
    [SerializeField] private CropData[] seedsForSale;      // carotte, tournesol, potiron

    [Header("Interface")]
    [SerializeField] private GameObject panel;             // MarketPanel
    [SerializeField] private Transform sellList;
    [SerializeField] private MarketSellRow sellRowPrefab;
    [SerializeField] private GameObject emptySellText;
    [SerializeField] private Transform buyList;
    [SerializeField] private MarketBuyRow buyRowPrefab;
    [SerializeField] private TMP_Text punkText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button closeButton;

    [Header("Input")]
    [SerializeField] private InputActionReference cancelAction;  // Gameplay/Cancel (Échap / B)

    [Header("Scripts à couper pendant que le marché est ouvert")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private readonly List<MarketBuyRow> buyRows = new List<MarketBuyRow>();
    private int openedFrame = -1;

    public bool IsOpen => panel.activeSelf;

    private void Start()
    {
        // Les lignes d'achat ne changent jamais : on les crée une seule fois
        foreach (CropData crop in seedsForSale)
        {
            MarketBuyRow row = Instantiate(buyRowPrefab, buyList);
            row.Setup(crop, this);
            buyRows.Add(row);
        }

        closeButton.onClick.AddListener(Close);
        panel.SetActive(false);
    }

    private void Update()
    {
        // On ignore l'image d'ouverture : sinon le B qui a OUVERT le marché le refermerait aussitôt
        if (IsOpen && Time.frameCount != openedFrame && cancelAction.action.WasPressedThisFrame())
        {
            Close();
        }
    }

    public void Open()
    {
        if (IsOpen) return;

        openedFrame = Time.frameCount;
        SetScriptsEnabled(false);          // le drone, la caméra, la pause et l'inventaire se figent
        cancelAction.action.Enable();

        inventory.OnChanged += Refresh;    // abonnements : la fenêtre se met à jour toute seule
        wallet.OnChanged += OnWalletChanged;

        feedbackText.text = "";
        panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        if (!IsOpen) return;

        inventory.OnChanged -= Refresh;    // désabonnements, toujours par paire
        wallet.OnChanged -= OnWalletChanged;

        cancelAction.action.Disable();
        panel.SetActive(false);
        SetScriptsEnabled(true);
    }

    // ---------- Actions des boutons ----------

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

        // On vérifie la place AVANT de payer : jamais payer pour rien
        if (!inventory.TryAdd(crop, ItemKind.Seed, CropQuality.Bronze))
        {
            feedbackText.text = "Inventaire plein !";
            return;
        }

        wallet.TrySpend(crop.SeedPrice);
        feedbackText.text = $"Graine de {crop.DisplayName} achetée !";
    }

    // ---------- Affichage ----------

    private void OnWalletChanged(int punk)
    {
        Refresh();
    }

    private void Refresh()
    {
        // 1. On efface les anciennes lignes de vente...
        foreach (Transform child in sellList)
        {
            Destroy(child.gameObject);
        }

        // 2. ...et on recrée une ligne par pile de récoltes
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

        // 3. Manette : si le bouton sélectionné vient d'être détruit, on en sélectionne un autre
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || !selected.activeInHierarchy || selected.transform.IsChildOf(sellList))
        {
            GameObject next = firstRow != null ? firstRow.FirstButton.gameObject : closeButton.gameObject;
            EventSystem.current.SetSelectedGameObject(next);
        }
    }

    private void SetScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null) script.enabled = value;
        }
    }
}