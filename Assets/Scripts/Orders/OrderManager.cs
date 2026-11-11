using System;
using System.Collections.Generic;
using UnityEngine;

// Propose 3 commandes au début (une par difficulté), garde la commande choisie,
// et mesure le temps de jeu pour le multiplicateur de score.
public class OrderManager : MonoBehaviour
{
    [SerializeField] private OrderData[] allOrders;   // les 15 commandes

    [Header("Multiplicateur de temps (en minutes)")]
    [SerializeField] private float fastTime = 10f;
    [SerializeField] private float mediumTime = 20f;
    [SerializeField] private float fastMultiplier = 2f;
    [SerializeField] private float mediumMultiplier = 1.5f;

    public OrderData CurrentOrder { get; private set; }
    public List<OrderData> Offers { get; } = new List<OrderData>();
    public float ElapsedSeconds { get; private set; }

    // Multiplicateur par paliers : x2 avant 10 min, x1.5 avant 20 min, x1 ensuite
    public float TimeMultiplier
    {
        get
        {
            float minutes = ElapsedSeconds / 60f;
            if (minutes < fastTime) return fastMultiplier;
            if (minutes < mediumTime) return mediumMultiplier;
            return 1f;
        }
    }

    public event Action OnOrderChosen;

    private void Awake()
    {
        PickOffers();
    }

    private void Update()
    {
        ElapsedSeconds += Time.deltaTime; // s'arrête tout seul pendant la pause (timeScale = 0)
    }

    // Tire au sort une commande de chaque difficulté
    private void PickOffers()
    {
        foreach (OrderDifficulty difficulty in Enum.GetValues(typeof(OrderDifficulty)))
        {
            List<OrderData> pool = new List<OrderData>();
            foreach (OrderData order in allOrders)
            {
                if (order.Difficulty == difficulty) pool.Add(order);
            }

            if (pool.Count > 0)
            {
                // UnityEngine.Random en entier : "System" a aussi une classe Random, il faut préciser
                Offers.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
            }
        }
    }

    public void ChooseOrder(OrderData order)
    {
        if (CurrentOrder != null) return; // une seule commande par partie
        CurrentOrder = order;
        OnOrderChosen?.Invoke();
    }
}