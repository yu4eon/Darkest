using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour
{
    [SerializeField] SOItems _type; 
    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] ParticleSystem _particules;
    [SerializeField] int _valeur;
    [SerializeField] Renderer[] _tableauRenderer;
    GameObject _perso;
    BiomesEtatsManager _biome;
    List<Material> _listeOutline = new();
    [SerializeField] float _scaleOutline = 1.1f;
    [SerializeField] float _rangeDetection = 1.0f;
    bool _estProche = false;
    // bool _est

    public void Init(GameObject perso, BiomesEtatsManager biome)
    {
        _perso = perso;
        _biome = biome;
        // Debug.Log(_player);
    }

    void Awake()
    {
        // _renderer = GetComponent<Renderer>();

        foreach (Renderer rend in _tableauRenderer)
        {
            _listeOutline.Add(rend.materials[1]);
        }
        // _outline = GetComponent<Renderer>().materials[1];
        // _outline.SetFloat("scale", 0.0f);
    }

    void Start()
    {
        foreach (Material outline in _listeOutline)
        {
            // Debug.Log(outline);
            outline.SetFloat("_scale", 0.0f);
        }

        // _outline.SetFloat("scale", 0.0f);
    }
    void Update()
    {
        if (Vector3.Distance(_perso.transform.position, transform.position) < _rangeDetection && _listeOutline != null && !_estProche)
        {
            _estProche = true;
            _perso.GetComponent<Perso>().RessourcesAProximite.Add(this);
            foreach (Material outline in _listeOutline)
            {
                outline.SetFloat("_scale", _scaleOutline);

            }
        }
        else if (Vector3.Distance(_perso.transform.position, transform.position) > _rangeDetection && _listeOutline != null && _estProche)
        {
            _perso.GetComponent<Perso>().RessourcesAProximite.Remove(this);
            _estProche = false;
            foreach (Material outline in _listeOutline)
            {
                // Debug.Log(outline);
                outline.SetFloat("_scale", 0.0f);
            }
            // _outline.SetFloat("scale", 0.0f);
        }
    }

    public void Collecter()
    {   
        Instantiate(_particules, transform.position, Quaternion.identity, transform.parent);
        _donneesPerso.AjouterRessource(_type, _valeur);
        _biome.infos["item"] = null;
        if(_biome.infos["biome"] != 1)
        {
            _biome.infos["biomeActif"] = _biome.ramassable;
            _biome.ChangerEtat(_biome.vide);
        }
        else
        {
            // _biome.infos["biomeActif"] = _biome.dangeureux;
            _biome.ChangerEtat(_biome.dangeureux);
        }
        Destroy(gameObject);
    }
}
