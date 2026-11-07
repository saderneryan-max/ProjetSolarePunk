using TMPro;
using UnityEngine;

// Affiche les Punk dans le HUD et fait réagir le texte quand le solde change.
public class WalletDisplay : MonoBehaviour
{
    [SerializeField] private Wallet wallet;
    [SerializeField] private TMP_Text punkText;
    [SerializeField] private Animator animator;          // facultatif : réutilise un "pop"
    [SerializeField] private string popStateName = "Score_Gain";

    private void OnEnable()
    {
        wallet.OnChanged += Show;   // abonnement...
    }

    private void OnDisable()
    {
        wallet.OnChanged -= Show;   // ...et désabonnement, toujours par paire
    }

    private void Show(int punk)
    {
        punkText.text = $"{punk} Punk";

        if (animator != null)
        {
            animator.Play(popStateName, 0, 0f);
        }
    }
}