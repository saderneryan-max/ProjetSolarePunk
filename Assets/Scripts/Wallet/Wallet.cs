using System;
using UnityEngine;

// Porte-monnaie du joueur : les "Punk", la monnaie du jeu.
public class Wallet : MonoBehaviour
{
    [SerializeField] private int startingPunk = 5;

    public int Punk { get; private set; }

    public event Action<int> OnChanged; // envoie le nouveau montant à ceux qui écoutent

    private void Awake()
    {
        Punk = startingPunk;
    }

    private void Start()
    {
        OnChanged?.Invoke(Punk); // premier affichage
    }

    public void Add(int amount)
    {
        Punk += amount;
        OnChanged?.Invoke(Punk);
    }

    // Dépense seulement si on a assez d'argent (tout ou rien, comme l'inventaire)
    public bool TrySpend(int amount)
    {
        if (Punk < amount) return false;

        Punk -= amount;
        OnChanged?.Invoke(Punk);
        return true;
    }
}