using TMPro;
using UnityEngine;

// Affiche le score de la partie qui vient de se terminer.
public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text finalScoreText;

    private void Start()
    {
        finalScoreText.text = $"Score final : {GameSession.LastScore}";
    }
}