using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIJeu : MonoBehaviour
{
    [SerializeField] SOPerso _donneesPerso;
    // [SerializeField] TextMeshProUGUI _champNiveauHuile;
    [SerializeField] ParticleSystem _particulesHuile;
    float _scaleIniHuile;
    [SerializeField] Slider _sliderSanite;
    [SerializeField] Slider _sliderSecondaire;
    // [SerializeField] float _tempsAjustementBarre = 2;
    [SerializeField] Image[] _tRunes;
    [SerializeField] TextMeshProUGUI _texteRadio;
    [SerializeField] Image _amulette;
    Canvas _canvas;
    Camera _camera;
    // Start is called before the first frame update

    public void Init(Camera camera)
    {
        _canvas = GetComponent<Canvas>();
        _camera = camera;
        _canvas.worldCamera = _camera;
    }
    void Start()
    {
        _sliderSanite.value = 1;
        // _champNiveauHuile.text = _donneesPerso.niveauHuile + "%";
        _donneesPerso.evenementMiseAJour.AddListener(MettreAJourUI);
        _scaleIniHuile = _particulesHuile.transform.localScale.x;

        foreach (Image rune in _tRunes)
        {
            rune.color = new Color(1, 1, 1, 0.2f);
        }
    }

    public void ActiverRune(int index)
    {
        _tRunes[index].color = new Color(1, 1, 1, 1);
    }

    public void SupprimerRune(int index)
    {
        _tRunes[index].color = new Color(1, 1, 1, 0f);
    }

    public void MettreAJourUI()
    {
        _sliderSanite.value = _donneesPerso.sanite / 100f;
        float newScale = _scaleIniHuile * (_donneesPerso.niveauHuile / 100f);
        _particulesHuile.transform.localScale = new Vector3(newScale, newScale, newScale);

        StartCoroutine(CoroutineAjusterBarreSecondaire());
        if (_donneesPerso.niveauRadio >= 2)
        {
            if (_donneesPerso.DistanceMinRune < _donneesPerso.rangeDetection)
            {
                _texteRadio.enabled = true;
                string valeur = ((Mathf.Floor(_donneesPerso.DistanceMinRune * 10)) / 10).ToString();
                Debug.Log(valeur);
                _texteRadio.text = valeur + "m";
            }
            else
            {
                _texteRadio.text = "";    
            }
        }
        else
        {
            _texteRadio.enabled = false;
        }

        if(_donneesPerso.possedeAmulette)
        {
            _amulette.enabled = true;
        }
        else
        {
            _amulette.enabled = false;
        }
    }

    IEnumerator CoroutineAjusterBarreSecondaire()
    {
        float temps = 0;
        while (temps < 1)
        {
            temps += Time.deltaTime;
            _sliderSecondaire.value = Mathf.Lerp(_sliderSecondaire.value, _sliderSanite.value, temps);
            yield return null;
        }
    }
}
