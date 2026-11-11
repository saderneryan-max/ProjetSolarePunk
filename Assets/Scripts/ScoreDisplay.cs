using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Affiche le score, le nombre de récoltes et la progression vers l'objectif,
// et fait réagir le score quand il change.
public class ScoreDisplay : MonoBehaviour
{
    [Header("Affichage")]
    [SerializeField] private TMP_Text scoreText;       // le nombre de points
    [SerializeField] private TMP_Text harvestText;     // "4 / 15"
    [SerializeField] private Image harvestBarFill;     // barre de progression (Image en mode Filled)
    [SerializeField] private TMP_Text lostText;        // "3 / 12" légumes perdus

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string gainStateName = "Score_Gain";
    [SerializeField] private string lossStateName = "Score_Loss";

    private int lastScore = 0;
    private bool hasShownOnce = false;

    public void ShowScore(int score, int harvestCount, int harvestGoal)
    {
        scoreText.text = score.ToString();
        harvestText.text = $"{harvestCount} / {harvestGoal}";

        if (harvestBarFill != null)
        {
            // (float) est INDISPENSABLE : sans lui, 4 / 15 donnerait 0 (division entière)
            harvestBarFill.fillAmount = harvestGoal > 0 ? (float)harvestCount / harvestGoal : 0f;
        }

        if (hasShownOnce && animator != null)
        {
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

    // Affiche les légumes perdus, en rouge quand on approche de la limite
    public void ShowLost(int lost, int max)
    {
        if (lostText == null) return;

        lostText.text = $"{lost} / {max}";
        lostText.color = lost >= max * 0.75f
            ? new Color(1f, 0.42f, 0.35f)    // rouge : danger !
            : new Color(1f, 0.96f, 0.88f);   // crème
    }
}