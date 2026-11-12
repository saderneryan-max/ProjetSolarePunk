using TMPro;
using UnityEngine;

// Invite d'interaction ("🖱️ / B  Marché") qui suit un objet 3D à l'écran.
// À placer sur HUD (toujours actif) ; "promptRoot" est l'objet affiché/caché.
public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private RectTransform promptRoot;
    [SerializeField] private TMP_Text label;

    private Interactable target;
    private Camera mainCamera;

    private void Start()
    {
        if (target == null) promptRoot.gameObject.SetActive(false);
    }

    public void Show(Interactable interactable)
    {
        target = interactable;
        label.text = interactable.PromptText;
        promptRoot.gameObject.SetActive(true); // réactiver relance l'animation d'apparition
    }

    public void Hide()
    {
        target = null;

        // Sécurité : à la fermeture de la scène, l'invite peut déjà avoir été détruite
        if (promptRoot != null)
        {
            promptRoot.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (mainCamera == null) mainCamera = Camera.main;

        // Position 3D de l'objet -> position en pixels sur l'écran
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.PromptPosition);

        // z < 0 : l'objet est DERRIÈRE la caméra, on n'affiche pas l'invite
        promptRoot.gameObject.SetActive(screenPos.z > 0f);
        promptRoot.position = screenPos;
    }
}