using UnityEngine;

// Les 3 niveaux de qualité d'une récolte
public enum CropQuality
{
    Bronze,
    Argent,
    Or
}

// Les 2 sortes d'objets que l'inventaire peut contenir
public enum ItemKind
{
    Seed,     // une graine (à planter)
    Harvest   // une récolte (à vendre ou à livrer)
}

// Outils liés à la qualité : couleur, nom, effet sur le prix, tirage au sort.
// "static" : on n'a pas besoin de créer d'objet, on appelle directement QualityUtils.Roll(...)
public static class QualityUtils
{
    // Place de la qualité dans la fourchette de prix : 0 = prix minimum, 1 = prix maximum
    public static float GetPriceFactor(CropQuality quality)
    {
        switch (quality)
        {
            case CropQuality.Or: return 1f;
            case CropQuality.Argent: return 0.5f;
            default: return 0f;
        }
    }

    public static Color GetColor(CropQuality quality)
    {
        switch (quality)
        {
            case CropQuality.Or: return new Color(1f, 0.83f, 0.35f);   // doré
            case CropQuality.Argent: return new Color(0.80f, 0.85f, 0.90f); // argenté
            default: return new Color(0.80f, 0.50f, 0.20f); // bronze
        }
    }

    // Version foncée de la couleur, lisible sur un fond clair (parchemin)
    public static Color GetDarkColor(CropQuality quality)
    {
        switch (quality)
        {
            case CropQuality.Or: return new Color(0.72f, 0.53f, 0.04f);
            case CropQuality.Argent: return new Color(0.40f, 0.46f, 0.54f);
            default: return new Color(0.63f, 0.32f, 0.18f);
        }
    }

    public static string GetName(CropQuality quality)
    {
        switch (quality)
        {
            case CropQuality.Or: return "Or";
            case CropQuality.Argent: return "Argent";
            default: return "Bronze";
        }
    }

    // Tire une qualité au hasard.
    // difficulty : 0 = début de partie, 1 = fin de partie (l'Or devient rare). Utilisé au bloc 3.
    public static CropQuality Roll(float difficulty)
    {
        difficulty = Mathf.Clamp01(difficulty);

        float goldChance = Mathf.Lerp(0.25f, 0.05f, difficulty);   // 25 % au début, 5 % à la fin
        float silverChance = Mathf.Lerp(0.40f, 0.25f, difficulty); // 40 % au début, 25 % à la fin

        float roll = Random.value; // nombre au hasard entre 0 et 1

        if (roll < goldChance) return CropQuality.Or;
        if (roll < goldChance + silverChance) return CropQuality.Argent;
        return CropQuality.Bronze;
    }
}