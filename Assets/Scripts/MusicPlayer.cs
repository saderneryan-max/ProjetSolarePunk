using System.Collections;
using UnityEngine;

// Lecteur de musique d'ambiance : enchaîne plusieurs morceaux avec un fondu entre chaque.
// Il survit aux changements de scène : la musique ne redémarre pas à chaque écran.
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [Header("Playlist")]
    [SerializeField] private AudioClip[] tracks;          // les morceaux à enchaîner
    [SerializeField] private bool shuffle = true;          // ordre aléatoire ?

    [Header("Volume et fondus")]
    [SerializeField] private float volume = 0.6f;
    [SerializeField] private float fadeDuration = 2f;      // durée des fondus (en secondes)

    // Le lecteur unique, partagé entre toutes les scènes
    private static MusicPlayer instance;

    private AudioSource audioSource;
    private int currentIndex = -1;                         // -1 = aucun morceau joué pour l'instant

    private void Awake()
    {
        // Un lecteur existe déjà (venu d'une scène précédente) : on garde celui-là, on supprime le doublon
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // cet objet ne sera pas détruit au changement de scène

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;               // c'est le script qui enchaîne les morceaux
        audioSource.playOnAwake = false;
        audioSource.ignoreListenerPause = true; // la musique continue pendant le menu pause
    }

    private void Start()
    {
        if (tracks.Length > 0)
        {
            StartCoroutine(PlaylistLoop());
        }
        else
        {
            Debug.LogWarning("MusicPlayer : aucun morceau dans la playlist !");
        }
    }

    // Joue les morceaux les uns après les autres, à l'infini
    private IEnumerator PlaylistLoop()
    {
        while (true) // boucle infinie sans danger : chaque "yield" rend la main au jeu
        {
            AudioClip clip = tracks[PickNextIndex()];
            audioSource.clip = clip;
            audioSource.volume = 0f;
            audioSource.Play();

            // 1. Fondu d'entrée
            yield return StartCoroutine(Fade(0f, volume));

            // 2. On attend que le morceau arrive presque à sa fin
            //    (ou qu'il s'arrête, par sécurité, pour ne jamais rester bloqué)
            yield return new WaitUntil(() =>
                audioSource.time >= clip.length - fadeDuration || !audioSource.isPlaying);

            // 3. Fondu de sortie, puis on recommence avec le morceau suivant
            yield return StartCoroutine(Fade(volume, 0f));
        }
    }

    // Fait passer le volume progressivement de "from" à "to"
    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            // unscaledDeltaTime : le temps réel, qui continue même quand le jeu est en pause
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null; // attend l'image suivante
        }
        audioSource.volume = to;
    }

    // Choisit le prochain morceau (jamais deux fois de suite le même en mode aléatoire)
    private int PickNextIndex()
    {
        if (!shuffle || tracks.Length == 1)
        {
            // % = reste de la division : après le dernier morceau, on revient au premier
            currentIndex = (currentIndex + 1) % tracks.Length;
        }
        else
        {
            int next;
            do
            {
                next = Random.Range(0, tracks.Length);
            }
            while (next == currentIndex); // on retire au sort tant que c'est le même

            currentIndex = next;
        }
        return currentIndex;
    }
}