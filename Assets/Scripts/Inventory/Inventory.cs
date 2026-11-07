using System;
using System.Collections.Generic;
using UnityEngine;

// Inventaire du joueur : récoltes et graines, rangées en piles, avec un nombre de cases limité.
public class Inventory : MonoBehaviour
{
    [Header("Capacité")]
    [SerializeField] private int maxSlots = 8;
    [SerializeField] private int maxHarvestStack = 10;   // récoltes par case
    [SerializeField] private int maxSeedStack = 30;      // graines par case

    [Header("Départ de partie")]
    [SerializeField] private CropData startingSeedCrop;  // la carotte
    [SerializeField] private int startingSeedCount = 10;

    private readonly List<InventorySlot> slots = new List<InventorySlot>();

    // Les autres scripts peuvent LIRE les cases, mais pas les modifier directement
    public IReadOnlyList<InventorySlot> Slots => slots;
    public int MaxSlots => maxSlots;

    // Événement : "préviens-moi quand l'inventaire change" (l'interface s'y abonne)
    public event Action OnChanged;

    private void Awake()
    {
        if (startingSeedCrop != null)
        {
            TryAdd(startingSeedCrop, ItemKind.Seed, CropQuality.Bronze, startingSeedCount);
        }
    }

    // Ajoute des objets en remplissant d'abord les piles existantes, puis de nouvelles cases.
    // Renvoie false (et n'ajoute RIEN) s'il n'y a pas assez de place.
    public bool TryAdd(CropData crop, ItemKind kind, CropQuality quality, int amount = 1)
    {
        if (GetFreeSpaceFor(crop, kind, quality) < amount) return false;

        int maxStack = GetMaxStack(kind);
        int remaining = amount;

        // 1. On complète les piles identiques qui ne sont pas pleines
        foreach (InventorySlot slot in slots)
        {
            if (remaining == 0) break;
            if (!slot.CanStackWith(crop, kind, quality)) continue;

            int added = Mathf.Min(maxStack - slot.Count, remaining);
            slot.Count += added;
            remaining -= added;
        }

        // 2. Ce qui reste va dans de nouvelles cases
        while (remaining > 0)
        {
            int added = Mathf.Min(maxStack, remaining);
            slots.Add(new InventorySlot(crop, kind, quality, added));
            remaining -= added;
        }

        OnChanged?.Invoke(); // "?." : on prévient seulement si quelqu'un écoute
        return true;
    }

    // Retire des objets. Renvoie false (et ne retire RIEN) s'il n'y en a pas assez.
    public bool TryRemove(CropData crop, ItemKind kind, CropQuality quality, int amount = 1)
    {
        if (CountOf(crop, kind, quality) < amount) return false;

        int remaining = amount;

        // On retire en partant de la fin, pour vider d'abord les dernières piles
        for (int i = slots.Count - 1; i >= 0 && remaining > 0; i--)
        {
            InventorySlot slot = slots[i];
            if (!slot.CanStackWith(crop, kind, quality)) continue;

            int removed = Mathf.Min(slot.Count, remaining);
            slot.Count -= removed;
            remaining -= removed;

            if (slot.Count == 0)
            {
                slots.RemoveAt(i); // une case vide libère sa place
            }
        }

        OnChanged?.Invoke();
        return true;
    }

    // Combien d'objets identiques possède-t-on, toutes piles confondues ?
    public int CountOf(CropData crop, ItemKind kind, CropQuality quality)
    {
        int total = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot.CanStackWith(crop, kind, quality)) total += slot.Count;
        }
        return total;
    }

    // Combien d'objets de ce type peut-on encore ranger ?
    private int GetFreeSpaceFor(CropData crop, ItemKind kind, CropQuality quality)
    {
        int maxStack = GetMaxStack(kind);
        int space = (maxSlots - slots.Count) * maxStack; // les cases vides

        foreach (InventorySlot slot in slots)
        {
            if (slot.CanStackWith(crop, kind, quality)) space += maxStack - slot.Count; // la place dans les piles
        }
        return space;
    }

    private int GetMaxStack(ItemKind kind)
    {
        return kind == ItemKind.Seed ? maxSeedStack : maxHarvestStack;
    }
}