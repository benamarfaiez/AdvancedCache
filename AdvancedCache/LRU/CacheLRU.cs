using System.Runtime.CompilerServices;

namespace AdvancedCache.LRU;

public class CacheLRU<K, V> where K : IEquatable<K>
{
    private readonly NoeudLRU<K, V>[] _indices;
    private readonly int _capacite;
    private int _tete = -1;
    private int _queue = -1;
    private int _taille = 0;

    public CacheLRU(int capacite)
    {
        _capacite = capacite;
        // On dimensionne le tableau à une taille fixe (puissance de 2 pour un hachage ultra-rapide)
        _indices = new NoeudLRU<K, V>[capacite * 2];

        for (int i = 0; i < _indices.Length; i++)
        {
            _indices[i].Precedent = -1;
            _indices[i].Suivant = -1;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculerIndex(K cle)
    {
        // Masquage binaire ultra-rapide au lieu d'un modulo classique
        return Math.Abs(cle.GetHashCode()) % _indices.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public V? Obtenir(K cle)
    {
        int index = TrouverIndex(cle);
        if (index == -1) return default;

        // Rendre le nœud le plus récent (on le déplace en tête)
        Detacher(index);
        PlacerEnTete(index);

        return _indices[index].Valeur;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inserer(K cle, V valeur)
    {
        int index = TrouverIndex(cle);

        if (index != -1)
        {
            // Mise à jour de la valeur existante
            _indices[index].Valeur = valeur;
            Detacher(index);
            PlacerEnTete(index);
            return;
        }

        // Si le cache est plein, on vire le plus ancien (la queue)
        if (_taille >= _capacite)
        {
            int ancienIndex = _queue;
            Detacher(ancienIndex);
            _indices[ancienIndex].EstOccupe = false;
            _taille--;
        }

        // Insertion du nouveau nœud (Sondage linéaire pour trouver une place vide)
        int hachage = CalculerIndex(cle);
        while (_indices[hachage].EstOccupe)
        {
            hachage = (hachage + 1) % _indices.Length;
        }

        _indices[hachage].Cle = cle;
        _indices[hachage].Valeur = valeur;
        _indices[hachage].EstOccupe = true;

        PlacerEnTete(hachage);
        _taille++;
    }

    // --- Méthodes privées de manipulation d'index (Pointeurs virtuels) ---

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int TrouverIndex(K cle)
    {
        int hachage = CalculerIndex(cle);
        int depart = hachage;

        while (_indices[hachage].EstOccupe)
        {
            if (_indices[hachage].Cle.Equals(cle)) return hachage;
            hachage = (hachage + 1) % _indices.Length;
            if (hachage == depart) break;
        }
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Detacher(int index)
    {
        if (_indices[index].Precedent != -1)
            _indices[_indices[index].Precedent].Suivant = _indices[index].Suivant;
        else
            _tete = _indices[index].Suivant;

        if (_indices[index].Suivant != -1)
            _indices[_indices[index].Suivant].Precedent = _indices[index].Precedent;
        else
            _queue = _indices[index].Precedent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PlacerEnTete(int index)
    {
        _indices[index].Suivant = _tete;
        _indices[index].Precedent = -1;

        if (_tete != -1)
            _indices[_tete].Precedent = index;

        _tete = index;

        if (_queue == -1)
            _queue = index;
    }
    public int NombreElements
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _taille;
    }
}