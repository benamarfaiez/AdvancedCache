using AdvancedCache.LRU;
using BenchmarkDotNet.Attributes;

namespace AdvancedCache.Benchmarks;

[MemoryDiagnoser]
public class CacheLRUBenchmarks
{
    private CacheLRU<string, int> _cache;

    // Configuration initiale avant l'exécution des tests
    [GlobalSetup]
    public void Setup()
    {
        // On initialise un cache de taille 1000 et on le pré-remplit
        _cache = new CacheLRU<string, int>(1000);
        for (int i = 0; i < 990; i++)
        {
            _cache.Inserer($"cle_{i}", i);
        }
    }

    // Test 1 : Vitesse de lecture (doit être proche du O(1) pur)
    [Benchmark]
    public int Benchmark_Obtenir_Element_Existant()
    {
        // On lit une clé du milieu, ce qui force la réorganisation de la LinkedList
        return _cache.Obtenir("cle_500");
    }

    // Test 2 : Vitesse d'insertion avec éviction (politique LRU)
    [Benchmark]
    public void Benchmark_Inserer_Avec_Eviction()
    {
        // Le cache est plein (limite 1000), cette insertion va forcer 
        // la suppression instantanée de l'élément le plus ancien.
        _cache.Inserer("nouvelle_cle_concurrente", 9999);
    }
}
