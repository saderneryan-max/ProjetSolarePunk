using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Met le jeu en pause (Échap / Start), affiche le menu pause
// et désactive les contrôles du drone pendant ce temps.
public class PauseMenu : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private GameObject pausePanel;          // le panneau du menu pause
    [SerializeField] private GameObject firstSelectedButton; // bouton sélectionné à l'ouverture (manette)

    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction; // Gameplay/Pause

    [Header("Scripts à couper pendant la pause")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableDuringPause; // les scripts du drone

    private bool isPaused = false;

    private void OnEnable()
    {
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.Disable();
    }

    private void Start()
    {
        SetPaused(false); // la partie commence toujours sans pause
    }

    private void Update()
    {
        // Update continue d'être appelé même quand timeScale = 0 : on peut donc toujours sortir de la pause
        if (pauseAction.action.WasPressedThisFrame())
        {
            SetPaused(!isPaused); // ! inverse le booléen : pause <-> pas pause
        }
    }

    // Appelée par le bouton "Reprendre"
    public void Resume()
    {
        SetPaused(false);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        // Afficher ou cacher le menu
        pausePanel.SetActive(paused);

        // Geler ou relancer le temps
        if (paused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        // Couper ou rallumer les scripts du drone (enabled = false déclenche leur OnDisable)
        foreach (MonoBehaviour script in scriptsToDisableDuringPause)
        {
            if (script != null)
            {
                script.enabled = !paused;
            }
        }

        // Sélectionner le premier bouton pour que la manette puisse naviguer
        if (paused && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}