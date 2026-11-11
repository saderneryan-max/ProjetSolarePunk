using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Une carte de commande sur l'affiche : nom, difficulté, récoltes demandées, points, bouton "Choisir".
public class OrderCard : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private TMP_Text linesText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private Button chooseButton;
    [SerializeField] private TMP_Text buttonLabel;

    public Button ChooseButton => chooseButton;

    public void Setup(OrderData order, UnityAction onChoose, bool canChoose)
    {
        titleText.text = order.OrderName;
        difficultyText.text = order.Difficulty.ToString();
        difficultyText.color = GetDifficultyColor(order.Difficulty);
        linesText.text = BuildLines(order);
        pointsText.text = $"{order.BasePoints} pts";

        buttonLabel.text = canChoose ? "Choisir" : "En cours";
        chooseButton.interactable = canChoose;
        chooseButton.onClick.AddListener(onChoose);
    }

    // Construit "5 x Potiron Or" ligne par ligne, avec la qualité en couleur (texte enrichi TextMeshPro)
    private static string BuildLines(OrderData order)
    {
        StringBuilder builder = new StringBuilder();
        foreach (OrderLine line in order.Lines)
        {
            string hex = ColorUtility.ToHtmlStringRGB(QualityUtils.GetDarkColor(line.Quality));
            builder.AppendLine($"{line.Count} x {line.Crop.DisplayName} <color=#{hex}><b>{QualityUtils.GetName(line.Quality)}</b></color>");
        }
        return builder.ToString();
    }

    private static Color GetDifficultyColor(OrderDifficulty difficulty)
    {
        switch (difficulty)
        {
            case OrderDifficulty.Facile: return new Color(0.30f, 0.69f, 0.42f); // vert
            case OrderDifficulty.Moyenne: return new Color(0.85f, 0.60f, 0.15f); // orange
            default: return new Color(0.80f, 0.30f, 0.27f); // rouge
        }
    }
}