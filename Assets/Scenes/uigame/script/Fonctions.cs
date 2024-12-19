using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fonctionc : MonoBehaviour
{

    [SerializeField] GameObject[] _listFonctions;
    [SerializeField] GameObject _buttonJouer;
    [SerializeField] FadeFromBlack _fadeFromBlack; // Reference to the FadeFromBlack script
    [SerializeField] int _activeFonction = 0;


    public void ChangerFonctionSuivant()
    {
        
        if(_activeFonction ==   _listFonctions.Length - 1)
        {
            _buttonJouer.SetActive(true);
            return;
        }
        else
        {
            _activeFonction++;
            _fadeFromBlack.resetFade();
        }

        for (int i = 0; i < _listFonctions.Length; i++)
        {
            if (i == _activeFonction)
            {
                _listFonctions[i].SetActive(true);
            }
            else
            {
                _listFonctions[i].SetActive(false);
            }
        }
    }

    public void ChangerFonctionPrecedent()
    {

        if (_activeFonction == 0)
        {
            return;
        }
        else
        {
            _activeFonction--;
            _fadeFromBlack.resetFade();
        }

        for (int i = 0; i < _listFonctions.Length; i++)
        {
            if (i == _activeFonction)
            {
                _listFonctions[i].SetActive(true);
            }
            else
            {
                _listFonctions[i].SetActive(false);
            }
        }
    }
}
