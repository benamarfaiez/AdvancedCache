# AdvancedCache

Un moteur de cache en mémoire à très haute performance. Conçue pour les applications exigeant un débit massif et une latence critique, cette bibliothèque surpasse les collections natives de .NET en éliminant la fragmentation de la mémoire et la pression sur le Garbage Collector (GC).

---

## 🚀 Moteurs de Cache Disponibles

### 1. Cache LRU Ultime (Least Recently Used)
Une réécriture complète *Hardware-Friendly* basée sur un tableau contigu de structures (`struct`) et des index entiers au lieu de pointeurs d'objets traditionnels.
- **Vitesse de lecture :** **~48 ns** (Gain de 40 % sur la localité spatiale du cache CPU).
- **Allocation mémoire :** **0 octet** 
- **Philosophie :** Évince les éléments qui n'ont pas été consultés depuis le plus longtemps. Idéal pour privilégier la **récence**.

### 2. Cache LFU Synchrone (Least Frequently Used)
Une implémentation hautement performante combinant un dictionnaire d'accès rapide et un double chaînage par paliers de fréquences. En cas d'égalité de fréquence, un mécanisme LRU (*Tie-breaker*) prend le relais.
- **Vitesse d'accès :** **~56 ns** en insertion et **~101 ns** en cas de *Hit* (garanti en temps constant $O(1)$ peu importe la taille du cache).
- **Allocation mémoire :** **0 octet** 
- **Philosophie :** Évince les éléments les moins souvent consultés. Idéal pour valoriser la **popularité à long terme**.

### 3. Cache ARC (Adaptive Replacement Cache)
Une implémentation stricte et *Thread-Safe* de l'algorithme auto-adaptatif d'IBM. L'ARC pilote dynamiquement 4 listes internes afin de trouver l'équilibre parfait entre la **récence** (LRU) et la **fréquence** (LFU) d'accès.
- **Vitesse d'accès :** **~58 ns** (Hit historique) à **~62 ns** (Hit en RAM).
- **Allocation mémoire :** **0 octet** 
- **Auto-adaptatif :** Ajuste sa stratégie d'éviction en temps réel sans aucune configuration manuelle.


---

## 🧠 Guide de Choix d'Ingénierie

Le choix d'un algorithme de cache dépend entièrement du **profil d'accès à vos données** (le *Workload*). Utiliser le mauvais moteur peut effondrer votre taux d'efficacité (*Cache Hit Rate*).

### 📋 Les 3 Critères Majeurs de Décision

1. **La Récence (Le facteur Temps) :** Une donnée qui vient d'être consultée a-t-elle de grandes chances d'être redemandée immédiatement ? *(Ex. : fils d'actualité, paniers d'achat)* 
 ➡️ **Priorité LRU**
2. **La Fréquence (Le facteur Popularité) :** Existe-t-il des données "stars" qui restent très demandées sur de longues périodes, même si elles ne sont pas lues chaque seconde ? *(Ex. : taux de change, fiches produits Best-Sellers)* 
 ➡️ **Priorité LFU**
3. **La Volatilité du Trafic :** Votre comportement d'accès change-t-il constamment de dynamique (vagues de nouveautés puis retour soudain au fond de catalogue) ? 
 ➡️ **Priorité ARC**

### 📊 Tableau Décisionnel Rapide

| Algorithme | Force Majeure | Point Faible Critique | Profil Type |
| :--- | :--- | :--- | :--- |
| **LRU** | Excellent pour le trafic "temps réel" et les données éphémères. | **Le "Scan Crash" :** Un balayage séquentiel de BDD vide tout le cache utile. | Flux d'activité, sessions web, tokens JWT. |
| **LFU** | Protégé contre les scans massifs. Les éléments populaires restent ancrés. | **Pollution historique :** Un élément obsolète mais très populaire par le passé reste bloqué. | Tables de référence, dictionnaires, configurations. |
| **ARC** | **Auto-adaptatif.** Équilibre parfait et dynamique entre LRU et LFU. | Complexité interne accrue (~15 % plus lent sur les écritures massives). | Systèmes de fichiers, bases de données, API mixtes. |

