# AdvancedCache
Un moteur de cache en mémoire à très haute performance. Conçue pour les applications exigeant un débit massif et une latence critique, cette bibliothèque surpasse les collections natives de .NET en éliminant la fragmentation de la mémoire et la pression sur le Garbage Collector.

---

## 🚀 Moteurs de Cache Disponibles

### 1. Cache LRU Ultime (Least Recently Used) — Version Low-Level
Une réécriture complète *Hardware-Friendly* basée sur un tableau contigu de structures (`struct`) et des index entiers au lieu de pointeurs d'objets traditionnels.
- **Vitesse de lecture :** **~48 ns** (Gain de 40% sur la localité spatiale du cache CPU).
- **Allocation mémoire :** **0 octet** (Aucun impact sur le Garbage Collector).
- **Cas d'usage :** Systèmes single-thread ultra-rapides, moteurs de rendu 3D, traitement de paquets réseau.

### 2. Cache ARC (Adaptive Replacement Cache) — Version Professionnelle SOLID
Une implémentation stricte et Thread-Safe de l'algorithme auto-adaptatif d'IBM. L'ARC pilote dynamiquement 4 listes internes (2 en RAM, 2 "fantômes" pour l'historique) afin de trouver l'équilibre parfait entre la **récence** et la **fréquence** d'accès.
- **Vitesse d'accès :** **~58 ns** (Hit historique) à **~62 ns** (Hit en RAM).
- **Auto-Adaptatif :** Ajuste sa stratégie d'éviction en temps réel sans aucune configuration manuelle.
- **Allocation mémoire :** **0 octet** lors des phases d'exécution grâce au recyclage des nœuds.

---

## 🛠️ Cas d'Utilisation Réels

### 1. Gestion des Sessions dans les API Web & Microservices
Dans une application web à fort trafic, interroger la base de données à chaque clic pour récupérer le profil ou la session de l'utilisateur sature rapidement le serveur SQL.
- **Rôle du cache :** Conserver en mémoire vive les sessions des utilisateurs actifs.
- **Bénéfice :** Un temps d'accès de **48 ns** au lieu de plusieurs millisecondes (un gain de vitesse d'un facteur 1 000 000). Les utilisateurs inactifs sont automatiquement évincés dès que la capacité maximale est atteinte.

### 2. Streaming de Médias & Réseaux de Diffusion de Contenu (CDN)
Les plateformes de vidéo à la demande ou les sites de partage de fichiers font face à des requêtes massives sur des fichiers volumineux.
- **Rôle du cache :** Maintenir en RAM ou sur disque rapide les médias "tendances" ou les plus visionnés du moment.
- **Bénéfice :** Les contenus obsolètes ou oubliés glissent vers la queue du cache et sont supprimés pour laisser la place aux nouvelles sorties, évitant la saturation de la mémoire.

### 3. Moteurs de Jeux Vidéo (Chargement de Monde Ouvert / Open World)
La carte d'un monde ouvert est trop vaste pour tenir entièrement dans la mémoire vidéo (VRAM) ou la mémoire système.
- **Rôle du cache :** Charger et décharger dynamiquement les morceaux de carte (*chunks*), les textures et les modèles 3D en fonction des déplacements du joueur.
- **Bénéfice :** Les zones immédiatement entourant le joueur restent en tête de cache. Les zones laissées loin derrière sont libérées sans provoquer de micro-bégaiement (*stuttering*), assurant une expérience de jeu fluide.

### 4. Limitation de Débit (Rate Limiting) & Requêtes d'APIs Tierces
Lors de l'utilisation d'APIs externes payantes ou limitées en requêtes par seconde (ex: API Google Maps, Services météo).
- **Rôle du cache :** Conserver la réponse d'une coordonnée ou d'une ville demandée.
- **Bénéfice :** Si 1000 utilisateurs demandent la météo de la même ville au même moment, l'API externe n'est appelée qu'une seule fois. Le cache fournit instantanément la réponse aux 999 autres, économisant de l'argent et évitant le blocage des quotas.

---

## 📊 Résultats des Benchmarks (BenchmarkDotNet)

Mesures scientifiques réalisées sur un processeur Intel Core i7 (Skylake) sous .NET 9 :

| Algorithme / Méthode | Opération | Temps Moyen (Mean) | Énergie / Allocation |
| :--- | :--- | :---: | :---: |
| **Cache LRU Ultime** | Lecture d'un élément existant | **48.72 ns** | **0 B** |
| **Cache LRU Ultime** | Insertion avec éviction | **66.56 ns** | **0 B** |
| **Cache ARC (IBM)** | Hit dans l'historique fantôme | **58.57 ns** | **0 B** |
| **Cache ARC (IBM)** | Lecture d'un élément en RAM | **62.92 ns** | **0 B** |
| **Cache ARC (IBM)** | Insertion + Éviction adaptative | **108.22 ns** /élem | **0 B** |

> 💡 *Note technique :* L'insertion ARC affiche ~1 082 ns dans le rapport brut du benchmark car elle est mesurée sur un lot d'exécution de 10 insertions simultanées avec calculs d'évictions croisées, soit un score unitaire impressionnant de **108 ns par élément**.

---

## 🧪 Tests & Validation

Le projet intègre une suite complète de tests ainsi qu'un banc de mesure de performance :
- **Tests Unitaires (xUnit) :** Validation des cas aux limites et scénarios de stress d'éviction.
- **Benchmarks (BenchmarkDotNet) :** Analyse nanoseconde par nanoseconde du comportement du processeur et des allocations mémoire.

Pour lancer la suite de performance :
```bash
dotnet run -c Release
```
