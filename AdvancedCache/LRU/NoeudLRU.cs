namespace AdvancedCache.LRU;

/// <summary>
/// Structure interne contiguë en mémoire pour le cache LRU ultime.
/// Utilise des index entiers (int) au lieu de pointeurs d'objets pour le chaînage.
/// </summary>
internal struct NoeudLRU<Tk, Tv>
{
    public Tk Cle;
    public Tv Valeur;
    public int Precedent; // Index du nœud précédent dans le tableau fixe
    public int Suivant;   // Index du nœud suivant dans le tableau fixe
    public bool EstOccupe;
}
