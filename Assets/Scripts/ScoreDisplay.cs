using TMPro;
using UnityEngine;

// Affiche le score et la progression, et fait réagir le texte quand le score change.
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string gainStateName = "Score_Gain";
    [SerializeField] private string lossStateName = "Score_Loss";

    private int lastScore = 0;
    private bool hasShownOnce = false; // pas d'animation au tout premier affichage

    public void ShowScore(int score, int harvestCount, int harvestGoal)
    {
        scoreText.text = $"Score : {score}\nRécoltes : {harvestCount} / {harvestGoal}";

        if (hasShownOnce && animator != null)
        {
            // On compare avec le score précédent pour choisir la bonne réaction
            if (score > lastScore)
            {
                animator.Play(gainStateName, 0, 0f);
            }
            else if (score < lastScore)
            {
                animator.Play(lossStateName, 0, 0f);
            }
        }

        lastScore = score;
        hasShownOnce = true;
    }
}