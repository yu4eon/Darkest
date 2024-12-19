
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Perso : MonoBehaviour
{
    [SerializeField] SOPerso _donneesPerso;
    // [SerializeField] Camera _cameraUI;
    [SerializeField] SONavigation _donneesNavigation;
    [SerializeField] SphereCollider _colliderSphere; // Transform du champ de force, pour pouvoir changer sa taille 
    [SerializeField] Light _pointLight; // Lumière du champ de force
    [SerializeField] Light _pointLampe; // Lumière de la lampe
    // [SerializeField] float _rayonSphereIniBase; // Rayon initial du champ de force
    [SerializeField] float _rayonSphereMin = 5f; // Rayon minimal du champ de force

    // [SerializeField] int _coutAttaque = 15; // Cout en huile de l'attaque
    Slider _sliderSanite;
    public Slider sliderSanite
    {
        get { return _sliderSanite; }
        set { _sliderSanite = value; }
    }

    List<Ressource> _RessourcesAProximite = new();

    public List<Ressource> RessourcesAProximite
    {
        get { return _RessourcesAProximite; }
        set { _RessourcesAProximite = value; }
    }

    List<Pillier> _lPilliersAProximite = new();
    public List<Pillier> lPilliersAProximite
    {
        get { return _lPilliersAProximite; }
        set { _lPilliersAProximite = value; }
    }

    List<Rune> _lRunesAProximite = new();
    public List<Rune> lRunesAProximite
    {
        get { return _lRunesAProximite; }
        set { _lRunesAProximite = value; }
    }

    List<EnnemisEtatsManager> _lEnnemisAProximite = new();
    public List<EnnemisEtatsManager> lEnnemisAProximite
    {
        get { return _lEnnemisAProximite; }
        set { _lEnnemisAProximite = value; }
    }

    List<FeuDeCamp> _lFeuxDeCamp = new();
    public List<FeuDeCamp> lFeuxDeCamp
    {
        get { return _lFeuxDeCamp; }
        set { _lFeuxDeCamp = value; }
    }

    bool _estInvincible = false;
    bool _enAttaque = false;
    bool _estProcheFeu = false;

    // Ajouter Liste pour feu de camp afin de determiner si le joeur est proche d'un feu de camp

    // Ajouter liste Ennemis à proximité pour déterminer si on dois jouer la musique de chasse
    bool _estMort = false;

    [SerializeField] float _vitesseBaisseSanite = 5f;
    [SerializeField] AudioClip _sonPickup;
    [SerializeField] AudioClip _sonInventaire;
    [SerializeField] AudioClip _sonDegats;
    [SerializeField] AudioClip _sonAttaque;
    [SerializeField] AudioClip _sonMort;
    [SerializeField] AudioClip _sonRune;
    [SerializeField] AudioClip _sonEnnemiEtourdi;
    [SerializeField] FeuDeCamp _prefabFeuDeCamp;
    [SerializeField] GameObject _particulesAttaque;
    [SerializeField] Camera _uiCamera;
    [SerializeField] Animator _animator;
    [SerializeField] Color _couleurRouge;
    [SerializeField] Transform _positionLanterne;
    UIJeu _uiJeu;
    UICraft _uiCraft;
    SoundManager _soundManager;
    Moveperso _moveperso;
    Radio _radio;
    CharacterController _controller;


    void Start()
    {
        _donneesPerso.Initialiser();
        StartCoroutine(CoroutineAjusterSanite());
        StartCoroutine(CoroutineBaisserHuileNaturel());
        StartCoroutine(CoroutineCheckFeuDeCamp());
        StartCoroutine(ChercherSol());
        _animator = GetComponent<Animator>();
        _moveperso = GetComponent<Moveperso>();
        _radio = GetComponent<Radio>();
        _controller = GetComponent<CharacterController>();

        _donneesPerso.evenementMiseAJour.AddListener(AjusterRange);
        _donneesPerso.evenementInstantiateFeuDeCamp.AddListener(InstantiateFeuDeCamp);


        AjusterRange();
    }

    IEnumerator ChercherSol()
    {
        while (_donneesPerso.sanite > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<BiomesEtatsManager>() != null)
                {
                    int biome = hit.collider.GetComponent<BiomesEtatsManager>().infos["biome"];
                    _donneesPerso.biomeActuel = biome;

                    if (biome == 1 && _donneesPerso.possedeAmulette == false)
                    {
                        _moveperso.Ralentir();
                        StartCoroutine(CoroutineAjusterCouleurLanterne(_couleurRouge));
                        PrendreDegats(10f);
                    }
                    else
                    {
                        _moveperso.RestaurerVitesse();
                        StartCoroutine(CoroutineAjusterCouleurLanterne(Color.white));
                    }
                }

            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CoroutineAjusterCouleurLanterne(Color color)
    {
        float t = 0;
        Color colorIni = _pointLight.color;
        while (t < 1)
        {
            t += Time.deltaTime;
            _pointLight.color = Color.Lerp(colorIni, color, t);
            _pointLampe.color = Color.Lerp(colorIni, color, t);
            yield return null;
        }
    }

    public void Initialiser(UIJeu uIJeu, SoundManager soundManager, UICraft uICraft)
    {
        _uiJeu = uIJeu;
        _soundManager = soundManager;
        _uiCraft = uICraft;

        _uiJeu.GetComponent<Canvas>().worldCamera = _uiCamera;
        // _donneesPerso.evenementMiseAJour.AddListener(_uiJeu.MettreAJourUI);
    }

    public void PrendreDegats(float degats)
    {
        if (_estInvincible) return;

        _soundManager.JouerSon(_sonDegats);
        _donneesPerso.sanite -= degats;
        _estInvincible = true;
        _uiJeu.MettreAJourUI();

        StartCoroutine(CoroutineInvincible());
        if (_donneesPerso.sanite <= 0) Mourir();
    }



    public void AjouterRune(int indexRune)
    {
        _donneesPerso.dRunes[indexRune] = true;
        // Debug.Log("Rune ajoutée : " + indexRune);
        _uiJeu.ActiverRune(indexRune);
    }

    void OnPickup()
    {
        if (_RessourcesAProximite.Count > 0)
        {
            Debug.Log(_soundManager + " " + _sonPickup);
            _soundManager.JouerSon(_sonPickup);
            foreach (Ressource ressource in _RessourcesAProximite)
            {
                ressource.Collecter();
                Debug.Log("Ressource collectée");
            }
            _RessourcesAProximite.Clear();
            _uiCraft.MettreAJourInventaire();
        }

        if (_lRunesAProximite.Count > 0)
        {
            _soundManager.JouerSon(_sonPickup);
            foreach (Rune rune in _lRunesAProximite)
            {
                rune.Collecter();
                _radio.EnleverRuneSurScene(rune);
                Debug.Log("Rune collectée");
            }
            _lRunesAProximite.Clear();
        }

        if (_lPilliersAProximite.Count > 0)
        {
            _soundManager.JouerSon(_sonRune);
            foreach (Pillier pillier in _lPilliersAProximite)
            {
                pillier.Activer();
                Debug.Log("Pillier activé");
            }
            _lPilliersAProximite.Clear();
        }

    }
    void OnInventory()
    {
        _soundManager.JouerSon(_sonInventaire);
        _uiCraft.gameObject.SetActive(!_uiCraft.gameObject.activeSelf);

        _uiCraft.MettreAJourInventaire();
    }


    void OnFire()
    {
        if (_donneesPerso.niveauHuile == 0 || _enAttaque)
        {
            return;
        }
        AjusterHuile(-_donneesPerso.coutAttaque);
        _enAttaque = true;
        _animator.SetTrigger("Attaque");


        _moveperso.enabled = false;

        foreach (EnnemisEtatsManager ennemi in _lEnnemisAProximite)
        {
            ennemi.ChangerEtat(ennemi.etourdi);
        }
        if(_lEnnemisAProximite.Count > 0)
        {
            _soundManager.JouerSon(_sonEnnemiEtourdi, 0.5f);
        }
    }


    public void StartCoroutineLampe()
    {
        _soundManager.JouerSon(_sonAttaque, 0.6f);
        StartCoroutine(CoroutineLampeAttaque());
    }

    public void ActiverMouvement()
    {
        // _moveperso.controller.enabled = true;
        _enAttaque = false;
        _moveperso.enabled = true;
    }

    IEnumerator CoroutineInvincible()
    {
        yield return new WaitForSeconds(1f);
        _estInvincible = false;
    }

    IEnumerator CoroutineAjusterSanite()
    {
        while (_donneesPerso.sanite > 0)
        {
            yield return new WaitForSeconds(_vitesseBaisseSanite);
            int ajustement = -1;
            if (_estProcheFeu)
            {
                ajustement = 10;
            }
            else if (_donneesPerso.niveauHuile == 0)
            {
                ajustement = -10;
            }
            _donneesPerso.sanite += ajustement;
            _uiJeu.MettreAJourUI();
            // _sliderSanite.value = _sanite / 100f;
        }
        Mourir();
    }

    IEnumerator CoroutineBaisserHuileNaturel()
    {

        while (_donneesPerso.niveauHuile > 0)
        {
            if (_estProcheFeu)
            {
                AjusterHuile(10);

            }
            else
            {
                AjusterHuile(-1);
            }
            yield return new WaitForSeconds(2f);

        }
    }

    public void AjusterHuile(int quantite)
    {
        if (_enAttaque)
        {
            return;
        }
        _donneesPerso.niveauHuile += quantite;
        _uiJeu.MettreAJourUI();
        AjusterRange();
    }

    public void AjusterRange()
    {
        if (_donneesPerso.niveauHuile <= 0)
        {
            // Debug.Log("Plus d'huile");
            _colliderSphere.radius = _rayonSphereMin;
            _pointLight.range = _rayonSphereMin;
        }
        else
        {
            // Debug.Log("Huile restante : " + _donneesPerso.niveauHuile);
            float nouveauRayon = Mathf.Clamp(_donneesPerso.rayon * (_donneesPerso.niveauHuile / 100f), _rayonSphereMin, int.MaxValue);
            _pointLight.range = nouveauRayon;
            _pointLight.intensity = Mathf.Clamp(_donneesPerso.rayon, 4, 12);
            _colliderSphere.radius = nouveauRayon;
        }
    }

    void Mourir()
    {
        if (_estMort)
        {
            return;
        }
        _moveperso.enabled = false;
        _estMort = true;
        _soundManager.JouerSon(_sonMort);
        StartCoroutine(CoroutineMourir());
        Debug.Log("Le perso est mort");
    }

    IEnumerator CoroutineMourir()
    {
        yield return new WaitForSeconds(1f);
        _donneesNavigation.AllerSceneSuivante();

    }

    IEnumerator CoroutineLampeAttaque()
    {
        Instantiate(_particulesAttaque, _positionLanterne.transform.position, Quaternion.identity, _positionLanterne);
        float intensiteIni = _pointLight.intensity;
        float rayonIni = _pointLight.range;
        float t = 0;
        float valeur = 0.5f;
        while (_enAttaque)
        {
            t += Time.deltaTime;
            if (t > 0.4f && valeur > 0)
            {
                valeur = -0.5f;
            }

            _pointLight.range = rayonIni * 1.5f;
            // _colliderSphere.localScale = new Vector3(rayonIni * 1.5f, rayonIni * 1.5f, rayonIni * 1.5f);
            _colliderSphere.radius = rayonIni * 1.5f;
            _pointLight.intensity = Mathf.Clamp(_pointLight.intensity + valeur, intensiteIni, intensiteIni * 4);
            yield return null;
        }
        _pointLight.intensity = intensiteIni;
        _pointLight.range = Mathf.Clamp(_donneesPerso.rayon * (_donneesPerso.niveauHuile / 100f), _rayonSphereMin, int.MaxValue);
        // _colliderSphere.localScale = new Vector3(_pointLight.range, _pointLight.range, _pointLight.range);
        _colliderSphere.radius = _pointLight.range;

    }

    IEnumerator CoroutineCheckFeuDeCamp()
    {
        List<FeuDeCamp> lFeuDeCampProche = new();
        while (_donneesPerso.sanite > 0)
        {
            foreach (FeuDeCamp feu in _lFeuxDeCamp)
            {
                if (Vector3.Distance(transform.position, feu.transform.position) < 8 && !_estProcheFeu)
                {
                    lFeuDeCampProche.Add(feu);

                }
                else if (Vector3.Distance(transform.position, feu.transform.position) > 8 && _estProcheFeu)
                {
                    lFeuDeCampProche.Remove(feu);
                }
            }
            if (lFeuDeCampProche.Count > 0)
            {
                Debug.Log("Le joueur est proche d'un feu de camp");
                _estProcheFeu = true;
            }
            else
            {
                Debug.Log("Le joueur n'est pas proche d'un feu de camp");
                _estProcheFeu = false;
            }

            yield return new WaitForSeconds(2f);
        }
    }


    public void InstantiateFeuDeCamp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 positionFeu = hit.point + Vector3.up * 0.1f;
            FeuDeCamp feuDeCamp = Instantiate(_prefabFeuDeCamp, positionFeu, Quaternion.identity);
            feuDeCamp.Init(200, this);
            _lFeuxDeCamp.Add(feuDeCamp);

        }
    }

    void OnApplicationQuit()
    {
        _donneesPerso.ResetNiveau();
        _donneesPerso.Initialiser(); // Initialise les données du personnage
    }
    void OnDestoy()
    {
        _donneesPerso.evenementMiseAJour.RemoveAllListeners();
        _donneesPerso.Initialiser(); // Initialise les données du personnage
    }

    public void Gagner()
    {
        _donneesNavigation.AllerSceneWin();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            Mourir();
        }
    }
}
