using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Affiche UNE case de l'inventaire : icône, quantité, qualité (ou "graine").
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image qualityBadge;
    [SerializeField] private GameObject seedTag;

    public void Show(InventorySlot slot)
    {
        icon.enabled = true;
        icon.sprite = slot.Crop.Icon;
        countText.text = $"x{slot.Count}";

        bool isHarvest = slot.Kind == ItemKind.Harvest;

        // Une récolte affiche sa qualité, une graine affiche l'étiquette "graine"
        qualityBadge.enabled = isHarvest;
        if (isHarvest)
        {
            qualityBadge.color = QualityUtils.GetColor(slot.Quality);
        }
        seedTag.SetActive(!isHarvest);
    }

    public void ShowEmpty()
    {
        icon.enabled = false;
        countText.text = "";
        qualityBadge.enabled = false;
        seedTag.SetActive(false);
    }
}