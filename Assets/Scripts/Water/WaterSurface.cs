using UnityEngine;

// Fait onduler la surface de l'eau en faisant glisser lentement sa texture.
[RequireComponent(typeof(Renderer))]
public class WaterSurface : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.02f, 0.01f); // vitesse du courant

    private Material material;
    private Vector2 offset;

    private void Start()
    {
        // .material crée une copie propre à cette mare : chaque mare peut avoir son propre courant
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        offset += scrollSpeed * Time.deltaTime;

        // Mathf.Repeat garde la valeur entre 0 et 1 (la texture se répète, ça ne se voit pas)
        offset.x = Mathf.Repeat(offset.x, 1f);
        offset.y = Mathf.Repeat(offset.y, 1f);

        material.mainTextureOffset = offset;
    }

    // La copie du matériau n'est pas détruite automatiquement : on fait le ménage nous-mêmes
    private void OnDestroy()
    {
        if (material != null)
        {
            Destroy(material);
        }
    }
}