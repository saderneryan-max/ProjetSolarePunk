# 🌱 Le Potager Solarpunk

Aux commandes d'un drone horticole alimenté au soleil, fais pousser un potager vivant, gère ton économie et livre la commande parfaite… le plus vite possible !

🎮 **Jouer dans le navigateur / télécharger pour Windows : https://rays4300.itch.io/le-potager-solarpunk**

Projet de synthèse – IUT Informatique, 2026 – réalisé par **Ryan Saderne** avec **Unity 6.3** (URP).

---

## 🎯 Le jeu

1. Sur l'**affiche de la base**, choisis une commande parmi trois (Facile, Moyenne, Difficile).
2. Plante, arrose et récolte carottes, tournesols et potirons. Chaque récolte reçoit une qualité **Bronze**, **Argent** ou **Or**, tirée au hasard.
3. Vends au **marché**, rachète de meilleures graines, et dépose dans la **caisse de livraison** les 15 récoltes demandées, avec la qualité exacte.
4. Expédie : **score = points de récolte + points de la commande × bonus de rapidité** (×2 avant 10 min, ×1,5 avant 20 min, ×1 ensuite).

Défaite : **12 légumes perdus**, ou **faillite** (plus de graines, de récoltes ni de Punk).
Le meilleur score est sauvegardé (PlayerPrefs).

## 🎮 Contrôles

| Action | Clavier / Souris | Manette |
|---|---|---|
| Se déplacer | ZQSD (WASD) | Stick gauche |
| Tourner la caméra | Souris | Stick droit |
| Planter | Espace | A |
| Arroser (maintenir) | E | X |
| Inventaire | Tab | Y |
| Interagir | Clic gauche | B |
| Fermer une fenêtre | Échap | B |
| Pause | Échap | Start |

## 🧩 Fonctionnalités

- Déplacement du drone par **Input System** (clavier + manette), caméra orbitale en plongée, suivi du relief par **Raycast**
- Croissance par **états** (graine → mûr → trop mûr → éclatement) avec `Time.deltaTime`, arrosage qui accélère la pousse
- **Zones** de terrain (plaine, colline, creux, mare) avec un légume par zone, réservoir d'eau à remplir aux mares
- Récolte par **Trigger**, injection de dépendances (`Setup`) à l'instanciation
- **Inventaire** à piles (même légume + même qualité), **marché** (vente / achat), **commandes** et caisse de livraison
- Données en **ScriptableObjects** (`CropData`, `OrderData`), légumes en **Prefab Variants**
- Classes de base **`Interactable`** et **`GameWindow`** (héritage) pour le marché, la caisse et l'affiche
- **Animator / AnimationClips** : croissance, hélices, interface, fenêtres ; **Coroutines** (fin de partie, playlist musicale)
- Sons (Audio Mixer), musique en playlist persistante, particules
- 3 scènes (titre, jeu, fin), menu pause, difficulté croissante, build **WebGL** et **Windows**

## 📁 Organisation du projet

```
Assets/
├ Animations/   Drone · Plants · UI
├ Audio/        Musique · SFX
├ Data/         Crops · Orders (ScriptableObjects)
├ Input/        DroneControls (Input Actions)
├ Models/       Base · Crops · Drone · Nature
├ Prefabs/      Effects · PlantStages · UI · Plant + variants
├ Scenes/       TitleScene · GameScene · EndScene
├ Scripts/      Drone · Inventory · Market · Orders · UI · Water · Zone …
└ Textures/ · UI/ · VFX/
```

Réglages rapides : vitesse du drone → `Drone` › *Drone Movement* › **Move Speed** ;
durées de pousse → `Prefabs/Plant_Carrot` (et variants) › *Plant* › **Seed Duration**.

## 🙏 Crédits

Voir [CREDITS.md.txt](CREDITS.md).

Les modèles du drone, de la base et des tournesols, ainsi que les visuels de la page itch.io, ont été générés par script Blender avec l'assistance de Claude (IA d'Anthropic). Le code a été écrit avec l'accompagnement pédagogique de la même IA.
