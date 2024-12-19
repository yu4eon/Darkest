using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// ScriptableObject qui permet de gérer les données du personnage
/// </summary>
[CreateAssetMenu(fileName = "Nouveau Perso", menuName = "ScriptableObject/Perso")]
public class SOPerso : ScriptableObject
{
    [SerializeField] int _niveauIni = 1; // Niveau initial du personnage
    [SerializeField] int _niveau = 1; // Niveau du personnage
    public int niveau // Getters et Setters
    {
        get { return _niveau; }
        set
        {
            _niveau = Mathf.Clamp(value, 1, int.MaxValue);
            _evenementMiseAJour.Invoke();
        }
    }
    [SerializeField] float _sanite = 100; // Sanité du personnage
    [SerializeField] float _saniteIni = 100; // Sanité initiale du personnage
    public float sanite // Getters et Setters
    {
        get { return _sanite; }
        set
        {
            _sanite = Mathf.Clamp(value, 0, _saniteIni);
            _evenementMiseAJour.Invoke();
        }
    }

    [SerializeField] int _niveauHuileIni = 100; // Niveau d'huile initial de la lanterne
    [SerializeField] int _niveauHuile = 100; // Niveau d'huile de la lanterne
    public int niveauHuile // Getters et Setters
    {
        get { return _niveauHuile; }
        set
        {
            _niveauHuile = Mathf.Clamp(value, 0, _niveauHuileIni);
            _evenementMiseAJour.Invoke();
        }
    }
    [SerializeField] SOItems[] _lItems; // Liste des ressources récupérables
    Dictionary<SOItems, int> _dRessources = new(); // Dictionnaire des ressources du personnage
    public Dictionary<SOItems, int> dRessources // Getters et Setters
    {
        get { return _dRessources; } 
        set { _dRessources = value; }
    }

    Dictionary<int, bool> _dRunes = new(); // Dictionnaire des runes du personnage
    public Dictionary<int, bool> dRunes // Getters et Setters
    {
        get { return _dRunes; }
        set { _dRunes = value; }
    }

    int _nbAutelsRestants = 5; // Nombre d'autels restants
    public int nbAutelsRestants // Getters et Setters
    {
        get { return _nbAutelsRestants; }
        set { _nbAutelsRestants = value; }
    }

    [SerializeField] float _rangeDetectionIni = 15f; // Portée de détection initiale de la radio
    float _rangeDetection; // Portée de détection de la radio
    public float rangeDetection // Getters et Setters
    {
        get { return _rangeDetection; }
        set { _rangeDetection = value; }
    }
    int _niveauRadio = 1; // Niveau de la radio
    public int niveauRadio // Getters et Setters
    {
        get { return _niveauRadio; }
        set { _niveauRadio = value; }
    }

    float distanceMinRune; // Distance de la rune la plus proche
    public float DistanceMinRune  // Getters et Setters
    {
        get { return distanceMinRune; }
        set { distanceMinRune = value; }
    } 

    [SerializeField] int _coutAttaqueIni = 10; // Coût d'attaque initial de la lanterne
    int _coutAttaque; // Coût d'attaque de la lanterne
    public int coutAttaque // Getters et Setters
    {
        get { return _coutAttaque; }
        set { _coutAttaque = value; }
    }
    [SerializeField] float _rayonIni = 8f; // Rayon initial de la lanterne
    float _rayon; // Rayon de la lanterne
    public float rayon // Getters et Setters
    {
        get { return _rayon; }
        set { _rayon = value; }
    }
    int _niveauLanterne = 1; // Niveau de la lanterne
    int _biomeActuel = 0; // Biome actuel
    public int biomeActuel // Getters et Setters
    {
        get { return _biomeActuel; }
        set { _biomeActuel = value; }
    }

    bool _possedeAmulette = false; // Possède une amulette
    public bool possedeAmulette // Getters et Setters
    {
        get { return _possedeAmulette; }
        set { _possedeAmulette = value; }
    }

