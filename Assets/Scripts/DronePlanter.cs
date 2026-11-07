using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Plante sous le drone le légume correspondant à la zone survolée,
// et lui transmet le ScoreManager dès sa naissance (injection de dépendances).
public class DronePlanter : MonoBehaviour
{
    [Header("Plantation")]
    [SerializeField] private Transform plantsParent;

    [Header("Dépendances")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GroundSensor groundSensor;
    [SerializeField] private ZoneDetector zoneDetector;

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
        // Quel légume pousse ici ?
        Plant plantPrefab = zoneDetector.GetPlantToPlant(out string zoneName);
        if (plantPrefab == null)
        {
            return; // zone où rien ne pousse
        }

        // Où est le sol ?
        if (!groundSensor.TryGetGroundPoint(transform.position, out Vector3 spawnPosition))
        {
            return;
        }

        Plant newPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity, plantsParent);
        newPlant.Setup(scoreManager, inventory); // injection : la plante reçoit tout ce dont elle a besoin
        plantedPlants.Add(newPlant);
    }
}