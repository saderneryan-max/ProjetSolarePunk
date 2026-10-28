// Petite "boîte" partagée entre les scènes.
// Une variable static survit au changement de scène, mais PAS à la fermeture du jeu.
public static class GameSession
{
    public static int LastScore = 0; // score de la dernière partie terminée
}