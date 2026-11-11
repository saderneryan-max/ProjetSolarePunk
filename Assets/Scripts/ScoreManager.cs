using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Garde le score de récolte, affiche le remplissage de la caisse, et termine la partie.
public class ScoreManager : MonoBehaviour
{
    [Header("Fin de partie")]
    [SerializeField] private float delayBeforeEndScreen = 1.5f;
    [SerializeField] private string endSceneName = "EndScene";

    [Header("Défaite")]
    [SerializeField] private int maxLostPlants = 12;   // légumes éclatés autorisés

    public int LostPlants { get; private set; }

    [Header("Affichage")]
    [SerializeField] private ScoreDisplay scoreDisplay;

    public int Score { get; private set; }

    private int deliveredCount = 0;
    private int deliveryCapacity = 15;
    private bool isGameOver = false;

    private void Start()
    {
        RefreshDisplay();
    }

    public void RegisterHarvest(int points)
    {
        if (isGameOver) return;
        Score += points;
        RefreshDisplay();
    }

    public void RegisterBurst(int penalty)
    {
        if (isGameOver) return;

        Score = Mathf.Max(0, Score - penalty);
        LostPlants++;
        RefreshDisplay();

        if (LostPlants >= maxLostPlants)
        {
            EndGame(false, 0, "Trop de récoltes perdues... Vous êtes un mauvais jardinier !");
        }
    }

    // Appelée par la caisse à chaque dépôt / retrait
    public void SetDeliveryProgress(int count, int capacity)
    {
        deliveredCount = count;
        deliveryCapacity = capacity;
        RefreshDisplay();
    }

    // Termine la partie : victoire (caisse expédiée) ou défaite (partie 2)
    public void EndGame(bool victory, int deliveryBonus, string message)
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(EndGameAfterDelay(victory, deliveryBonus, message));
    }

    private IEnumerator EndGameAfterDelay(bool victory, int deliveryBonus, string message)
    {
        int finalScore = Mathf.Max(0, Score + deliveryBonus);

        GameSession.HarvestScore = Score;
        GameSession.DeliveryBonus = deliveryBonus;
        GameSession.LastScore = finalScore;
        GameSession.IsVictory = victory;
        GameSession.EndMessage = message;
        // Seule une victoire peut battre le record
        GameSession.IsNewRecord = victory && BestScoreSaver.TrySave(finalScore);

        yield return new WaitForSeconds(delayBeforeEndScreen);
        SceneManager.LoadScene(endSceneName);
    }

    private void RefreshDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.ShowScore(Score, deliveredCount, deliveryCapacity);
            scoreDisplay.ShowLost(LostPlants, maxLostPlants);
        }
    }
}