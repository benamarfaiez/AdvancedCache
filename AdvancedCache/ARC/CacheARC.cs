using System.Runtime.CompilerServices;

namespace AdvancedCache.ARC;

public sealed class CacheARC<K, V> : ICache<K, V> where K : notnull
{
    private readonly int _c; // Capacité cible du cache (RAM)
    private double _p;       // Paramètre d'adaptation dynamique (Taille cible de T1)

    private readonly Dictionary<K, NoeudARC<K, V>> _dictionnaire;
    private readonly Lock _lock = new();

    // Composants spécialisés et isolés (SRP)
    private readonly DoubleLinkedListARC<K, V> _t1 = new();
    private readonly DoubleLinkedListARC<K, V> _b1 = new();
    private readonly DoubleLinkedListARC<K, V> _t2 = new();
    private readonly DoubleLinkedListARC<K, V> _b2 = new();

    public CacheARC(int capacite)
    {
        if (capacite <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacite), "La capacité du cache doit être supérieure à zéro.");
        }

        _c = capacite;
        _p = 0.0;
        _dictionnaire = new Dictionary<K, NoeudARC<K, V>>(capacite * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public V? Obtenir(K cle)
    {
        ArgumentNullException.ThrowIfNull(cle);

        lock (_lock)
        {
            if (!_dictionnaire.TryGetValue(cle, out var noeud))
            {
                return default; // Cache Miss total
            }

            // Cache Hit : L'élément est en RAM (T1 ou T2)
            if (noeud.ListeActuelle == PositionListeARC.T1 || noeud.ListeActuelle == PositionListeARC.T2)
            {
                ExtraireDeSaListeActuelle(noeud);
                noeud.ListeActuelle = PositionListeARC.T2;
                _t2.PlacerEnTete(noeud);
                return noeud.Valeur;
            }

            // Cache Hit dans l'historique (B1 ou B2) : Ajustement dynamique de '_p'
            if (noeud.ListeActuelle == PositionListeARC.B1)
            {
                double delta = _b1.Taille >= _b2.Taille ? 1.0 : (double)_b2.Taille / _b1.Taille;
                _p = Math.Min(_c, _p + delta);
                ExecuterAlgorithmeDeRemplacement(noeud);
            }
            else
            {
                double delta = _b2.Taille >= _b1.Taille ? 1.0 : (double)_b1.Taille / _b2.Taille;
                _p = Math.Max(0.0, _p - delta);
                ExecuterAlgorithmeDeRemplacement(noeud);
            }

            // Migration de l'historique fantôme vers la RAM (T2)
            ExtraireDeSaListeActuelle(noeud);
            noeud.ListeActuelle = PositionListeARC.T2;
            _t2.PlacerEnTete(noeud);

            return noeud.Valeur;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inserer(K cle, V valeur)
    {
        ArgumentNullException.ThrowIfNull(cle);

        lock (_lock)
        {
            // Si la clé existe déjà, on met à jour et on déclenche la logique de récence
            if (_dictionnaire.TryGetValue(cle, out var noeudExistant))
            {
                noeudExistant.Valeur = valeur;
                Obtenir(cle);
                return;
            }

            var nouveauNoeud = new NoeudARC<K, V>(cle, valeur, PositionListeARC.T1);
            int l1Taille = _t1.Taille + _b1.Taille;
            int l2Taille = _t2.Taille + _b2.Taille;

            if (l1Taille == _c)
            {
                if (_t1.Taille < _c)
                {
                    EvincerLePlusAncienDeLaListe(_b1);
                    ExecuterAlgorithmeDeRemplacement(nouveauNoeud);
                }
                else
                {
                    EvincerLePlusAncienDeLaListe(_t1);
                }
            }
            else if (l1Taille < _c && (l1Taille + l2Taille >= _c))
            {
                if (l1Taille + l2Taille == 2 * _c)
                {
                    EvincerLePlusAncienDeLaListe(_b2);
                }
                ExecuterAlgorithmeDeRemplacement(nouveauNoeud);
            }

            // Enregistrement sécurisé du nouveau nœud
            _dictionnaire[cle] = nouveauNoeud;
            _t1.PlacerEnTete(nouveauNoeud);
        }
    }

    private void ExecuterAlgorithmeDeRemplacement(NoeudARC<K, V> noeud)
    {
        bool conditionT1 = _t1.Taille > 0 && ((noeud.ListeActuelle == PositionListeARC.B2 && _t1.Taille == (int)_p) || (_t1.Taille > (int)_p));

        if (conditionT1)
        {
            var mruT1 = _t1.Queue!;
            _t1.Extraire(mruT1);
            mruT1.Valeur = default; // Libère la RAM (GC-Friendly)
            mruT1.ListeActuelle = PositionListeARC.B1;
            _b1.PlacerEnTete(mruT1);
        }
        else if (_t2.Taille > 0)
        {
            var mfuT2 = _t2.Queue!;
            _t2.Extraire(mfuT2);
            mfuT2.Valeur = default; // Libère la RAM (GC-Friendly)
            mfuT2.ListeActuelle = PositionListeARC.B2;
            _b2.PlacerEnTete(mfuT2);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ExtraireDeSaListeActuelle(NoeudARC<K, V> noeud)
    {
        switch (noeud.ListeActuelle)
        {
            case PositionListeARC.T1: _t1.Extraire(noeud); break;
            case PositionListeARC.B1: _b1.Extraire(noeud); break;
            case PositionListeARC.T2: _t2.Extraire(noeud); break;
            case PositionListeARC.B2: _b2.Extraire(noeud); break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EvincerLePlusAncienDeLaListe(DoubleLinkedListARC<K, V> liste)
    {
        if (liste.Queue != null)
        {
            var cible = liste.Queue;
            liste.Extraire(cible);
            _dictionnaire.Remove(cible.Cle);
            cible.Valeur = default;
        }
    }
}