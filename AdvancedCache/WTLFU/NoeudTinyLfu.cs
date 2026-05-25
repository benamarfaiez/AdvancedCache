namespace AdvancedCache.WTLFU;

public class NoeudTinyLfu<K, V>(K cle, V valeur) where K : notnull
{
    public K Cle { get; } = cle;
    public V Valeur { get; set; } = valeur;
    public ZoneCache ZoneActuelle { get; set; } = ZoneCache.Fenetre;

    // Pointeurs pour la liste doublement chaînée en O(1)
    public NoeudTinyLfu<K, V>? Precedent { get; set; }
    public NoeudTinyLfu<K, V>? Suivant { get; set; }
}