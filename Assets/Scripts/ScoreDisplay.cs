using TMPro;
using UnityEngine;

// Affiche le score dans un texte de l'interface.
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string prefix = "Score : ";

    public void ShowScore(int score)
    {
        scoreText.text = prefix + score;
    }
}