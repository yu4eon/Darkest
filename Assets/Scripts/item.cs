using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class item : MonoBehaviour
{
    [Header("Les Scriptable Objects")]
    SOItems _item; 

    [SerializeField] SOPerso _donneesPerso; 

    [Header("Les conteneur d'information")]
    [SerializeField] TextMeshProUGUI _champNom; 
    [SerializeField] TextMeshProUGUI _champQuantity; 
    [SerializeField] Image _imageItem; 
    [SerializeField] Image _imageItemLowAlpha; 




    public void Init(SOItems item)
    {
        _item = item;
        AfficherInfo();
    }


    void AfficherInfo()
    {
        _imageItemLowAlpha.sprite = _item.imgItem;
        _champNom.text = _item.typeRessource.ToString();
        // _champQuantity.text = _item.quantite.ToString();
        _imageItem.sprite = _item.imgItem;
        if(_donneesPerso.dRessources[_item] == 0)
        {
            _imageItem.color = new Color(1, 1, 1, 0.5f);
        }
        _champQuantity.text = _donneesPerso.dRessources[_item].ToString();

    }

}
