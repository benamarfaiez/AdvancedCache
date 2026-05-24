namespace AdvancedCache.WTLFU;

/// <summary>
/// Cache en mémoire ultra-performant basé sur l'algorithme W-TinyLFU (Window TinyLFU).
/// Offre une complexité O(1) pour toutes les opérations (Obtenir, Inserer, Éviction).
/// Zéro allocation de listes chaînées grâce à des pointeurs internes.
/// </summary>
/// <remarks>
/// W-TinyLFU divise le cache en 3 zones:
/// - Window: Zone petite (1% de la capacité) pour les éléments récents.
/// - Probation: Zone médiane pour les éléments testés.
/// - Protected: Zone principale pour les éléments avec une fréquence d'accès élevée.
/// 
/// Lors d'une éviction, un "duel de fréquence" s'opère entre l'élément éjecté
/// de la zone Window et le moins récemment utilisé de la zone Probation.
/// </remarks>

public class CacheWTinyLfu<K, V> : ICache<K, V> where K : notnull
{
    private readonly int _capaciteMaximale;
    private readonly int _limiteFenetre;
    private readonly int _limiteProtegee;

    private readonly Dictionary<K, NoeudTinyLfu<K, V>> _indexGlobal = new();
    private readonly FiltreCountMinSketch<K> _filtreFrequence;

    private readonly ListeDoublementChaineeCustom<K, V> _listeFenetre = new();
    private readonly ListeDoublementChaineeCustom<K, V> _listeProbation = new();
    private readonly ListeDoublementChaineeCustom<K, V> _listeProtegee = new();

    public CacheWTinyLfu(int capaciteMaximale)
    {
        if (capaciteMaximale < 10)
            throw new ArgumentException("La capacité minimale pour W-TinyLFU est de 10 éléments.", nameof(capaciteMaximale));

        _capaciteMaximale = capaciteMaximale;

        // Répartition classique de l'architecture : 1% Fenêtre, 20% Probation, 79% Protégée
        _limiteFenetre = Math.Max(1, capaciteMaximale / 100);
        _limiteProtegee = (int)((capaciteMaximale - _limiteFenetre) * 0.8);

        _filtreFrequence = new FiltreCountMinSketch<K>(capaciteMaximale);
    }

    public V? Obtenir(K cle)
    {
        if (!_indexGlobal.TryGetValue(cle, out var noeud))
        {
            return default;
        }

        _filtreFrequence.Incrementer(cle);
        MettreAJourRécence(noeud);

        return noeud.Valeur;
    }

    public void Inserer(K cle, V valeur)
    {
        if (_indexGlobal.TryGetValue(cle, out var noeudExistant))
        {
            noeudExistant.Valeur = valeur;
            _filtreFrequence.Incrementer(cle);
            MettreAJourRécence(noeudExistant);
            return;
        }

        var nouveauNoeud = new NoeudTinyLfu<K, V>(cle, valeur);
        _indexGlobal[cle] = nouveauNoeud;

        _listeFenetre.AjouterEnTete(nouveauNoeud);
        _filtreFrequence.Incrementer(cle);

        // Si la zone de transition (Fenetre) déborde, on pousse vers le Main Cache
        if (_listeFenetre.Taille > _limiteFenetre)
        {
            var candidat = _listeFenetre.RetirerDernier()!;
            ArbitrerAdmission(candidat);
        }
    }

    private void MettreAJourRécence(NoeudTinyLfu<K, V> noeud)
    {
        if (noeud.ZoneActuelle == ZoneCache.Fenetre)
        {
            _listeFenetre.Retirer(noeud);
            _listeFenetre.AjouterEnTete(noeud);
        }
        else if (noeud.ZoneActuelle == ZoneCache.Probation)
        {
            _listeProbation.Retirer(noeud);
            noeud.ZoneActuelle = ZoneCache.Protegee;
            _listeProtegee.AjouterEnTete(noeud);

            // Si la zone protégée déborde, le maillon faible descend en probation
            if (_listeProtegee.Taille > _limiteProtegee)
            {
                var retrograde = _listeProtegee.RetirerDernier()!;
                retrograde.ZoneActuelle = ZoneCache.Probation;
                _listeProbation.AjouterEnTete(retrograde);
            }
        }
        else // Zone Protegee
        {
            _listeProtegee.Retirer(noeud);
            _listeProtegee.AjouterEnTete(noeud);
        }
    }

    private void ArbitrerAdmission(NoeudTinyLfu<K, V> candidat)
    {
        int tailleMainActuelle = _listeProbation.Taille + _listeProtegee.Taille;
        int limiteMainCache = _capaciteMaximale - _limiteFenetre;

        if (tailleMainActuelle < limiteMainCache)
        {
            candidat.ZoneActuelle = ZoneCache.Probation;
            _listeProbation.AjouterEnTete(candidat);
            return;
        }

        // Duel d'admission TinyLFU entre le candidat et la victime (le LRU de la probation)
        var victime = _listeProbation.RegarderDernier();
        if (victime == null)
        {
            _indexGlobal.Remove(candidat.Cle);
            return;
        }

        int freqCandidat = _filtreFrequence.EstimerFrequence(candidat.Cle);
        int freqVictime = _filtreFrequence.EstimerFrequence(victime.Cle);

        if (freqCandidat > freqVictime)
        {
            _listeProbation.RetirerDernier();
            _indexGlobal.Remove(victime.Cle);

            candidat.ZoneActuelle = ZoneCache.Probation;
            _listeProbation.AjouterEnTete(candidat);
        }
        else
        {
            // Le candidat perd le duel et est directement écarté du cache
            _indexGlobal.Remove(candidat.Cle);
        }
    }
}
