using AdvancedCache.ARC;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace AdvancedCache.Benchmarks;

[MemoryDiagnoser] // Permet de traquer les allocations au byte près
[Orderer(SummaryOrderPolicy.FastestToSlowest)] // Trie les résultats du plus rapide au plus lent
[RankColumn] // Ajoute une colonne de classement (1er, 2e, etc.)
public class CacheARCBenchmarks
{
    private const int CapaciteCache = 1000;
    private CacheARC<string, string> _cacheARC = null!;
    private List<string> _clesDeTest = null!;

    [GlobalSetup]
    public void Setup()
    {
        _cacheARC = new CacheARC<string, string>(CapaciteCache);
        _clesDeTest = new List<string>(CapaciteCache * 2);

        // Pré-génération des clés pour éviter que la création de chaînes de caractères 
        // ne pollue les mesures de performance du cache
        for (int i = 0; i < CapaciteCache * 2; i++)
        {
            _clesDeTest.Add($"cle_de_test_performance_{i}");
        }

        // On remplit le cache une première fois pour le mettre en situation réelle (Cache chaud)
        for (int i = 0; i < CapaciteCache; i++)
        {
            _cacheARC.Inserer(_clesDeTest[i], "DonneeMiseEnCache");
        }
    }

    [Benchmark]
    public string? Benchmark_ARC_Obtenir_Element_Existant()
    {
        // On lit un élément qu'on sait présent pour mesurer le "Cache Hit"
        // L'élément va migrer vers la liste fréquente T2 en tâche de fond
        return _cacheARC.Obtenir(_clesDeTest[500]);
    }

    [Benchmark]
    public void Benchmark_ARC_Inserer_Avec_Eviction_Et_Adaptation()
    {
        // On insère en boucle de nouvelles clés au-delà de la capacité maximale du cache.
        // Cela va forcer l'ARC à :
        // 1. Déclencher son algorithme de remplacement.
        // 2. Évincer des éléments de la RAM vers les listes fantômes.
        // 3. Libérer la mémoire des valeurs associées.
        for (int i = CapaciteCache; i < CapaciteCache + 10; i++)
        {
            _cacheARC.Inserer(_clesDeTest[i], "NouvelleDonnee");
        }
    }

    [Benchmark]
    public string? Benchmark_ARC_Hit_Dans_Liste_Fantome()
    {
        // On force un hit dans une liste fantôme (B1 ou B2).
        // Cela va forcer la variable interne '_p' à s'ajuster dynamiquement.
        // C'est le test ultime de la logique adaptative d'IBM.
        return _cacheARC.Obtenir(_clesDeTest[0]);
    }
}