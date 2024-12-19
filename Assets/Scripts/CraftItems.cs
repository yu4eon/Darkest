using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItems : MonoBehaviour
{

    // [Header("Les Scriptable Objects")]
    // [SerializeField] SOCraftItems _CraftItems; 
    // [SerializeField] SOPerso _donnePerso; 

    SORecette _recette;

    // [Header("Les conteneur d'information")]
    [SerializeField] TextMeshProUGUI _champNom;
    [SerializeField] Image _imageCraftItem;
    [SerializeField] RectTransform _conteneurRessources;
    [SerializeField] GameObject _prefabRessources;
    [SerializeField] Button _boutonCraft;
    [SerializeField] SOPerso _donnePerso;
    [SerializeField] AudioClip _sonCraft;
    SoundManager _soundManager;
    // [SerializeField] Image _imageRessource1; 
    // [SerializeField] TextMeshProUGUI _champQuantity1; 
    // [SerializeField] Image _imageRessource2; 
    // [SerializeField] TextMeshProUGUI _champQuantity2; 

    public void Init(SORecette recette, SoundManager soundManager)
    {
        _recette = recette;
        _soundManager = soundManager;
        AfficherInfo();
        // _CraftItems = recette;
        // AfficherInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        // AfficherInfo();
    }

    void AfficherInfo()
    {
        _champNom.text = _recette.nomRecette;
        _imageCraftItem.sprite = _recette.imgRecette;
        _boutonCraft.onClick.AddListener(() => Fabriquer());
        foreach (Transform child in _conteneurRessources)
        {
            Destroy(child.gameObject);
        }
        foreach (CoutMaterial cout in _recette.dCoutsRessources)
        {
            GameObject panelRessource = Instantiate(_prefabRessources.gameObject, _conteneurRessources);
            panelRessource.GetComponent<RessourcePanel>().Init(cout.typeItem, cout.quantiteItemRessource);
        }
    }

    void Fabriquer()
    {

        if (_donnePerso.FabriquerObjet(_recette))
        {
            Debug.Log("Craft");
            _soundManager.JouerSon(_sonCraft);
            if (_recette.prochaineRecette != null)
            {
                _recette = _recette.prochaineRecette;
                AfficherInfo();
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }



}
