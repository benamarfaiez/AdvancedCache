namespace AdvancedCache.ARC;

internal class DoubleLinkedListARC<K, V> where K : notnull
{
    public NoeudARC<K, V>? Tete { get; private set; }
    public NoeudARC<K, V>? Queue { get; private set; }
    public int Taille { get; private set; }

    public void PlacerEnTete(NoeudARC<K, V> noeud)
    {
        noeud.Suivant = Tete;
        noeud.Precedent = null;

        Tete?.Precedent = noeud;
        Tete = noeud;

        Queue ??= noeud;
        Taille++;
    }

    public void Extraire(NoeudARC<K, V> noeud)
    {
        if (noeud.Precedent != null) noeud.Precedent.Suivant = noeud.Suivant;
        else Tete = noeud.Suivant;

        if (noeud.Suivant != null) noeud.Suivant.Precedent = noeud.Precedent;
        else Queue = noeud.Precedent;

        noeud.Precedent = null;
        noeud.Suivant = null;
        Taille--;
    }
}
