using TMPro;
using UnityEngine;

// Affiche le score et la progression vers l'objectif.
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    public void ShowScore(int score, int harvestCount, int harvestGoal)
    {
        // $"..." permet d'insérer des variables entre { } ; \n = retour à la ligne
        scoreText.text = $"Score : {score}\nRécoltes : {harvestCount} / {harvestGoal}";
    }
}