    UnityEvent _evenementMiseAJour = new UnityEvent(); // Événement de mise à jour
    public UnityEvent evenementMiseAJour // Getters et Setters
    {
        get { return _evenementMiseAJour; }
        set { _evenementMiseAJour = value; }
    }

    UnityEvent _evenementInstantiateFeuDeCamp = new UnityEvent(); // Événement d'instanciation du feu de camp
    public UnityEvent evenementInstantiateFeuDeCamp // Getters et Setters
    {
        get { return _evenementInstantiateFeuDeCamp; }
        set { _evenementInstantiateFeuDeCamp = value; }
    }


    public void ResetNiveau()
    {
        _niveau = _niveauIni;
    }
    public void Initialiser()
    {
        _sanite = _saniteIni;
        _niveauLanterne = 1;
        _niveauRadio = 1;
        _possedeAmulette = false;
        AjusterRangeLanterne();
        AjusterRangeRadio();


        _rangeDetection = _rangeDetectionIni;
        _niveauHuile = _niveauHuileIni;
        _nbAutelsRestants = 5;
        _dRessources.Clear();
        _dRunes.Clear();
        foreach (SOItems ressource in _lItems)
        {
            _dRessources.Add(ressource, 0);
            // Debug.Log(_dRessources[ressource]);
        }

        for (int i = 0; i < 5; i++)
        {
            _dRunes.Add(i, false);
            // Debug.Log(_dRunes);
        }
    }

    public void AjusterRangeLanterne()
    {
        switch (_niveauLanterne)
        {
            case 1:
                _rayon = _rayonIni;
                break;
            case 2:
                _rayon = _rayonIni * 1.5f;
                break;
            case 3:
                _rayon = _rayonIni * 2f;
                break;
            default:
                _rayon = _rayonIni;
                break;
        }
    }

    public void AjusterRangeRadio()
    {
        switch (_niveauRadio)
        {
            case 1:
                _rangeDetection = _rangeDetectionIni;
                _coutAttaque = Mathf.RoundToInt(_coutAttaqueIni);
                break;
            case 2:
                _rangeDetection = _rangeDetectionIni * 1.8f;
                _coutAttaque = Mathf.RoundToInt(_coutAttaqueIni / 1.5f);
                break;
            case 3:
                _rangeDetection = _rangeDetectionIni * 2.5f;
                _coutAttaque = Mathf.RoundToInt(_coutAttaqueIni / 2f);
                break;
            default:
                _rangeDetection = _rangeDetectionIni;
                _coutAttaque = Mathf.RoundToInt(_coutAttaqueIni);
                break;
        }
    }

    public void AjouterRessource(SOItems ressource, int quantite)
    {
        if (_dRessources.ContainsKey(ressource))
        {
            _dRessources[ressource] += quantite;
            _evenementMiseAJour.Invoke();
        }
    }

    public bool FabriquerObjet(SORecette recette)
    {
        List<CoutMaterial> _lRessourcesEnleve = new();
        foreach (CoutMaterial cout in recette.dCoutsRessources)
        {
            if (_dRessources.ContainsKey(cout.typeItem) && _dRessources[cout.typeItem] >= cout.quantiteItemRessource)
            {
                _lRessourcesEnleve.Add(cout);
            }
            else
            {
                Debug.Log("Pas assez de ressources");
                return false;
            }
        }
        foreach (CoutMaterial cout in _lRessourcesEnleve)
        {
            _dRessources[cout.typeItem] -= cout.quantiteItemRessource;
        }

        if (recette.typeItem == TypeItem.Radio)
        {
            _niveauRadio++;
            AjusterRangeRadio();
            Debug.Log("Range amélioré :" + _rangeDetection);
        }
        else if (recette.typeItem == TypeItem.Lanterne)
        {
            _niveauLanterne++;
            AjusterRangeLanterne();
            Debug.Log("Range amélioré :" + _rayon);
        }
        else if (recette.typeItem == TypeItem.FeuDeCamp)
        {
            _evenementInstantiateFeuDeCamp.Invoke();

        }
        else if (recette.typeItem == TypeItem.Amulette)
        {
            _possedeAmulette = true;
        }
        return true;
        
    }
}
