using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillier : MonoBehaviour
{
    [SerializeField, Range(1, 5)] int _indexRune;
    Renderer _renderer;
    GameObject _perso;
    BiomesEtatsManager _biome;
    Material _outline;
    Animator _animator;
    UIJeu _uiJeu;
    [SerializeField] float _scaleOutline = 1.1f;
    [SerializeField] float _rangeDetection = 3.0f;
    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] GameObject _rune;

    bool _estProche = false;
    bool _estActivable = true;

    public void Init(GameObject perso, UIJeu uIJeu)
    {
        _perso = perso;
        _uiJeu = uIJeu;
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<Renderer>();
        _outline = _renderer.materials[1];
        
    }

    void Start()
    {

        _outline.SetFloat("_scale", 0.0f);
        _rune.SetActive(false);


        // _outline.SetFloat("scale", 0.0f);
    }
    void Update()
    {
        if (Vector3.Distance(_perso.transform.position, transform.position) < _rangeDetection && _outline != null && !_estProche && _donneesPerso.dRunes[Mathf.Clamp(_indexRune -1, 0, 4)] == true && _estActivable)
        {
            _estProche = true;
            // _perso.GetComponent<Perso>().ressourcesAProximite.Add(this);
            _perso.GetComponent<Perso>().lPilliersAProximite.Add(this);

            // Debug.Log(outline);¸

            _outline.SetFloat("_scale", _scaleOutline);


        }
        else if (Vector3.Distance(_perso.transform.position, transform.position) > _rangeDetection && _outline != null && _estProche && _donneesPerso.dRunes[Mathf.Clamp(_indexRune -1, 0, 4)] == true && _estActivable)
        {
            // _perso.GetComponent<Perso>().ressourcesAProximite.Remove(this);
            _estProche = false;
            _perso.GetComponent<Perso>().lPilliersAProximite.Remove(this);

            // Debug.Log(outline);
            _outline.SetFloat("_scale", 0.0f);

            // _outline.SetFloat("scale", 0.0f);
        }
    }

    public void Activer()
    {
        _estActivable = false;
        // _perso.GetComponent<Perso>().pilliersAProximite.Remove(this);
        _rune.SetActive(true);
        _animator.SetTrigger("Activer");
        _outline.SetFloat("_scale", 0.0f);
        _uiJeu.SupprimerRune(_indexRune - 1);
        _donneesPerso.nbAutelsRestants--;
        if(_donneesPerso.nbAutelsRestants == 0)
        {
            _perso.GetComponent<Perso>().Gagner();
        }

        // _perso.GetComponent<Perso>().AjouterRessource(_type, _valeur);
        // _biome.infos["item"] = null;
        // if (_biome.infos["biome"] != 1)
        // {
        //     _biome.infos["biomeActif"] = _biome.ramassable;
        //     _biome.ChangerEtat(_biome.vide);
        // }
        // else
        // {
        //     // _biome.infos["biomeActif"] = _biome.dangeureux;
        //     _biome.ChangerEtat(_biome.dangeureux);
        // }
        // Destroy(gameObject);
    }


}
