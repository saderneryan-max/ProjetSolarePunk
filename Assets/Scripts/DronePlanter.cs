using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Fait apparaître une plantation juste sous le drone quand le joueur appuie sur "Plant",
// et lui transmet le ScoreManager dès sa naissance (injection de dépendances).
public class DronePlanter : MonoBehaviour
{
    [Header("Plantation")]
    [SerializeField] private Plant plantPrefab;        // CHANGÉ : type Plant au lieu de GameObject
    [SerializeField] private Transform plantsParent;
    [SerializeField] private float groundHeight = 0f;

    [Header("Dépendances")]
    [SerializeField] private ScoreManager scoreManager; // NOUVEAU : à transmettre aux plantes

    [Header("Input")]
    [SerializeField] private InputActionReference plantAction;

    private List<Plant> plantedPlants = new List<Plant>();

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
        if (plantAction.action.WasPressedThisFrame())
        {
            PlantSeed();
        }
    }

    private void PlantSeed()
    {
        if (plantPrefab == null)
        {
            Debug.LogError("DronePlanter : aucun Prefab de plantation assigné dans l'Inspector !");
            return;
        }

        Vector3 spawnPosition = new Vector3(transform.position.x, groundHeight, transform.position.z);

        // Comme plantPrefab est de type Plant, Instantiate renvoie directement le script Plant
        Plant newPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity, plantsParent);

        // INJECTION : on donne à la plante ce dont elle a besoin, au moment où elle naît
        newPlant.Setup(scoreManager);

        plantedPlants.Add(newPlant);
    }
}