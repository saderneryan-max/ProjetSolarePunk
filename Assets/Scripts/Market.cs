using UnityEngine;

// Le marché de la base : hérite d'Interactable, et ouvre la fenêtre du marché.
public class Market : Interactable
{
    [SerializeField] private MarketUI marketUI;

    // "override" : on écrit NOTRE version de la méthode promise par Interactable
    public override void Interact()
    {
        marketUI.Open();
    }
}