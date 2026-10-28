using System;
using UnityEngine;

// Les 3 états possibles d'une plantation (une seule valeur à la fois)
public enum PlantState
{
    Seed,      // graine : pas encore récoltable
    Ripe,      // légume mûr : récoltable !
    Overripe   // trop mûr : va bientôt éclater
}

// Gère la croissance d'une plantation : elle passe d'un état à l'autre avec le temps,
// change d'apparence à chaque étape, puis éclate si personne ne la récolte.
public class Plant : MonoBehaviour
{
    [Header("Durée de chaque étape (en secondes)")]
    [SerializeField] private float seedDuration = 8f;
    [SerializeField] private float ripeDuration = 6f;
    [SerializeField] private float overripeDuration = 4f;

    [Header("Apparence de chaque étape")]
    [SerializeField] private Transform visualRoot;          // l'enfant "Visual"
    [SerializeField] private GameObject seedModelPrefab;
    [SerializeField] private GameObject ripeModelPrefab;
    [SerializeField] private GameObject overripeModelPrefab;

    [Header("Arrosage")]
    [SerializeField] private float wateredGrowthMultiplier = 2f; // ×2 = pousse deux fois plus vite
    [SerializeField] private float wetDuration = 3f;            // reste mouillée 3 s après l'arrosage

    private float wetTimer = 0f; // temps restant avant que la plante sèche

    // Tout le monde peut LIRE l'état (la récolte en aura besoin),
    // mais seul ce script peut le MODIFIER.
    public PlantState CurrentState { get; private set; }

    private float stageTimer = 0f;          // chrono de l'étape en cours
    private float growthMultiplier = 1f;    // vitesse de pousse (servira à l'arrosage, brique 2.3)
    private GameObject currentModel;        // le modèle 3D actuellement affiché

    private void Start()
    {
        // Une plantation commence toujours en graine
        ChangeState(PlantState.Seed);
    }

    private void Update()
    {
        UpdateWetness();

        // On avance le chrono (plus vite si la plante est mouillée)
        stageTimer += Time.deltaTime * growthMultiplier;

        // Assez de temps écoulé ? On passe à l'étape suivante
        if (stageTimer >= GetCurrentStageDuration())
        {
            GoToNextStage();
        }
    }

    // Appelée par le drone quand il arrose cette plantation
    public void Water()
    {
        wetTimer = wetDuration; // on "recharge" l'humidité
    }

    // Fait sécher la plante petit à petit et règle sa vitesse de pousse
    private void UpdateWetness()
    {
        if (wetTimer > 0f)
        {
            wetTimer -= Time.deltaTime;
            growthMultiplier = wateredGrowthMultiplier; // mouillée : pousse plus vite
        }
        else
        {
            growthMultiplier = 1f; // sèche : vitesse normale
        }
    }

    // Renvoie la durée de l'étape dans laquelle on se trouve
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

    // Décide de ce qui vient après l'étape actuelle
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

    // Change l'état ET l'apparence de la plantation
    private void ChangeState(PlantState newState)
    {
        CurrentState = newState;
        stageTimer = 0f; // nouveau chrono pour la nouvelle étape

        // On détruit l'ancien modèle s'il y en a un...
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        // ...et on fait apparaître celui de la nouvelle étape, dans "Visual"
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

    // Renvoie le bon Prefab de modèle selon l'état
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

    // Le légume a trop attendu : il éclate et disparaît
    private void Burst()
    {
        // Plus tard (brique 4.2) : son + particules d'éclatement ici
        Destroy(gameObject); // supprime toute la plantation de la scène ET de la Hierarchy
    }
}