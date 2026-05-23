using AdvancedCache.LFU;

namespace AdvancedCache.Tests;
public class CacheLfuTests
{
    [Fact]
    public void Obtenir_CleInexistante_RetourneValeurParDefaut()
    {
        // Arrange
        ICache<string, string> cache = new CacheLFU<string, string>(3);

        // Act
        var resultat = cache.Obtenir("cle_inconnue");

        // Assert
        Assert.Null(resultat);
    }

    [Fact]
    public void InsererEtObtenir_CasNominal_RetourneLaValeurInseree()
    {
        // Arrange
        ICache<int, string> cache = new CacheLFU<int, string>(2);

        // Act
        cache.Inserer(1, "Premium");
        cache.Inserer(2, "Gratuit");

        // Assert
        Assert.Equal("Premium", cache.Obtenir(1));
        Assert.Equal("Gratuit", cache.Obtenir(2));
    }

    [Fact]
    public void Inserer_CleExistante_MetAJourLaValeurEtAugmenteLaFrequence()
    {
        // Arrange
        ICache<int, string> cache = new CacheLFU<int, string>(2);
        cache.Inserer(1, "Version 1");
        cache.Inserer(2, "Autre Element");

        // Act
        // On met à jour la clé 1 -> sa fréquence passe à 2
        cache.Inserer(1, "Version 2");

        // On sature le cache en insérant une clé 3. 
        // C'est la clé 2 (fréquence 1) qui doit sauter, pas la clé 1 (fréquence 2).
        cache.Inserer(3, "Nouvel Element");

        // Assert
        Assert.Equal("Version 2", cache.Obtenir(1)); // Toujours là
        Assert.Null(cache.Obtenir(2));              // Évincé !
        Assert.Equal("Nouvel Element", cache.Obtenir(3));
    }

    [Fact]
    public void Eviction_SatureLeCache_SupprimeLElementLeMoinsFrequent()
    {
        // Arrange
        var cache = new CacheLFU<string, int?>(3);
        cache.Inserer("A", 10);
        cache.Inserer("B", 20);
        cache.Inserer("C", 30);

        // On change les fréquences d'accès :
        // "A" -> lue 2 fois (freq = 3)
        cache.Obtenir("A");
        cache.Obtenir("A");

        // "B" -> lue 1 fois (freq = 2)
        cache.Obtenir("B");

        // "C" reste à freq = 1 (juste l'insertion)

        // Act
        // On insère "D". Le cache est plein (capacité 3). "C" a la plus petite fréquence (1), il doit être supprimé.
        cache.Inserer("D", 40);

        // Assert
        Assert.NotNull(cache.Obtenir("A"));
        Assert.NotNull(cache.Obtenir("B"));
        Assert.Null(cache.Obtenir("C")); // Évincé
        Assert.Equal(40, cache.Obtenir("D"));
    }

    [Fact]
    public void Eviction_EgaliteDeFrequence_SupprimeLePlusAncienSelonLRU()
    {
        // Arrange
        var cache = new CacheLFU<string, string>(2);

        // L'ordre d'insertion compte. "A" est inséré avant "B".
        cache.Inserer("A", "Pomme"); // freq = 1
        cache.Inserer("B", "Poire"); // freq = 1

        // Act
        // On insère "C". "A" et "B" ont la même fréquence minimale (1).
        // L'algorithme doit évincer le plus ancien (politique LRU), donc "A".
        cache.Inserer("C", "Pêche");

        // Assert
        Assert.Null(cache.Obtenir("A")); // Évincé (le plus ancien)
        Assert.Equal("Poire", cache.Obtenir("B"));
        Assert.Equal("Pêche", cache.Obtenir("C"));
    }

    [Fact]
    public void Instanciation_CapaciteInvalide_LeveUneArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CacheLFU<int, int>(0));
        Assert.Throws<ArgumentException>(() => new CacheLFU<int, int>(-5));
    }

    [Fact]
    public void ScenarioComplexe_DynamiqueDeFrequence()
    {
        // Arrange
        var cache = new CacheLFU<int, string>(2);

        cache.Inserer(1, "Un");     // [1: freq 1]
        cache.Inserer(2, "Deux");   // [2: freq 1], [1: freq 1]

        Assert.Equal("Un", cache.Obtenir(1)); // [1: freq 2], [2: freq 1]

        // Sature le cache. Freq min = 1 (clé 2). Clé 2 saute.
        cache.Inserer(3, "Trois"); // [1: freq 2], [3: freq 1]

        Assert.Null(cache.Obtenir(2)); // Évincé
        Assert.Equal("Trois", cache.Obtenir(3)); // [3: freq 2], [1: freq 2]

        // Maintenant 1 et 3 ont une freq de 2. On lit 3 une fois de plus.
        Assert.Equal("Trois", cache.Obtenir(3)); // [3: freq 3], [1: freq 2]

        // On insère 4. Le moins fréquent est 1 (freq 2). Clé 1 saute.
        cache.Inserer(4, "Quatre"); // [3: freq 3], [4: freq 1]

        // Assertions finales
        Assert.Null(cache.Obtenir(1));
        Assert.Equal("Trois", cache.Obtenir(3));
        Assert.Equal("Quatre", cache.Obtenir(4));
    }
}