namespace AdvancedCache.LFU;

public class CacheLFU<K, V> : ICache<K, V> where K : notnull
{
    private readonly int _capacite;

    // Accès direct au nœud via sa clé
    private readonly Dictionary<K, NoeudLFU<K, V>> _nodeMap = [];

    // Regroupement des nœuds par fréquence
    private readonly Dictionary<int, DoubleLinkedListLFU<K, V>> _freqMap = [];

    private int _minFrequence = 0;

    public CacheLFU(int capacite)
    {
        if (capacite <= 0)
            throw new ArgumentException("La capacité doit être supérieure à 0.", nameof(capacite));
        _capacite = capacite;
    }

    public V? Obtenir(K cle)
    {
        if (!_nodeMap.TryGetValue(cle, out var noeud))
        {
            return default; // Ou lève une exception selon votre besoin
        }

        MettreAJourFrequence(noeud);
        return noeud.Valeur;
    }

    public void Inserer(K cle, V valeur)
    {
        if (_nodeMap.TryGetValue(cle, out var noeudExistant))
        {
            // Mise à jour de la valeur et de la fréquence
            noeudExistant.Valeur = valeur;
            MettreAJourFrequence(noeudExistant);
            return;
        }

        // Si le cache est plein, on doit faire de la place
        if (_nodeMap.Count >= _capacite)
        {
            EvincerMoinsFrequent();
        }

        // Insertion du nouveau nœud
        var nouveauNoeud = new NoeudLFU<K, V>(cle, valeur);
        _nodeMap[cle] = nouveauNoeud;

        // Un nouvel élément a toujours une fréquence de 1
        _minFrequence = 1;
        ObtenirOuCreerListe(1).AjouterEnTete(nouveauNoeud);
    }

    private void MettreAJourFrequence(NoeudLFU<K, V> noeud)
    {
        int ancienneFreq = noeud.Frequence;
        var ancienneListe = _freqMap[ancienneFreq];
        ancienneListe.Retirer(noeud);

        // Si la liste de la fréquence minimale devient vide, on incrémente la freq minimale globale
        if (ancienneFreq == _minFrequence && ancienneListe.Taille == 0)
        {
            _minFrequence++;
        }

        noeud.Frequence++;
        ObtenirOuCreerListe(noeud.Frequence).AjouterEnTete(noeud);
    }

    private void EvincerMoinsFrequent()
    {
        var listeMinFreq = _freqMap[_minFrequence];

        // Le dernier élément de la liste doublement chaînée est le "LRU" de cette fréquence
        var noeudAEvincer = listeMinFreq.RetirerDernier();

        if (noeudAEvincer != null)
        {
            _nodeMap.Remove(noeudAEvincer.Cle);
        }
    }

    private DoubleLinkedListLFU<K, V> ObtenirOuCreerListe(int frequence)
    {
        if (!_freqMap.TryGetValue(frequence, out var liste))
        {
            liste = new DoubleLinkedListLFU<K, V>();
            _freqMap[frequence] = liste;
        }
        return liste;
    }
}
