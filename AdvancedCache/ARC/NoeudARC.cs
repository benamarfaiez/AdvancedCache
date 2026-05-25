namespace AdvancedCache.ARC;

/// <summary>
/// Représente un maillon doublement chaîné au sein de l'architecture ARC.
/// </summary>
internal class NoeudARC<K, V>(K cle, V? valeur, PositionListeARC liste)
{
    public K Cle { get; } = cle;
    public V? Valeur { get; set; } = valeur;
    public PositionListeARC ListeActuelle { get; set; } = liste;

    // Pointeurs de chaînage bas niveau
    public NoeudARC<K, V>? Precedent { get; set; }
    public NoeudARC<K, V>? Suivant { get; set; }

}
