namespace AdvancedCache;

public interface ICache<K, V> where K : notnull
{
    V? Obtenir(K cle);
    void Inserer(K cle, V valeur);
}
