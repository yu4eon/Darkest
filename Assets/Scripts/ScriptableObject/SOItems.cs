using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ScriptableObject qui permet de définir les ressources récupérables dans le jeu
/// </summary>
[CreateAssetMenu(fileName = "Objet", menuName = "ItemsInvontory")]
public class SOItems : ScriptableObject
{
    [Header("Objet info")]
    [SerializeField] TypeRessource _typeRessource; // Type de l'item
    [SerializeField] string _nom = "Nom de l'objet"; // Nom de l'item
    [SerializeField] Sprite _imgItem; // Image de l'item


    // Getters et Setters
    public TypeRessource typeRessource { get => _typeRessource; set => _typeRessource = value; } 
    public string nom { get => _nom;  set => _nom = value; }
    public Sprite imgItem { get => _imgItem; set => _imgItem = value; }
}
