using UnityEngine;
using UnityEngine.SceneManagement;

// Petit outil appelé par les boutons de l'interface pour changer de scène ou quitter le jeu.
public class SceneLoader : MonoBehaviour
{
    // Charge la scène dont on donne le nom (ex : "GameScene")
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // sécurité : remet le temps en marche (utile après le menu pause)
        AudioListener.pause = false; // sécurité : on quitte peut-être la scène depuis le menu pause
        SceneManager.LoadScene(sceneName);
    }

    // Ferme le jeu
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Dans l'éditeur, Application.Quit() ne fait rien : on arrête le Play Mode à la place
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}