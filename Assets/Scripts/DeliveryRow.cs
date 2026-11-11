using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Une ligne de la fenêtre de livraison : icône, qualité, texte, valeur, et un bouton d'action.
public class DeliveryRow : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image qualityBadge;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionLabel;

    public Button ActionButton => actionButton;

    public void Setup(CropData crop, CropQuality quality, string label, string value,
                      string actionName, UnityAction onClick, bool canAct)
    {
        icon.sprite = crop.Icon;
        qualityBadge.color = QualityUtils.GetColor(quality);
        nameText.text = label;
        valueText.text = value;
        actionLabel.text = actionName;

        actionButton.onClick.AddListener(onClick);
        actionButton.interactable = canAct;
    }
}