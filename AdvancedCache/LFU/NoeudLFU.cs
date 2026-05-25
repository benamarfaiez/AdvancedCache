namespace AdvancedCache.LFU;

public class NoeudLFU<K, V>(K cle, V valeur) where K : notnull
{
    public K Cle { get; } = cle;
    public V Valeur { get; set; } = valeur;
    public int Frequence { get; set; } = 1;

    // Pointeurs pour la liste doublement chaînée
    public NoeudLFU<K, V>? Precedent { get; set; }
    public NoeudLFU<K, V>? Suivant { get; set; }
}
