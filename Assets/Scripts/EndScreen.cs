using TMPro;
using UnityEngine;

// Écran de fin : victoire ou défaite, détail du score, meilleur score, message.
public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;        // "Récolte" / "Partie"
    [SerializeField] private TMP_Text subtitleText;     // "terminée !" / "perdue..."
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text detailText;       // "Récoltes : 40  +  Livraison : 320"
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject newRecordLabel;

    private void Start()
    {
        bool victory = GameSession.IsVictory;

        titleText.text = victory ? "Livraison" : "Partie";
        subtitleText.text = victory ? "réussie !" : "perdue...";

        finalScoreText.text = $"Score final : {GameSession.LastScore}";
        detailText.text = $"Récoltes : {GameSession.HarvestScore}   +   Livraison : {GameSession.DeliveryBonus}";
        bestScoreText.text = $"Meilleur score : {BestScoreSaver.Load()}";
        messageText.text = GameSession.EndMessage;

        newRecordLabel.SetActive(GameSession.IsNewRecord);
    }
}