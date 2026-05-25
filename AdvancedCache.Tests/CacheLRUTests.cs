using AdvancedCache.LRU;

namespace AdvancedCache.Tests;

public class CacheLRUTests
{
    [Fact]
    public void Cache_Devrait_Evincer_Le_Moins_Recemment_Utilise_Quand_Plein()
    {
        // ARRANGEMENT (On prépare le cache avec une taille de 2)
        var cache = new CacheLRU<string, string>(2);

        // ACT (On insère 3 éléments -> provoque une éviction)
        cache.Inserer("1", "Alice");
        cache.Inserer("2", "Bob");
        cache.Inserer("3", "Charlie"); // "1" (Alice) devrait être viré ici

        // ASSERT (On vérifie le résultat)
        Assert.Null(cache.Obtenir("1"));        // Alice doit avoir disparu
        Assert.Equal("Bob", cache.Obtenir("2")); // Bob doit être là
        Assert.Equal("Charlie", cache.Obtenir("3")); // Charlie doit être là
    }

    [Fact]
    public void Acceder_A_Un_Element_Devrait_Le_Rendre_Recent()
    {
        // ARRANGEMENT
        var cache = new CacheLRU<string, string>(2);
        cache.Inserer("1", "Alice");
        cache.Inserer("2", "Bob");

        // ACT : On lit "1" (Alice), elle devient la plus récente
        cache.Obtenir("1");

        // On insère un 3ème élément, c'est donc "2" (Bob) qui doit être viré
        cache.Inserer("3", "Charlie");

        // ASSERT
        Assert.Null(cache.Obtenir("2"));        // Bob a été viré
        Assert.Equal("Alice", cache.Obtenir("1")); // Alice est restée !
    }
    [Fact]
    public void CacheLRU_DevraitPasser_LeTestDeStressEtCycleDeVieComplet()
    {
        // 1. ARRANGEMENT : Initialisation d'un cache ultra-compact (Taille = 3)
        // Une petite taille permet de provoquer des évictions très rapidement.
        var cache = new CacheLRU<string, int>(3);

        // =================================================================
        // ÉTAPE 1 : Remplissage initial à saturation
        // =================================================================
        cache.Inserer("A", 10);
        cache.Inserer("B", 20);
        cache.Inserer("C", 30);

        Assert.Equal(3, cache.NombreElements);
        Assert.Equal(10, cache.Obtenir("A")); // Ordre de récence : B -> C -> A

        // =================================================================
        // ÉTAPE 2 : Mise à jour d'un élément existant (sans dépasser la capacité)
        // =================================================================
        cache.Inserer("B", 99); // Modifie B. Ordre de récence : C -> A -> B

        Assert.Equal(3, cache.NombreElements);
        Assert.Equal(99, cache.Obtenir("B"));

        // =================================================================
        // ÉTAPE 3 : Première éviction ( Provoquée par l'ajout d'un nouveau "D" )
        // Le moins récemment utilisé actuel est "C". Il doit disparaître.
        // =================================================================
        cache.Inserer("D", 40); // Ordre de récence : A -> B -> D

        Assert.Equal(3, cache.NombreElements);
        Assert.Equal(0, cache.Obtenir("C")); // Doit renvoyer 'default' (0 pour un int) car C a été évincé
        Assert.Equal(10, cache.Obtenir("A")); // A est toujours là

        // =================================================================
        // ÉTAPE 4 : Rafraîchissement par lecture et éviction consécutive
        // Actuellement, l'ordre est : B -> D -> A
        // Si on lit "B", il redevient le plus récent : D -> A -> B
        // =================================================================
        cache.Obtenir("B");

        // On ajoute "E". C'est "D" qui devrait sauter car il est en fin de chaîne.
        cache.Inserer("E", 50); // Ordre de récence : A -> B -> E

        Assert.Null(cache.Obtenir("D") == 40 ? "D aurait dû être supprimé" : null);
        Assert.Equal(0, cache.Obtenir("D")); // Vérification stricte : D est mort
        Assert.Equal(10, cache.Obtenir("A")); // A a survécu car protégé par son statut récent
        Assert.Equal(99, cache.Obtenir("B")); // B a survécu
        Assert.Equal(50, cache.Obtenir("E")); // E est présent

        // =================================================================
        // ÉTAPE 5 : Gestion des cas aux limites (Corner Cases)
        // =================================================================
        // Vérification qu'un élément inconnu renvoie bien la valeur par défaut
        Assert.Equal(0, cache.Obtenir("CLE_INCONNUE"));

        // Re-vérification de la taille finale inchangée
        Assert.Equal(3, cache.NombreElements);
    }
}
