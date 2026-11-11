using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Fenêtre de la caisse : à gauche les récoltes du joueur, à droite la commande et son avancement.
public class DeliveryUI : GameWindow
{
    [Header("Données")]
    [SerializeField] private Inventory playerInventory;

    [Header("Interface")]
    [SerializeField] private Transform inventoryList;
    [SerializeField] private Transform crateList;
    [SerializeField] private DeliveryRow rowPrefab;
    [SerializeField] private GameObject emptyInventoryText;
    [SerializeField] private TMP_Text crateTitle;
    [SerializeField] private TMP_Text bonusText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button shipButton;

    private DeliveryCrate crate;

    protected override void Start()
    {
        base.Start();
        shipButton.onClick.AddListener(Ship);
    }

    public void Open(DeliveryCrate target)
    {
        crate = target;
        OpenWindow();
    }

    protected override void OnOpened()
    {
        feedbackText.text = "";
        playerInventory.OnChanged += Refresh;
        crate.Storage.OnChanged += Refresh;
    }

    protected override void OnClosed()
    {
        playerInventory.OnChanged -= Refresh;
        crate.Storage.OnChanged -= Refresh;
    }

    protected override void Refresh()
    {
        ClearChildren(inventoryList);
        ClearChildren(crateList);

        // Pas encore de commande : on renvoie le joueur vers l'affiche
        if (crate.Order == null)
        {
            crateTitle.text = "Aucune commande";
            bonusText.text = "";
            feedbackText.text = "Choisis d'abord une commande sur l'affiche de la base !";
            emptyInventoryText.SetActive(false);
            shipButton.interactable = false;
            EnsureSelection(closeButton.gameObject, inventoryList, crateList);
            return;
        }

        // Colonne de gauche : les récoltes du joueur
        DeliveryRow firstRow = null;
        foreach (InventorySlot slot in playerInventory.Slots)
        {
            if (slot.Kind != ItemKind.Harvest) continue;

            CropData crop = slot.Crop;
            CropQuality quality = slot.Quality;
            bool wanted = crate.CanAccept(crop, quality);

            DeliveryRow row = Instantiate(rowPrefab, inventoryList);
            row.Setup(crop, quality,
                      $"{crop.DisplayName} {QualityUtils.GetName(quality)}  x{slot.Count}",
                      wanted ? "Demandé !" : "Non demandé",
                      "Déposer", () => Deposit(crop, quality), wanted);

            if (firstRow == null && wanted) firstRow = row;
        }
        emptyInventoryText.SetActive(inventoryList.childCount == 0);

        // Colonne de droite : chaque ligne de la commande et son avancement
        foreach (OrderLine line in crate.Order.Lines)
        {
            CropData crop = line.Crop;
            CropQuality quality = line.Quality;
            int delivered = crate.DeliveredOf(crop, quality);

            DeliveryRow row = Instantiate(rowPrefab, crateList);
            row.Setup(crop, quality,
                      $"{crop.DisplayName} {QualityUtils.GetName(quality)}",
                      $"{delivered} / {line.Count}",
                      "Reprendre", () => Withdraw(crop, quality), delivered > 0);
        }

        crateTitle.text = $"{crate.Order.OrderName} : {crate.Count} / {crate.Required}";
        bonusText.text = $"{crate.Order.BasePoints} pts x{crate.TimeMultiplier:0.#} = {crate.GetOrderScore()} pts";
        shipButton.interactable = crate.IsComplete;

        GameObject preferred = crate.IsComplete ? shipButton.gameObject
                             : firstRow != null ? firstRow.ActionButton.gameObject
                             : closeButton.gameObject;
        EnsureSelection(preferred, inventoryList, crateList);
    }

    private void Deposit(CropData crop, CropQuality quality)
    {
        if (!crate.Deposit(crop, quality))
        {
            feedbackText.text = "La commande n'en demande plus !";
        }
    }

    private void Withdraw(CropData crop, CropQuality quality)
    {
        if (!crate.Withdraw(crop, quality))
        {
            feedbackText.text = "Inventaire plein !";
        }
    }

    private void Ship()
    {
        Close();
        crate.Ship();
    }
}