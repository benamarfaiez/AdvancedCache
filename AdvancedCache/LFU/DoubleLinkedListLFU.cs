namespace AdvancedCache.LFU
{
    public class DoubleLinkedListLFU<K, V> where K : notnull
    {
        private readonly NoeudLFU<K, V> _head;
        private readonly NoeudLFU<K, V> _tail;
        public int Taille { get; private set; } = 0;

        public DoubleLinkedListLFU()
        {
            // Nœuds sentinelles pour éviter de gérer les cas aux limites (null)
            _head = new NoeudLFU<K, V>(default!, default!);
            _tail = new NoeudLFU<K, V>(default!, default!);
            _head.Suivant = _tail;
            _tail.Precedent = _head;
        }

        public void AjouterEnTete(NoeudLFU<K, V> noeud)
        {
            noeud.Suivant = _head.Suivant;
            noeud.Precedent = _head;
            _head.Suivant!.Precedent = noeud;
            _head.Suivant = noeud;
            Taille++;
        }

        public void Retirer(NoeudLFU<K, V> noeud)
        {
            noeud.Precedent!.Suivant = noeud.Suivant;
            noeud.Suivant!.Precedent = noeud.Precedent;
            Taille--;
        }

        public NoeudLFU<K, V>? RetirerDernier()
        {
            if (Taille == 0) return null;
            var dernier = _tail.Precedent!;
            Retirer(dernier);
            return dernier;
        }
    }
}
