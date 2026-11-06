using UnityEngine;
using UnityEngine.InputSystem;

// Affiche l'aide des commandes du périphérique que le joueur utilise en ce moment :
// les touches du clavier, ou les boutons de la manette.
public class ControlsHintSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject keyboardHints;
    [SerializeField] private GameObject gamepadHints;
    [SerializeField] private float stickDeadZone = 0.2f;  // ignore les tout petits mouvements du stick

    private bool? isShowingGamepad = null; // null = rien d'affiché pour l'instant

    private void Start()
    {
        // Au départ : la manette si elle est branchée, sinon le clavier
        Show(Gamepad.current != null);
    }

    private void Update()
    {
        if (GamepadWasUsed())
        {
            Show(true);
        }
        else if (KeyboardOrMouseWasUsed())
        {
            Show(false);
        }
    }

    private bool GamepadWasUsed()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return false;

        return gamepad.buttonSouth.wasPressedThisFrame
            || gamepad.buttonWest.wasPressedThisFrame
            || gamepad.startButton.wasPressedThisFrame
            || gamepad.leftStick.ReadValue().magnitude > stickDeadZone
            || gamepad.rightStick.ReadValue().magnitude > stickDeadZone;
    }

    private bool KeyboardOrMouseWasUsed()
    {
        bool keyboardUsed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool mouseUsed = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 1f;
        return keyboardUsed || mouseUsed;
    }

    private void Show(bool gamepad)
    {
        if (isShowingGamepad == gamepad) return; // déjà le bon affichage : rien à faire

        isShowingGamepad = gamepad;
        keyboardHints.SetActive(!gamepad);
        gamepadHints.SetActive(gamepad);
    }
}