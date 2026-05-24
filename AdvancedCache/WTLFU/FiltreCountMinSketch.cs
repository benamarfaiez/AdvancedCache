namespace AdvancedCache.WTLFU;

/// <summary>
/// Filtre d'admission Count-Min Sketch pour estimer la fréquence d'accès des clés.
/// Utilise 4 fonctions de hachage différentes pour minimiser les collisions.
/// Incorpore un mécanisme de vieillissement automatique.
/// </summary>
/// <remarks>
/// Le Count-Min Sketch est une structure de données probabiliste qui offre une estimation
/// conservative de la fréquence d'accès avec une consommation mémoire minimale.
/// </remarks>

public class FiltreCountMinSketch<K> where K : notnull
{
    private readonly byte[] _table;
    private readonly int _masque;
    private int _compteurOperations = 0;
    private readonly int _seuilVieillissement;

    public FiltreCountMinSketch(int capaciteMaximale)
    {
        // Puissance de 2 supérieure pour optimiser les modulos par un masque binaire (&)
        int taille = 1;
        while (taille < capaciteMaximale * 4) taille <<= 1;

        _table = new byte[taille];
        _masque = taille - 1;
        _seuilVieillissement = capaciteMaximale * 10;
    }

    public void Incrementer(K cle)
    {
        int hash = cle.GetHashCode();

        // Simulation de 4 fonctions de hachage par décalage binaire (MurmurHash style)
        for (int i = 0; i < 4; i++)
        {
            int index = (hash ^ (i * 0x5bd1e995)) & _masque;
            if (_table[index] < 15) _table[index]++; // Max 4-bits (valeur 15)
        }

        _compteurOperations++;
        if (_compteurOperations >= _seuilVieillissement)
        {
            VieillirCompteurs();
        }
    }

    public int EstimerFrequence(K cle)
    {
        int hash = cle.GetHashCode();
        int minFrequence = int.MaxValue;

        for (int i = 0; i < 4; i++)
        {
            int index = (hash ^ (i * 0x5bd1e995)) & _masque;
            if (_table[index] < minFrequence) minFrequence = _table[index];
        }

        return minFrequence;
    }

    private void VieillirCompteurs()
    {
        // Atténuation de la pollution historique : division par 2 de tous les scores
        for (int i = 0; i < _table.Length; i++)
        {
            _table[i] >>= 1;
        }
        _compteurOperations >>= 1;
    }
}