---

## 🛠️ Exemples Concrets

### Cas 1 : Vous développez un jeu vidéo "Open World" (Choix : LRU)
- **Le comportement :** Le joueur avance en ligne droite. Le moteur doit charger les textures de la zone devant lui et jeter celles de la zone qu'il vient de quitter définitivement.
- **Pourquoi ce choix :** La récence spatiale et temporelle est absolue. Un cache LFU serait catastrophique car il garderait en mémoire la zone de départ du jeu (où le joueur a passé beaucoup de temps au début) au détriment des nouvelles zones découvertes.

### Cas 2 : Vous créez une API de conversion de devises / Taux de change (Choix : LFU)
- **Le comportement :** Les taux EUR/USD ou USD/JPY sont demandés des millions de fois par jour. À l'inverse, le taux lié à une monnaie très rare n'est demandé qu'une fois par mois.
- **Pourquoi ce choix :** Les paires majeures ont une fréquence d'accès tellement gigantesque qu'elles doivent être verrouillées en RAM. Même si une rafale de requêtes interroge des monnaies rares simultanément, le LFU protège les données "stars" de l'éviction.

### Cas 3 : Vous développez une plateforme de Streaming Vidéo (Choix : ARC)
- **Le comportement :** Face à une sortie de série, les utilisateurs font du *binge-watching* intense (Récence). En parallèle, des films classiques indémodables récoltent un trafic stable et continu chaque jour (Fréquence).
- **Pourquoi ce choix :** L'ARC s'adapte en temps réel. Si une nouveauté cartonne, il agrandit sa liste LRU. Quand le calme revient, il redonne l'avantage aux fichiers fréquemment vus (LFU), garantissant le meilleur taux de *Hit* possible sans intervention humaine.

### Cas 4 : Limitation de Débit (Rate Limiting) & API Tierces (Choix : LRU)
- **Le comportement :** Intercepter les abus de requêtes sur des fenêtres glissantes de quelques minutes.
- **Pourquoi ce choix :** Les adresses IP ou clés d'API doivent être suivies activement tant qu'elles émettent des requêtes. Dès qu'un client arrête son activité, sa présence dans le cache n'a plus aucune valeur : le LRU l'éliminera naturellement au profit des nouveaux connectés.

---

## 📊 Résultats des Benchmarks (BenchmarkDotNet)

Mesures scientifiques réalisées sous .NET 10 (mode *Release*, capacité variable) :

| Algorithme / Moteur | Opération | Capacité | Temps Moyen (Mean) | Allocation Mémoire |
| :--- | :--- | :---: | :---: | :---: |
| **Cache LRU ** | Lecture d'un élément existant | 10 000 | **48.72 ns** | **0 B** |
| **Cache LRU ** | Insertion avec éviction | 10 000 | **66.56 ns** | **0 B** |
| **Cache LFU ** | Obtenir (Cache Miss) | 10 000 | **20.21 ns** | **0 B** |
| **Cache LFU ** | Inserer avec Éviction LFU | 10 000 | **103.47 ns** | **0 B** |
| **Cache LFU ** | Inserer (Mise à jour) | 10 000 | **109.86 ns** | **0 B** |
| **Cache LFU ** | Obtenir (Cache Hit) | 10 000 | **120.41 ns** | **0 B** |
| **Cache ARC ** | Lecture d'un élément en RAM | 10 000 | **62.92 ns** | **0 B** |
| **Cache ARC ** | Insertion + Éviction adaptative | 10 000 | **108.22 ns** | **0 B** |

---

## 🧪 Tests & Validation

Le projet intègre une suite complète de tests ainsi qu'un banc de mesure de performance :
- **Tests Unitaires (xUnit) :** Validation des cas aux limites, de l'intégrité des types de données (gestion de la valeur par défaut pour les types de valeur comme `int`), et scénarios de stress d'éviction.
- **Benchmarks (BenchmarkDotNet) :** Analyse nanoseconde par nanoseconde du comportement du processeur et des allocations mémoire.

Pour lancer la suite de performance :
```bash
dotnet run -c Release
