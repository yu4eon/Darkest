using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FeuDeCamp : MonoBehaviour
{
    // [SerializeField] private int _dureeVieMax = 200;
    Light _lumiere;
    SphereCollider _collider;
    float _rayonIni;
    float _intensiteIni;
    [SerializeField]int _dureeVie = 200;
    int _dureeVieIni;
    Perso _perso;
    // Start is called before the first frame update
    public void Init(int dureeVie, Perso perso)
    {
        _dureeVie = dureeVie;
        _dureeVieIni = dureeVie;
        _perso = perso;
        _perso.lFeuxDeCamp.Add(this);
    }

    void Start()
    {
        // _dureeVie = _dureeVieMax;
        _lumiere = GetComponentInChildren<Light>();
        _collider = GetComponent<SphereCollider>();
        _rayonIni = _collider.radius;
        _intensiteIni = _lumiere.intensity;
        StartCoroutine(DetruireFeu());
    }

    IEnumerator DetruireFeu()
    {
        while(_dureeVie > 0)
        {
            yield return new WaitForSeconds(1);
            // Debug.Log("Feu de camp : " + _dureeVie);
            _dureeVie--;
            _collider.radius = _rayonIni * (_dureeVie / (float)_dureeVieIni);
            _lumiere.intensity = _intensiteIni * (_dureeVie / (float)_dureeVieIni);
        }
        _perso.lFeuxDeCamp.Remove(this);
        Destroy(gameObject);
    }
}
