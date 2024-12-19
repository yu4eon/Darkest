using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Classe qui permet de définir les coûts en ressources pour la fabrication d'un item
/// </summary>
[System.Serializable]
public class CoutMaterial
{
    public SOItems typeItem; // Type de l'item
    public int quantiteItemRessource; // Quantité de l'item nécessaire pour la fabrication

}
