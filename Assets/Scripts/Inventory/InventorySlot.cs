// Une case de l'inventaire : quel objet, de quelle sorte, de quelle qualité, et combien.
[System.Serializable]
public class InventorySlot
{
    public CropData Crop;
    public ItemKind Kind;
    public CropQuality Quality;
    public int Count;

    public InventorySlot(CropData crop, ItemKind kind, CropQuality quality, int count)
    {
        Crop = crop;
        Kind = kind;
        Quality = quality;
        Count = count;
    }

    // Deux objets s'empilent SEULEMENT s'ils ont exactement les mêmes attributs
    public bool CanStackWith(CropData crop, ItemKind kind, CropQuality quality)
    {
        return Crop == crop && Kind == kind && Quality == quality;
    }
}