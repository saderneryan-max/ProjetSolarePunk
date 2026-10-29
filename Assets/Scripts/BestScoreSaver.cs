using UnityEngine;

// Lit et écrit le meilleur score sur le disque avec PlayerPrefs,
// pour qu'il soit conservé même après la fermeture du jeu.
public static class BestScoreSaver
{
    // La clé est écrite à UN SEUL endroit : impossible de faire une faute de frappe ailleurs
    private const string BestScoreKey = "BestScore";

    // Renvoie le meilleur score enregistré (0 au tout premier lancement)
    public static int Load()
    {
        return PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    // Enregistre le score SEULEMENT s'il bat le record.
    // Renvoie true si c'est un nouveau record.
    public static bool TrySave(int score)
    {
        if (score <= Load())
        {
            return false; // pas mieux : on ne touche à rien, le record ne baisse jamais
        }

        PlayerPrefs.SetInt(BestScoreKey, score);
        PlayerPrefs.Save(); // force l'écriture sur le disque tout de suite
        return true;
    }
}