using UnityEngine;

// Les 3 états possibles d'une plantation (une seule valeur à la fois)
public enum PlantState
{
    Seed,      // en croissance : pas encore récoltable
    Ripe,      // légume mûr : récoltable !
    Overripe   // trop mûr : tremble, puis éclate
}

// Gère la vie d'une plantation : croissance par stades (animée), arrosage, récolte et éclatement.
public class Plant : MonoBehaviour
{
    [Header("Identité")]
    [SerializeField] private string displayName = "Carotte";

    [Header("Durée de chaque état (en secondes)")]
    [SerializeField] private float seedDuration = 9f;      // temps total de pousse (tous les stades)
    [SerializeField] private float ripeDuration = 6f;
    [SerializeField] private float overripeDuration = 4f;

    [Header("Apparence")]
    [SerializeField] private Transform modelRoot;              // l'enfant ModelRoot : les modèles y apparaissent
    [SerializeField] private GameObject[] growthStageModels;   // stades de pousse, dans l'ordre (1, 2, 3...)
    [SerializeField] private GameObject ripeModelPrefab;       // légume mûr
    [SerializeField] private GameObject overripeModelPrefab;   // légume trop mûr

    [Header("Animation")]
    [SerializeField] private Animator visualAnimator;          // l'Animator posé sur Visual
    [SerializeField] private string popStateName = "Plant_Pop";
    [SerializeField] private string trembleStateName = "Plant_Tremble";

    [Header("Arrosage")]
    [SerializeField] private float wateredGrowthMultiplier = 2f;
    [SerializeField] private float wetDuration = 3f;

    [Header("Récolte")]
    [SerializeField] private int pointsPerHarvest = 1;
    [SerializeField] private int burstPenalty = 1;

    [Header("Effets (son + particules)")]
    [SerializeField] private GameObject plantEffectPrefab;
    [SerializeField] private GameObject harvestEffectPrefab;
    [SerializeField] private GameObject burstEffectPrefab;
    [SerializeField] private float effectHeight = 0.5f;   // hauteur de l'effet au-dessus du sol

    public string DisplayName { get { return displayName; } }
    public PlantState CurrentState { get; private set; }

    private float stageTimer = 0f;
    private float growthMultiplier = 1f;
    private float wetTimer = 0f;
    private int currentGrowthStage = -1;   // -1 = aucun stade affiché pour l'instant
    private GameObject currentModel;

    private ScoreManager scoreManager;     // donné par le DronePlanter à la naissance
    private bool isFinished = false;       // déjà récoltée ou éclatée ?

    // Injection de dépendances : appelée juste après l'Instantiate
    public void Setup(ScoreManager manager)
    {
        scoreManager = manager;
    }

    private void Start()
    {
        SpawnEffect(plantEffectPrefab);
        ChangeState(PlantState.Seed);
    }

    private void Update()
    {
        if (isFinished) return;

        UpdateWetness();
        stageTimer += Time.deltaTime * growthMultiplier;

        if (CurrentState == PlantState.Seed)
        {
            UpdateGrowthStage();
        }

        if (stageTimer >= GetCurrentStageDuration())
        {
            GoToNextStage();
        }
    }

    // ---------- CROISSANCE PAR STADES ----------

    // Affiche le bon stade de pousse selon l'avancement du chrono
    private void UpdateGrowthStage()
    {
        if (growthStageModels == null || growthStageModels.Length == 0) return;

        // Avancement de 0 à 1, transformé en numéro de stade (0, 1, 2...)
        float progress = stageTimer / seedDuration;
        int stage = Mathf.Min(growthStageModels.Length - 1, (int)(progress * growthStageModels.Length));

        if (stage != currentGrowthStage)
        {
            currentGrowthStage = stage;
            ShowModel(growthStageModels[stage]);
            PlayAnimation(popStateName);
        }
    }

    private void ChangeState(PlantState newState)
    {
        CurrentState = newState;
        stageTimer = 0f;

        switch (newState)
        {
            case PlantState.Seed:
                currentGrowthStage = -1;
                UpdateGrowthStage(); // affiche tout de suite le 1er stade
                break;
            case PlantState.Ripe:
                ShowModel(ripeModelPrefab);
                PlayAnimation(popStateName);
                break;
            case PlantState.Overripe:
                ShowModel(overripeModelPrefab);
                PlayAnimation(trembleStateName); // signal d'alerte : il va éclater !
                break;
        }
    }

    // Remplace le modèle affiché par un nouveau
    private void ShowModel(GameObject modelPrefab)
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        if (modelPrefab != null)
        {
            currentModel = Instantiate(modelPrefab, modelRoot);
        }
        else
        {
            Debug.LogWarning($"{displayName} : un modèle n'est pas assigné pour l'état {CurrentState}");
        }
    }

    // Crée un effet à l'emplacement de la plante.
    // Il n'est PAS enfant de la plante : il survit donc à sa destruction.
    private void SpawnEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null) return;

        Vector3 position = transform.position + Vector3.up * effectHeight;
        Instantiate(effectPrefab, position, Quaternion.identity);
    }

    // Relance une animation depuis son début (0f = au tout début du clip)
    private void PlayAnimation(string stateName)
    {
        if (visualAnimator != null)
        {
            visualAnimator.Play(stateName, 0, 0f);
        }
    }

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

    // ---------- RÉCOLTE ----------

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DroneMovement>() != null)
        {
            TryHarvest(); // ne récolte que si la plante est mûre
        }
    }

    private void TryHarvest()
    {
        if (isFinished || CurrentState != PlantState.Ripe) return;

        isFinished = true;

        if (scoreManager != null)
        {
            scoreManager.RegisterHarvest(pointsPerHarvest);
        }

        // Plus tard (brique 4.2) : son + particules de récolte ici
        SpawnEffect(harvestEffectPrefab);
        Destroy(gameObject);
    }

    private void Burst()
    {
        if (isFinished) return;
        isFinished = true;

        if (scoreManager != null)
        {
            scoreManager.RegisterBurst(burstPenalty);
        }

        // Plus tard (brique 4.2) : son + particules d'éclatement ici
        SpawnEffect(burstEffectPrefab);
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
}