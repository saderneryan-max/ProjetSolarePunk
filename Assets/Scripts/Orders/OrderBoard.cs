using UnityEngine;

// L'affiche des commandes de la base : encore un Interactable, le drone n'a rien à changer.
public class OrderBoard : Interactable
{
    [SerializeField] private OrderBoardUI orderBoardUI;

    public override void Interact()
    {
        orderBoardUI.Open();
    }
}