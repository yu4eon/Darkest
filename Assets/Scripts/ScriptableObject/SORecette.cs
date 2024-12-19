using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nouvelle Recette", menuName = "ScriptableObject/Recette")]
public class SORecette : ScriptableObject
{
    [SerializeField] string _nomRecette;
    public string nomRecette
    {
        get { return _nomRecette; }
    }
    [SerializeField] TypeItem _typeItem;
    public TypeItem typeItem
    {
        get { return _typeItem; }
    }
    [SerializeField] Sprite _imgRecette;
    public Sprite imgRecette
    {
        get { return _imgRecette; }
    }
    [SerializeField] SORecette _prochaineRecette;
    public SORecette prochaineRecette
    {
        get { return _prochaineRecette; }
    }
    public List<CoutMaterial> dCoutsRessources = new();
}
