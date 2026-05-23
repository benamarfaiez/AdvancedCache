namespace AdvancedCache.LFU;

public class NoeudLFU<K, V> where K : notnull
{
    public K Cle { get; }
    public V Valeur { get; set; }
    public int Frequence { get; set; } = 1;

    // Pointeurs pour la liste doublement chaînée
    public NoeudLFU<K, V>? Precedent { get; set; }
    public NoeudLFU<K, V>? Suivant { get; set; }

    public NoeudLFU(K cle, V valeur)
    {
        Cle = cle;
        Valeur = valeur;
    }
}
