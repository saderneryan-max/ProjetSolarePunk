// "Boîte" partagée entre les scènes : résultat de la dernière partie.
public static class GameSession
{
    public static int LastScore = 0;
    public static int HarvestScore = 0;
    public static int DeliveryBonus = 0;
    public static bool IsNewRecord = false;
    public static bool IsVictory = true;
    public static string EndMessage = "";
}