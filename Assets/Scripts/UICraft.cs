using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICraft : MonoBehaviour
{


    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] GameObject _prefabItem;
    [SerializeField] GameObject _prefabRecette;
    [SerializeField] RectTransform _conteneurRessources;
    [SerializeField] RectTransform _conteneurRecettes;
    [SerializeField] List<SORecette> _recettes = new();
    [SerializeField] SoundManager _soundManager;

    void Start()
    {
        _donneesPerso.evenementMiseAJour.AddListener(MettreAJourInventaire);
        InitialiserCraft();
        MettreAJourInventaire();
    }
    public void MettreAJourInventaire()
    {
        foreach (Transform child in _conteneurRessources)
        {
            Destroy(child.gameObject);
        }

        foreach (KeyValuePair<SOItems, int> ressource in _donneesPerso.dRessources)
        {
            GameObject panelItem = Instantiate(_prefabItem, _conteneurRessources);
            panelItem.GetComponent<item>().Init(ressource.Key);
        }
    }

    void InitialiserCraft()
    {
        foreach (SORecette recette in _recettes)
        {
            GameObject panelRecette = Instantiate(_prefabRecette, _conteneurRecettes);
            panelRecette.GetComponent<CraftItems>().Init(recette, _soundManager);

        }
    }

}