using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftItems", menuName = "ItemsCrafting")]

public class SOCraftItems : ScriptableObject
{
    [Header("Objet info")]
    [SerializeField] string _nom = "Nom de l'objet";
    [SerializeField] Sprite _imgItemCraft;
    [SerializeField] Sprite _imgItemRessource1;
    [SerializeField] int _quantiteItemRessource1 = 1;
    [SerializeField] Sprite _imgItemRessource2;
    [SerializeField] int _quantiteItemRessource2 = 1;

    public string nom { get => _nom;  set => _nom = value; }
    public Sprite imgItemCraft { get => _imgItemCraft; set => _imgItemCraft = value; }
    public Sprite imgItemRessource1 { get => _imgItemRessource1; set => _imgItemRessource1 = value; }
    public int quantiteItemRessource1 { get => _quantiteItemRessource1; set => _quantiteItemRessource1 = value; }
    public Sprite imgItemRessource2 { get => _imgItemRessource2; set => _imgItemRessource2 = value; }
    public int quantiteItemRessource2 { get => _quantiteItemRessource2; set => _quantiteItemRessource2 = value; }

}
