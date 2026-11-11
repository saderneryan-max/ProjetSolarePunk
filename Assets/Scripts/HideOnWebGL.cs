using UnityEngine;

// Cache cet objet dans la version navigateur (WebGL), où "Quitter le jeu" n'a pas de sens.
// Il reste visible dans l'éditeur et dans une version PC.
public class HideOnWebGL : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }
}