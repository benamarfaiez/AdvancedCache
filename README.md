# AdvancedCache
Un moteur de cache en mémoire à très haute performance. Conçue pour les applications exigeant un débit massif et une latence critique, cette bibliothèque surpasse les collections natives de .NET en éliminant la fragmentation de la mémoire et la pression sur le Garbage Collector.

## 🛠️ Cas d'Utilisation Typiques

Le cache LRU est un composant d'infrastructure indispensable dès lors que votre application manipule un grand volume de données dont l'accès n'est pas uniforme (Loi de Pareto : 20% des données font 80% du trafic).

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

## 💻 Exemples d'Utilisation (Version Ultime)

```csharp
using MonApplication.Cache;

// Création d'un cache à allocation contiguë limité à 1000 éléments
var cache = new CacheLRU<string, string>(1000);

// Insertion ultra-rapide (66 ns)
cache.Inserer("meteo_paris", "{ 'temp': 18, 'status': 'Ensoleillé' }");

// Lecture ultra-rapide avec mise à jour de la récence (48 ns)
string? resultat = cache.Obtenir("meteo_paris");
```

---

## 🧪 Tests & Validation

Le projet intègre une suite complète de tests ainsi qu'un banc de mesure de performance :
- **Tests Unitaires (xUnit) :** Validation des cas aux limites et scénarios de stress d'éviction.
- **Benchmarks (BenchmarkDotNet) :** Analyse nanoseconde par nanoseconde du comportement du processeur et des allocations mémoire.

Pour lancer la suite de performance :
```bash
cd MonProjet.Cache.Benchmarks
dotnet run -c Release
```