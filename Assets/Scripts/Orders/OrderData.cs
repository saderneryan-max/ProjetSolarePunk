using UnityEngine;

public enum OrderDifficulty
{
    Facile,
    Moyenne,
    Difficile
}

// Une ligne de commande : "5 Potiron Or"
[System.Serializable]
public class OrderLine
{
    public CropData Crop;
    public CropQuality Quality;
    [Min(1)] public int Count = 1;
}

// Fiche d'une commande : son nom, sa difficulté, ses points et la liste des récoltes demandées.
[CreateAssetMenu(fileName = "Order_", menuName = "Potager/Order Data")]
public class OrderData : ScriptableObject
{
    [SerializeField] private string orderName = "Commande";
    [SerializeField] private OrderDifficulty difficulty = OrderDifficulty.Facile;
    [SerializeField] private int basePoints = 100;
    [SerializeField] private OrderLine[] lines;

    public string OrderName => orderName;
    public OrderDifficulty Difficulty => difficulty;
    public int BasePoints => basePoints;
    public OrderLine[] Lines => lines;

    // Nombre total de récoltes demandées
    public int TotalCount
    {
        get
        {
            int total = 0;
            if (lines != null)
            {
                foreach (OrderLine line in lines) total += line.Count;
            }
            return total;
        }
    }

    // Combien de récoltes EXACTEMENT de ce type (légume + qualité) la commande demande-t-elle ?
    public int GetRequired(CropData crop, CropQuality quality)
    {
        int required = 0;
        foreach (OrderLine line in lines)
        {
            if (line.Crop == crop && line.Quality == quality) required += line.Count;
        }
        return required;
    }

    // Appelée par Unity dans l'éditeur à chaque modification de la fiche : vérifie qu'on a bien 15 récoltes
    private void OnValidate()
    {
        if (lines != null && lines.Length > 0 && TotalCount != 15)
        {
            Debug.LogWarning($"{name} : cette commande demande {TotalCount} récoltes au lieu de 15.");
        }
    }
}