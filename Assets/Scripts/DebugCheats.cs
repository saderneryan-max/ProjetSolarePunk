using UnityEngine;
using UnityEngine.InputSystem;

// Raccourcis de test, actifs UNIQUEMENT dans l'éditeur Unity (jamais dans le jeu publié).
// F1 : +100 Punk   |   F2 : vitesse x4 (et retour à x1)
public class DebugCheats : MonoBehaviour
{
    [SerializeField] private Wallet wallet;

    private void Update()
    {
#if UNITY_EDITOR
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.f1Key.wasPressedThisFrame)
        {
            wallet.Add(100);
        }
        if (keyboard.f2Key.wasPressedThisFrame)
        {
            Time.timeScale = Time.timeScale > 1f ? 1f : 4f;
        }
#endif
    }
}