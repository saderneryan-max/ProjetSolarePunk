using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Garde le score et le nombre de récoltes, et termine la partie quand l'objectif est atteint.
public class ScoreManager : MonoBehaviour
{
    [Header("Objectif")]
    [SerializeField] private int harvestGoal = 15;               // nombre de légumes à récolter
    [SerializeField] private float delayBeforeEndScreen = 1f;    // petite pause avant l'écran de fin
    [SerializeField] private string endSceneName = "EndScene";

    [Header("Affichage")]
    [SerializeField] private ScoreDisplay scoreDisplay;

    public int Score { get; private set; }
    public int HarvestCount { get; private set; }

    private bool isGameOver = false; // empêche de compter des points après la fin

    private void Start()
    {
        Score = 0;
        HarvestCount = 0;
        RefreshDisplay();
    }

    // Appelée par une plante récoltée
    public void RegisterHarvest(int points)
    {
        if (isGameOver) return;

        Score += points;
        HarvestCount++;
        RefreshDisplay();

        if (HarvestCount >= harvestGoal)
        {
            StartCoroutine(EndGameAfterDelay());
        }
    }

    // Appelée par une plante qui éclate
    public void RegisterBurst(int penalty)
    {
        if (isGameOver) return;

        // Mathf.Max garde la plus grande des deux valeurs : le score ne descend jamais sous 0
        Score = Mathf.Max(0, Score - penalty);
        RefreshDisplay();
    }

    // Coroutine : une fonction qui peut faire une "pause" sans bloquer le jeu
    private IEnumerator EndGameAfterDelay()
    {
        isGameOver = true;
        GameSession.LastScore = Score; // on range le score dans la boîte partagée

        yield return new WaitForSeconds(delayBeforeEndScreen); // on attend 1 s...

        SceneManager.LoadScene(endSceneName); // ...puis on passe à l'écran de fin
    }

    private void RefreshDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.ShowScore(Score, HarvestCount, harvestGoal);
        }
    }
}