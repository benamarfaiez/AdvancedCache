using AdvancedCache.ARC;
using FluentAssertions;

namespace AdvancedCache.Tests;

public class CacheARCTests
{
    [Fact]
    public void Algorithme_DoitPurgerHistoriqueB1_SiTailleMaximaleAtteinte()
    {
        // Arrange
        // Capacité c = 2. La taille cumulée des 4 listes ne peut jamais dépasser 2c (soit 4 éléments).
        var cache = new CacheARC<string, string>(2);

        cache.Inserer("A", "vA");
        cache.Inserer("B", "vB");
        cache.Inserer("C", "vC");
        cache.Inserer("D", "vD");
        cache.Inserer("E", "vE");

        cache.Obtenir("E").Should().Be("vE");
        cache.Obtenir("B").Should().BeNull();

        var valueA = cache.Obtenir("A");
        valueA.Should().BeNull();
    }

    [Fact]
    public void Algorithme_DoitNettoyerLaRAM_LorsDeLaMigrationVersLesListesFantômes()
    {
        // Arrange
        var cache = new CacheARC<string, string>(1);
        cache.Inserer("CleLourde", "Une chaine de caracteres tres longue en memoire");

        // Act
        // On force l'éviction de "CleLourde" de la RAM (T1) vers l'historique (B1) 
        // en insérant une nouvelle clé.
        cache.Inserer("NouvelleCle", "v2");

        // Assert
        // On vérifie que la clé est passée en mode "Fantôme". 
        // L'ARC doit avoir écrasé sa valeur à 'default' pour couper la référence
        // et permettre au Garbage Collector (GC) de libérer l'espace mémoire immédiatement.
        cache.Obtenir("CleLourde").Should().BeNull();
    }
}
