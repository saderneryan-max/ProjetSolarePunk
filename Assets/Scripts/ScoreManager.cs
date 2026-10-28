using UnityEngine;

// Garde le score de la partie et prévient l'affichage quand il change.
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreDisplay scoreDisplay; // le texte du score à l'écran

    // Tout le monde peut lire le score, seul ce script peut le modifier
    public int Score { get; private set; }

    private void Start()
    {
        Score = 0;
        RefreshDisplay(); // affiche "0" dès le début
    }

    // Appelée par une plante quand elle est récoltée
    public void AddPoints(int amount)
    {
        Score += amount;
        RefreshDisplay(); // mise à jour immédiate, sans délai
    }

    private void RefreshDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.ShowScore(Score);
        }
    }
}