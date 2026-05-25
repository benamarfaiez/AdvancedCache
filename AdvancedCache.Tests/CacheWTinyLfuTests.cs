using AdvancedCache.WTLFU;

namespace AdvancedCache.Tests;

public class CacheWTinyLfuTests
{
    [Fact]
    public void Obtenir_CleInexistante_RetourneValeurParDefaut()
    {
        var cache = new CacheWTinyLfu<string, int>(10);
        Assert.Equal(0, cache.Obtenir("Inconnu")); // Gestion propre des Value Types (int -> 0)
    }

    [Fact]
    public void InsererEtObtenir_CasNominal_StockeLaDonnee()
    {
        var cache = new CacheWTinyLfu<string, string>(10);
        cache.Inserer("Cle1", "Donnee1");
        Assert.Equal("Donnee1", cache.Obtenir("Cle1"));
    }

    [Fact]
    public void Eviction_FiltreTinyLfu_RejetteLesScansEtGardeLesElementsPopulaires()
    {
        // Capacité de 10
        var cache = new CacheWTinyLfu<string, int>(10);

        // On remplit le cache principal et on simule une forte popularité sur "Star"
        cache.Inserer("Star", 999);
        for (int i = 0; i < 20; i++) cache.Obtenir("Star"); // Fréquence énorme

        // On insère d'autres éléments pour saturer le cache
        for (int i = 1; i <= 15; i++)
        {
            cache.Inserer($"Element-{i}", i);
        }

        // L'élément "Star" doit impérativement survivre grâce au filtre TinyLFU
        Assert.Equal(999, cache.Obtenir("Star"));
    }

    [Fact]
    public void Instanciation_CapaciteTropFaible_LeveUneArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CacheWTinyLfu<string, string>(5));
    }

}