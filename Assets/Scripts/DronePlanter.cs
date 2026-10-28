using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Fait apparaître une plantation juste sous le drone quand le joueur appuie sur "Plant".
public class DronePlanter : MonoBehaviour
{
    [Header("Plantation")]
    [SerializeField] private GameObject plantPrefab;   // le moule à copier
    [SerializeField] private Transform plantsParent;   // l'objet "Plants" pour ranger la Hierarchy
    [SerializeField] private float groundHeight = 0f;  // hauteur du sol

    [Header("Input")]
    [SerializeField] private InputActionReference plantAction; // Gameplay/Plant

    // Liste de toutes les plantations créées (une variable simple n'en garderait qu'une !)
    private List<GameObject> plantedPlants = new List<GameObject>();

    private void OnEnable()
    {
        plantAction.action.Enable();
    }

    private void OnDisable()
    {
        plantAction.action.Disable();
    }

    private void Update()
    {
        // Vrai uniquement pendant l'image où le bouton vient d'être enfoncé
        if (plantAction.action.WasPressedThisFrame())
        {
            PlantSeed();
        }
    }

    private void PlantSeed()
    {
        // Sécurité : si on a oublié d'assigner le Prefab, on prévient au lieu de faire planter le jeu
        if (plantPrefab == null)
        {
            Debug.LogError("DronePlanter : aucun Prefab de plantation assigné dans l'Inspector !");
            return;
        }

        // On garde le X et le Z du drone, mais on descend au niveau du sol
        Vector3 spawnPosition = new Vector3(transform.position.x, groundHeight, transform.position.z);

        // Création de la copie, rangée dans l'objet "Plants"
        GameObject newPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity, plantsParent);

        plantedPlants.Add(newPlant);
    }
}