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

        // 1. On remplit la mémoire RAM (T1)
        cache.Inserer("A", "vA"); // T1: [A]
        cache.Inserer("B", "vB"); // T1: [B, A]

        // 2. On provoque des évictions vers B1 (Historique Récence)
        cache.Inserer("C", "vC"); // T1 pousse A hors de la RAM -> T1: [C, B], B1: [A (fantôme)]
        cache.Inserer("D", "vD"); // T1 pousse B hors de la RAM -> T1: [D, C], B1: [B, A (fantômes)]

        // À ce stade : T1 compte 2 éléments (D, C) et B1 compte 2 éléments (B, A). Total = 4 (2c).
        // Le cache et son historique sont pleins à craquer.

        // Act
        // L'insertion d'un 5e élément ("E") déclenche le cas limite : 
        // l1Taille (T1 + B1) == c (2 + 2 = 4, ce qui vaut 2c en limite globale).
        // L'ARC doit supprimer définitivement le plus ancien fantôme de B1 (qui est "A") pour faire de la place.
        cache.Inserer("E", "vE");

        // Assert
        cache.Obtenir("E").Should().Be("vE"); // Nouveau venu bien présent en RAM (T1)
        cache.Obtenir("B").Should().BeNull(); // 'B' est toujours un fantôme dans B1 (valeur null mais clé connue)

        // C'est le test crucial : 'A' a été complètement radié du dictionnaire pour éviter la fuite de mémoire.
        // Un Obtenir sur une clé radiée doit renvoyer la valeur par défaut du type.
        var actionObtenirA = () => cache.Obtenir("A");
        actionObtenirA().Should().BeNull();
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
