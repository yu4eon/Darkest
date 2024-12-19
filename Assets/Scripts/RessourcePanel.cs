using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RessourcePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _champQuantite;
    [SerializeField] Image _imageRessource;
    SOItems _typeItem;
    int _quantite;
    public void Init(SOItems type, int quantite)
    {
        // _type = type;
        _typeItem = type;
        _quantite = quantite;
        AfficherInfo();
    }

    void AfficherInfo()
    {
        _imageRessource.sprite = _typeItem.imgItem;
        _champQuantite.text = _quantite.ToString();
    }
}
