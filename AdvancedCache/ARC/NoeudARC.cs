namespace AdvancedCache.ARC;

/// <summary>
/// Représente un maillon doublement chaîné au sein de l'architecture ARC.
/// </summary>
internal class NoeudARC<K, V>
{
    public K Cle { get; }
    public V? Valeur { get; set; }
    public PositionListeARC ListeActuelle { get; set; }

    // Pointeurs de chaînage bas niveau
    public NoeudARC<K, V>? Precedent { get; set; }
    public NoeudARC<K, V>? Suivant { get; set; }

    public NoeudARC(K cle, V? valeur, PositionListeARC liste)
    {
        Cle = cle;
        Valeur = valeur;
        ListeActuelle = liste;
    }
}
