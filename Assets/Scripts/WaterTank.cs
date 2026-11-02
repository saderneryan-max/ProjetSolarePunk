using UnityEngine;

// Réservoir d'eau du drone : se vide pendant l'arrosage, se remplit au-dessus d'une mare.
public class WaterTank : MonoBehaviour
{
    [Header("Capacité")]
    [SerializeField] private float capacity = 100f;
    [SerializeField] private float consumptionPerSecond = 20f; // 5 s d'arrosage avec un réservoir plein
    [SerializeField] private float refillPerSecond = 50f;      // 2 s pour un remplissage complet

    [Header("Références")]
    [SerializeField] private ZoneDetector zoneDetector;
    [SerializeField] private WaterTankDisplay display;

    public float CurrentWater { get; private set; }
    public bool HasWater { get { return CurrentWater > 0f; } }
    public bool IsRefilling { get; private set; }

    private void Start()
    {
        CurrentWater = capacity; // la partie commence avec un réservoir plein
    }

    private void Update()
    {
        // On remplit seulement au-dessus d'une mare, et s'il reste de la place
        IsRefilling = zoneDetector.IsAboveWater() && CurrentWater < capacity;

        if (IsRefilling)
        {
            CurrentWater = Mathf.Min(capacity, CurrentWater + refillPerSecond * Time.deltaTime);
        }

        RefreshDisplay();
    }

    // Appelée par le DroneWaterer à chaque image d'arrosage
    public void UseWater(float deltaTime)
    {
        // Mathf.Max : le réservoir ne descend jamais sous 0
        CurrentWater = Mathf.Max(0f, CurrentWater - consumptionPerSecond * deltaTime);
    }

    private void RefreshDisplay()
    {
        if (display != null)
        {
            display.Show(CurrentWater / capacity, IsRefilling);
        }
    }
}