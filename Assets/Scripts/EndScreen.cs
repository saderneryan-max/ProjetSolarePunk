using TMPro;
using UnityEngine;

// Affiche le score de la partie, le meilleur score, et un message en cas de nouveau record.
public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject newRecordLabel; // texte "Nouveau record !"

    private void Start()
    {
        finalScoreText.text = $"Score final : {GameSession.LastScore}";
        bestScoreText.text = $"Meilleur score : {BestScoreSaver.Load()}";

        // On n'affiche "Nouveau record !" que si c'en est un
        newRecordLabel.SetActive(GameSession.IsNewRecord);
    }
}