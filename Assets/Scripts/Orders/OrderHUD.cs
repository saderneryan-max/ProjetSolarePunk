using System.Text;
using TMPro;
using UnityEngine;

// Encart du HUD : commande en cours, avancement de chaque ligne, chrono et multiplicateur.
public class OrderHUD : MonoBehaviour
{
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private DeliveryCrate crate;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text linesText;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        // Chrono "mm:ss" + multiplicateur actuel
        int totalSeconds = Mathf.FloorToInt(orderManager.ElapsedSeconds);
        timerText.text = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}   x{orderManager.TimeMultiplier:0.#}";

        OrderData order = orderManager.CurrentOrder;
        if (order == null)
        {
            titleText.text = "Aucune commande";
            linesText.text = "Va choisir une commande sur l'affiche de la base !";
            return;
        }

        titleText.text = order.OrderName;

        StringBuilder builder = new StringBuilder();
        foreach (OrderLine line in order.Lines)
        {
            int delivered = crate.DeliveredOf(line.Crop, line.Quality);
            string hex = ColorUtility.ToHtmlStringRGB(QualityUtils.GetColor(line.Quality));
            // Une ligne terminée passe en vert
            string countColor = delivered >= line.Count ? "#8CFF7A" : "#FFF6E0";

            builder.AppendLine($"<color={countColor}>{delivered}/{line.Count}</color>  {line.Crop.DisplayName} <color=#{hex}>{QualityUtils.GetName(line.Quality)}</color>");
        }
        linesText.text = builder.ToString();
    }
}