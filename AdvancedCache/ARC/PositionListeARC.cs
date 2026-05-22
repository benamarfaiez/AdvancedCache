namespace AdvancedCache.ARC;

/// <summary>
/// Spécifie l'emplacement d'un nœud dans la structure à 4 listes de l'ARC.
/// </summary>
internal enum PositionListeARC
{
    T1, // Récence (En RAM)
    B1, // Historique de récence (Fantôme / Clé seule)
    T2, // Fréquence (En RAM)
    B2  // Historique de fréquence (Fantôme / Clé seule)
}
