using UnityEngine;

// Caisse de livraison : on y dépose les récoltes demandées par la commande choisie.
public class DeliveryCrate : Interactable
{
    [Header("Références")]
    [SerializeField] private Inventory crateStorage;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private DeliveryUI deliveryUI;

    public Inventory Storage => crateStorage;
    public OrderData Order => orderManager.CurrentOrder;
    public float TimeMultiplier => orderManager.TimeMultiplier;
    public int Count => crateStorage.TotalCount;
    public int Required => Order != null ? Order.TotalCount : 15;

    private void OnEnable()
    {
        crateStorage.OnChanged += UpdateProgress;
        orderManager.OnOrderChosen += UpdateProgress;
    }

    private void OnDisable()
    {
        crateStorage.OnChanged -= UpdateProgress;
        orderManager.OnOrderChosen -= UpdateProgress;
    }

    private void Start()
    {
        UpdateProgress();
    }

    public override void Interact()
    {
        deliveryUI.Open(this);
    }

    public int DeliveredOf(CropData crop, CropQuality quality)
    {
        return crateStorage.CountOf(crop, ItemKind.Harvest, quality);
    }

    // La commande demande-t-elle encore ce type EXACT de récolte ?
    public bool CanAccept(CropData crop, CropQuality quality)
    {
        if (Order == null) return false;
        return DeliveredOf(crop, quality) < Order.GetRequired(crop, quality);
    }

    // Toutes les lignes de la commande sont-elles remplies ?
    public bool IsComplete
    {
        get
        {
            if (Order == null) return false;
            foreach (OrderLine line in Order.Lines)
            {
                if (DeliveredOf(line.Crop, line.Quality) < line.Count) return false;
            }
            return true;
        }
    }

    public bool Deposit(CropData crop, CropQuality quality)
    {
        if (!CanAccept(crop, quality)) return false;
        if (!playerInventory.TryRemove(crop, ItemKind.Harvest, quality)) return false;

        crateStorage.TryAdd(crop, ItemKind.Harvest, quality);
        return true;
    }

    public bool Withdraw(CropData crop, CropQuality quality)
    {
        if (DeliveredOf(crop, quality) == 0) return false;
        if (!playerInventory.TryAdd(crop, ItemKind.Harvest, quality)) return false;

        crateStorage.TryRemove(crop, ItemKind.Harvest, quality);
        return true;
    }

    // Points de la commande, multipliés selon le temps mis
    public int GetOrderScore()
    {
        if (Order == null) return 0;
        return Mathf.RoundToInt(Order.BasePoints * TimeMultiplier);
    }

    public void Ship()
    {
        if (!IsComplete) return;
        scoreManager.EndGame(true, GetOrderScore(),
            $"Commande « {Order.OrderName} » livrée ! Bonus de rapidité x{TimeMultiplier:0.#}");
    }

    private void UpdateProgress()
    {
        scoreManager.SetDeliveryProgress(Count, Required);
    }
}