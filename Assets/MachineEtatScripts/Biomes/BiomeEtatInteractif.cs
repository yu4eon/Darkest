using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe pour l'état final du biome
/// </summary>
public class BiomeEtatInteractif : BiomesEtatsBase
{
    /// <summary>
    /// Initialisation de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void InitEtat(BiomesEtatsManager biome)
    {
        
    }

    /// <summary>
    /// Sortie de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void ExitEtat(BiomesEtatsManager biome)
    {

    }

    /// <summary>
    /// Mise à jour de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void UpdateEtat(BiomesEtatsManager biome)
    {

    }

    /// <summary>
    /// Collision avec un autre objet
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui se collisionne avec le cube</param>
    public override void TriggerEnterEtat(BiomesEtatsManager biome, Collider other)
    {

    }
    /// <summary>
    /// Objet qui reste en collision avec le biome
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui reste en collision avec le cube</param>
    public override void TriggerStayEtat(BiomesEtatsManager biome, Collider other)
    {
        
    }   

    /// <summary>
    /// Objet qui sort de la collision avec le biome
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui sort de la collision avec le cube</param>
    public override void TriggerExitEtat(BiomesEtatsManager biome, Collider other)
    {

    }
}
