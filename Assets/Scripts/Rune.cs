using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour
{
    [SerializeField, Range(1, 5)] int _indexRune;
    [SerializeField] ParticleSystem _particules;
    [SerializeField] Color _couleur;
    Renderer _renderer;
    GameObject _perso;
    BiomesEtatsManager _biome;
    Material _outline;
    [SerializeField] float _scaleOutline = 1.1f;
    [SerializeField] float _rangeDetection = 3.0f;
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

        _renderer = GetComponent<Renderer>();
        _outline = _renderer.materials[1];
        // _outline = GetComponent<Renderer>().materials[1];
        // _outline.SetFloat("scale", 0.0f);
    }

    void Start()
    {

        _outline.SetColor("_Color", _couleur);
        // Debug.Log(outline);
        _outline.SetFloat("_scale", 0.0f);


        // _outline.SetFloat("scale", 0.0f);
    }
    void Update()
    {
        if (Vector3.Distance(_perso.transform.position, transform.position) < _rangeDetection && _outline != null && !_estProche)
        {
            _estProche = true;
            // _perso.GetComponent<Perso>().ressourcesAProximite.Add(this);
            _perso.GetComponent<Perso>().lRunesAProximite.Add(this);

            // Debug.Log(outline);
            _outline.SetFloat("_scale", _scaleOutline);


        }
        else if (Vector3.Distance(_perso.transform.position, transform.position) > _rangeDetection && _outline != null && _estProche)
        {
            // _perso.GetComponent<Perso>().ressourcesAProximite.Remove(this);
            _perso.GetComponent<Perso>().lRunesAProximite.Remove(this);
            _estProche = false;

            // Debug.Log(outline);
            _outline.SetFloat("_scale", 0.0f);

            // _outline.SetFloat("scale", 0.0f);
        }
    }

    public void Collecter()
    {
        Instantiate(_particules, transform.position, Quaternion.identity, transform.parent);
        // _perso.GetComponent<Perso>().AjouterRessource(_type, _valeur);
        _perso.GetComponent<Perso>().AjouterRune(_indexRune -1);

        _biome.infos["item"] = null;
        if (_biome.infos["biome"] != 1)
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
