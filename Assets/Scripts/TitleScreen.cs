using TMPro;
using UnityEngine;

// Affiche le meilleur score sur l'écran titre.
public class TitleScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text bestScoreText;

    private void Start()
    {
        bestScoreText.text = $"Meilleur score : {BestScoreSaver.Load()}";
    }
}