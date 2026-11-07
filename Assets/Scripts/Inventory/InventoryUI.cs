using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Ouvre et ferme l'inventaire (Tab / Y) et affiche son contenu sous forme de cases.
// À placer sur le Canvas (PAS sur InventoryPanel : un objet désactivé n'exécute pas son Update).
public class InventoryUI : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private Inventory inventory;

    [Header("Interface")]
    [SerializeField] private GameObject panel;              // InventoryPanel
    [SerializeField] private Transform slotsContainer;      // SlotsGrid
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private TMP_Text capacityText;

    [Header("Input")]
    [SerializeField] private InputActionReference inventoryAction;  // Gameplay/Inventory

    private readonly List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private void OnEnable()
    {
        inventoryAction.action.Enable();
        inventory.OnChanged += Refresh;   // on s'ABONNE : "préviens-moi quand tu changes"
    }

    private void OnDisable()
    {
        inventoryAction.action.Disable();
        inventory.OnChanged -= Refresh;   // on se DÉSABONNE (toujours par paire, comme Enable/Disable)
    }

    private void Start()
    {
        // On crée une case d'affichage pour chaque case de l'inventaire
        for (int i = 0; i < inventory.MaxSlots; i++)
        {
            slotUIs.Add(Instantiate(slotPrefab, slotsContainer));
        }

        panel.SetActive(false);
        Refresh();
    }

    private void Update()
    {
        if (inventoryAction.action.WasPressedThisFrame())
        {
            // Réactiver le panneau relance automatiquement son animation d'ouverture
            panel.SetActive(!panel.activeSelf);
        }
    }

    // Met à jour toutes les cases (appelée par l'événement OnChanged)
    private void Refresh()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < inventory.Slots.Count)
            {
                slotUIs[i].Show(inventory.Slots[i]);
            }
            else
            {
                slotUIs[i].ShowEmpty();
            }
        }

        capacityText.text = $"{inventory.Slots.Count} / {inventory.MaxSlots} cases";
    }
}