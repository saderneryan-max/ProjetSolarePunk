using TMPro;
using UnityEngine;

// Fenêtre de l'affiche : propose 3 commandes, puis affiche celle qui est en cours.
public class OrderBoardUI : GameWindow
{
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private OrderCard cardPrefab;
    [SerializeField] private TMP_Text infoText;

    public void Open()
    {
        OpenWindow();
    }

    protected override void Refresh()
    {
        ClearChildren(cardsContainer);
        OrderCard firstCard = null;

        if (orderManager.CurrentOrder == null)
        {
            infoText.text = "Choisis ta commande : plus elle est difficile, plus elle rapporte !";
            foreach (OrderData order in orderManager.Offers)
            {
                OrderData chosen = order; // chaque bouton "retient" SA commande
                OrderCard card = Instantiate(cardPrefab, cardsContainer);
                card.Setup(order, () => Choose(chosen), true);
                if (firstCard == null) firstCard = card;
            }
        }
        else
        {
            infoText.text = "Commande en cours : livre-la dans la caisse au plus vite !";
            OrderCard card = Instantiate(cardPrefab, cardsContainer);
            card.Setup(orderManager.CurrentOrder, () => { }, false);
        }

        GameObject preferred = firstCard != null ? firstCard.ChooseButton.gameObject : closeButton.gameObject;
        EnsureSelection(preferred, cardsContainer);
    }

    private void Choose(OrderData order)
    {
        orderManager.ChooseOrder(order);
        Refresh();
    }
}