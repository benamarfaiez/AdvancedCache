using AdvancedCache.LFU;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace AdvancedCache.Benchmarks;

[MemoryDiagnoser] // Mesure les allocations de mémoire (GC)
[Orderer(SummaryOrderPolicy.FastestToSlowest)] // Trie les résultats du plus rapide au plus lent
[RankColumn] // Ajoute une colonne de classement
public class CacheLFUBenchmark
{
    private CacheLFU<int, string> _cache = null!;
    private List<int> _clesExistantes = null!;
    private List<int> _clesInexistantes = null!;
    private Random _random = null!;

    // On teste avec différentes capacités pour voir si l'échelle impacte le O(1)
    [Params(100, 10_000)]
    public int Capacite { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _random = new Random(42); // Graine fixe pour la reproductibilité
        _cache = new CacheLFU<int, string>(Capacite);
        _clesExistantes = new List<int>();
        _clesInexistantes = new List<int>();

        // 1. Remplir le cache au max de sa capacité
        for (int i = 0; i < Capacite; i++)
        {
            _cache.Inserer(i, $"Valeur-{i}");
            _clesExistantes.Add(i);
        }

        // 2. Préparer des clés qui n'existent pas pour tester les "Cache Miss"
        for (int i = Capacite; i < Capacite * 2; i++)
        {
            _clesInexistantes.Add(i);
        }
    }

    [Benchmark(Description = "Obtenir (Cache Hit)")]
    public string? BenchmarkObtenirHit()
    {
        // On pioche une clé existante au hasard
        int cleAleatoire = _clesExistantes[_random.Next(_clesExistantes.Count)];
        return _cache.Obtenir(cleAleatoire);
    }

    [Benchmark(Description = "Obtenir (Cache Miss)")]
    public string? BenchmarkObtenirMiss()
    {
        // On pioche une clé inexistante au hasard
        int cleAleatoire = _clesInexistantes[_random.Next(_clesInexistantes.Count)];
        return _cache.Obtenir(cleAleatoire);
    }

    [Benchmark(Description = "Inserer (Mise à jour)")]
    public void BenchmarkInsererMiseAJour()
    {
        int cleAleatoire = _clesExistantes[_random.Next(_clesExistantes.Count)];
        _cache.Inserer(cleAleatoire, "NouvelleValeur");
    }

    [Benchmark(Description = "Inserer avec Éviction LFU")]
    public void BenchmarkInsererEviction()
    {
        // On insère une clé inexistante, ce qui force le cache (plein) 
        int nouvelleCle = _clesInexistantes[_random.Next(_clesInexistantes.Count)];
        _cache.Inserer(nouvelleCle, "DonneeFlash");
    }
}
