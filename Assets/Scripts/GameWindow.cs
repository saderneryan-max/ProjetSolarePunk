using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Classe de BASE des fenêtres de jeu (marché, caisse de livraison) :
// ouvrir / fermer, figer le drone pendant ce temps, fermer avec Échap / B.
// Chaque fenêtre qui en hérite n'a plus qu'à écrire son propre affichage (Refresh).
public abstract class GameWindow : MonoBehaviour
{
    [Header("Fenêtre")]
    [SerializeField] protected GameObject panel;
    [SerializeField] protected Button closeButton;
    [SerializeField] private InputActionReference cancelAction;     // Gameplay/Cancel
    [SerializeField] private MonoBehaviour[] scriptsToDisable;      // drone, caméra, pause, inventaire...

    private int openedFrame = -1;

    public bool IsOpen => panel.activeSelf;

    // "protected virtual" : les classes enfants peuvent compléter cette méthode
    protected virtual void Start()
    {
        closeButton.onClick.AddListener(Close);
        panel.SetActive(false);
    }

    protected virtual void Update()
    {
        // On ignore l'image d'ouverture : le B qui OUVRE ne doit pas aussi FERMER
        if (IsOpen && Time.frameCount != openedFrame && cancelAction.action.WasPressedThisFrame())
        {
            Close();
        }
    }

    protected void OpenWindow()
    {
        if (IsOpen) return;

        openedFrame = Time.frameCount;
        SetScriptsEnabled(false);
        cancelAction.action.Enable();
        panel.SetActive(true);

        OnOpened();
        Refresh();
    }

    public void Close()
    {
        if (!IsOpen) return;

        OnClosed();
        cancelAction.action.Disable();
        panel.SetActive(false);
        SetScriptsEnabled(true);
    }

    // Points d'accroche pour les enfants (abonnements aux événements, etc.)
    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }

    // Chaque fenêtre DOIT écrire son propre affichage
    protected abstract void Refresh();

    // Manette : si le bouton sélectionné a été détruit (liste reconstruite), on en sélectionne un autre
    protected void EnsureSelection(GameObject preferred, params Transform[] rebuiltLists)
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        bool lost = selected == null || !selected.activeInHierarchy;

        foreach (Transform list in rebuiltLists)
        {
            if (selected != null && selected.transform.IsChildOf(list)) lost = true;
        }

        if (lost) EventSystem.current.SetSelectedGameObject(preferred);
    }

    // Petit outil : vide une liste d'interface
    protected static void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent) Destroy(child.gameObject);
    }

    private void SetScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null) script.enabled = value;
        }
    }
}