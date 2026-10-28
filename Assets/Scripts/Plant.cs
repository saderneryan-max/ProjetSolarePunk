using UnityEngine;

// Les 3 états possibles d'une plantation (une seule valeur à la fois)
public enum PlantState
{
    Seed,      // graine : pas encore récoltable
    Ripe,      // légume mûr : récoltable !
    Overripe   // trop mûr : va bientôt éclater
}

// Gère la vie d'une plantation : croissance, arrosage, récolte et éclatement.
public class Plant : MonoBehaviour
{
    [Header("Durée de chaque étape (en secondes)")]
    [SerializeField] private float seedDuration = 8f;
    [SerializeField] private float ripeDuration = 6f;
    [SerializeField] private float overripeDuration = 4f;

    [Header("Apparence de chaque étape")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private GameObject seedModelPrefab;
    [SerializeField] private GameObject ripeModelPrefab;
    [SerializeField] private GameObject overripeModelPrefab;

    [Header("Arrosage")]
    [SerializeField] private float wateredGrowthMultiplier = 2f;
    [SerializeField] private float wetDuration = 3f;

    [Header("Récolte")]                                   // NOUVEAU
    [SerializeField] private int pointsPerHarvest = 1;    // NOUVEAU

    public PlantState CurrentState { get; private set; }

    private float stageTimer = 0f;
    private float growthMultiplier = 1f;
    private float wetTimer = 0f;
    private GameObject currentModel;

    private ScoreManager scoreManager;   // NOUVEAU : donné par le DronePlanter à la naissance
    private bool isFinished = false;     // NOUVEAU : déjà récoltée ou éclatée ?

    // NOUVEAU : injection de dépendances, appelée juste après l'Instantiate
    public void Setup(ScoreManager manager)
    {
        scoreManager = manager;
    }

    private void Start()
    {
        ChangeState(PlantState.Seed);
    }

    private void Update()
    {
        if (isFinished) return; // NOUVEAU : une plante récoltée ne pousse plus

        UpdateWetness();

        stageTimer += Time.deltaTime * growthMultiplier;

        if (stageTimer >= GetCurrentStageDuration())
        {
            GoToNextStage();
        }
    }

    // ---------- DÉTECTION DU DRONE (NOUVEAU) ----------
    private void OnTriggerEnter(Collider other)
    {
        // "other" est le Collider qui entre : on vérifie que c'est bien le drone
        if (other.GetComponent<DroneMovement>() != null)
        {
            TryHarvest(); // ne récolte que si la plante est mûre (vérifié dans TryHarvest)
        }
    }

    // Récolte la plante SEULEMENT si elle est mûre
    private void TryHarvest()
    {
        if (isFinished || CurrentState != PlantState.Ripe)
        {
            return;
        }

        isFinished = true;

        if (scoreManager != null)
        {
            scoreManager.AddPoints(pointsPerHarvest);
        }
        else
        {
            Debug.LogWarning("Plant : aucun ScoreManager reçu, le point n'est pas compté !");
        }

        // Plus tard (brique 4.2) : son + particules de récolte ici
        Destroy(gameObject);
    }

    // ---------- ARROSAGE ----------

    public void Water()
    {
        wetTimer = wetDuration;
    }

    private void UpdateWetness()
    {
        if (wetTimer > 0f)
        {
            wetTimer -= Time.deltaTime;
            growthMultiplier = wateredGrowthMultiplier;
        }
        else
        {
            growthMultiplier = 1f;
        }
    }

    // ---------- CROISSANCE ----------

    private float GetCurrentStageDuration()
    {
        switch (CurrentState)
        {
            case PlantState.Seed:
                return seedDuration;
            case PlantState.Ripe:
                return ripeDuration;
            default:
                return overripeDuration;
        }
    }

    private void GoToNextStage()
    {
        switch (CurrentState)
        {
            case PlantState.Seed:
                ChangeState(PlantState.Ripe);
                break;
            case PlantState.Ripe:
                ChangeState(PlantState.Overripe);
                break;
            case PlantState.Overripe:
                Burst();
                break;
        }
    }

    private void ChangeState(PlantState newState)
    {
        CurrentState = newState;
        stageTimer = 0f;

        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        GameObject modelPrefab = GetModelPrefab(newState);
        if (modelPrefab != null)
        {
            currentModel = Instantiate(modelPrefab, visualRoot);
        }
        else
        {
            Debug.LogWarning($"Plant : aucun modèle assigné pour l'état {newState}");
        }
    }

    private GameObject GetModelPrefab(PlantState state)
    {
        switch (state)
        {
            case PlantState.Seed:
                return seedModelPrefab;
            case PlantState.Ripe:
                return ripeModelPrefab;
            default:
                return overripeModelPrefab;
        }
    }

    private void Burst()
    {
        if (isFinished) return; // NOUVEAU : sécurité contre un double appel
        isFinished = true;

        // Plus tard (brique 4.2) : son + particules d'éclatement ici
        Destroy(gameObject);
    }
}