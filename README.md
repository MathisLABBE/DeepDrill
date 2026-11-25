# DeepDrill – jeu mobile de forage

Jeu mobile en 2D où l'on pilote une foreuse qui creuse **le plus profond possible** avant de tomber à court d'énergie. En chemin, on récolte des minerais, on évite la lave, puis on dépense ses gains en améliorations pour aller plus loin à la partie suivante.

Projet personnel, réalisé seul depuis août 2026, en cours.

## Gameplay

- La foreuse avance en continu dans la direction où elle pointe ; le joueur la **dirige** et elle ralentit selon la **dureté du terrain** rencontré.
- Le terrain est **généré à l'infini** pendant la descente : roche, blocs indestructibles, **filons de diamant et d'or** et **poches de lave** mortelles.
- Le tunnel creusé reste visible derrière la foreuse.
- En fin de partie, les minerais sont convertis en pièces, qui servent à acheter des **améliorations** : énergie, puissance de forage, taille de la foreuse, aimant à minerais.

## Ce qui m'a intéressé techniquement

- **Génération procédurale** du terrain avec du **bruit de Perlin** : les minerais forment des filons et la lave des poches cohérentes, plutôt que des blocs posés au hasard.
- Passage d'un forage « case par case » à un **forage physique continu** : la tête de la foreuse détruit le terrain au contact, et une sonde placée devant elle mesure sa résistance pour régler la vitesse.
- Nettoyage du terrain déjà traversé pour garder un **jeu fluide sur téléphone**.
- **Monétisation** avec LevelPlay : vidéo récompensée pour doubler les gains ou repartir après une défaite, publicité plein écran toutes les quelques parties.
- Progression sauvegardée localement (pièces et niveaux d'amélioration).

## Technologies

- Unity 6 (6000.3) · C#
- LevelPlay (Unity Ads / ironSource) pour les publicités
- TextMesh Pro · Git LFS pour les fichiers audio et binaires

## Organisation du code

| Dossier `Assets/Scripts/` | Rôle |
|---|---|
| `Player/` | Déplacement et orientation de la foreuse, tête de forage, sonde de terrain, tunnel |
| `World/` | Génération du terrain, types d'éléments et de minerais, lave |
| `Gameplay/` | Déroulement d'une partie : énergie, profondeur, récolte, fin de partie, relance |
| `Progression/` | Sauvegarde des pièces et des améliorations |
| `UI/` | Menu principal (améliorations), interface en jeu, écran de fin |
| `Ads/` | Chargement et affichage des publicités |
| `Camera/` | Suivi de la foreuse |

## Ouvrir le projet

Prérequis : [Unity Hub](https://unity.com/download) avec **Unity 6000.3.23f1** et le module Android, et [Git LFS](https://git-lfs.com/).

```powershell
git lfs install
git clone https://github.com/MathisLABBE/DeepDrill.git
```

Ouvrir ensuite le dossier dans Unity Hub, puis la scène de départ `Assets/Scenes/MainMenu.unity` (menu et améliorations) ; la partie se joue dans `Game.unity`.

## Auteur

Mathis LABBE – étudiant en BUT Informatique en alternance · [GitHub](https://github.com/MathisLABBE) · [LinkedIn](https://www.linkedin.com/in/mathis-labbe-92518131b/)
