namespace AdvancedCache.WTLFU;

public class NoeudTinyLfu<K, V> where K : notnull
{
    public K Cle { get; }
    public V Valeur { get; set; }
    public ZoneCache ZoneActuelle { get; set; }

    // Pointeurs pour la liste doublement chaînée en O(1)
    public NoeudTinyLfu<K, V>? Precedent { get; set; }
    public NoeudTinyLfu<K, V>? Suivant { get; set; }

    public NoeudTinyLfu(K cle, V valeur)
    {
        Cle = cle;
        Valeur = valeur;
        ZoneActuelle = ZoneCache.Fenetre;
    }
}

public class ListeDoublementChaineeCustom<K, V> where K : notnull
{
    private readonly NoeudTinyLfu<K, V> _head;
    private readonly NoeudTinyLfu<K, V> _tail;
    public int Taille { get; private set; } = 0;

    public ListeDoublementChaineeCustom()
    {
        _head = new NoeudTinyLfu<K, V>(default!, default!);
        _tail = new NoeudTinyLfu<K, V>(default!, default!);
        _head.Suivant = _tail;
        _tail.Precedent = _head;
    }

    public void AjouterEnTete(NoeudTinyLfu<K, V> noeud)
    {
        noeud.Suivant = _head.Suivant;
        noeud.Precedent = _head;
        _head.Suivant!.Precedent = noeud;
        _head.Suivant = noeud;
        Taille++;
    }

    public void Retirer(NoeudTinyLfu<K, V> noeud)
    {
        if (noeud.Precedent != null && noeud.Suivant != null)
        {
            noeud.Precedent.Suivant = noeud.Suivant;
            noeud.Suivant.Precedent = noeud.Precedent;
            noeud.Precedent = null;
            noeud.Suivant = null;
            Taille--;
        }
    }

    public NoeudTinyLfu<K, V>? RetirerDernier()
    {
        if (Taille == 0) return null;
        var dernier = _tail.Precedent!;
        Retirer(dernier);
        return dernier;
    }

    public NoeudTinyLfu<K, V>? RegarderDernier()
    {
        return Taille == 0 ? null : _tail.Precedent;
    }
}
