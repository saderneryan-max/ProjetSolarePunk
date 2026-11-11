using UnityEngine;

// Détecte si le joueur est bloqué (plus rien pour continuer) et termine la partie.
// Évite un "softlock" : une partie impossible à finir, mais qui ne se termine jamais.
public class BankruptcyChecker : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Inventory crateStorage;     // la caisse : on peut y reprendre des récoltes
    [SerializeField] private Wallet wallet;
    [SerializeField] private Transform plantsParent;     // l'objet "Plants"
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CropData[] seedsForSale;
    [SerializeField] private float checkInterval = 1f;   // inutile de vérifier à chaque image

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < checkInterval) return;
        timer = 0f;

        if (IsStuck())
        {
            scoreManager.EndGame(false, 0, "Plus de graines, plus de récoltes, plus de Punk... Le potager a fait faillite !");
        }
    }

    // Le joueur est bloqué s'il n'a PLUS AUCUN moyen d'avancer
    private bool IsStuck()
    {
        if (plantsParent.childCount > 0) return false;      // quelque chose pousse encore
        if (playerInventory.Slots.Count > 0) return false;  // il a des graines ou des récoltes
        if (crateStorage.TotalCount > 0) return false;      // il peut reprendre des récoltes dans la caisse

        foreach (CropData crop in seedsForSale)
        {
            if (wallet.Punk >= crop.SeedPrice) return false; // il peut encore acheter une graine
        }
        return true;
    }
}