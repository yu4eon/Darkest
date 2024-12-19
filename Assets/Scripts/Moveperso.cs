using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveperso : MonoBehaviour
{


    [SerializeField] private float _vitesseMouvement = 5f; //Vitesse du personnage

    [SerializeField] private float _vitesseRotation = 3.0f; // Vitesse à laquel le personnage tourne
    [SerializeField] private float _gravite = 0.2f; // force de gravité
    [SerializeField] private float _vitesseSaut; //la vitesse à laquel le personnage monte et descend
    [SerializeField] private float _intervallePas = 0.7f; // Intervalle entre les pas en secondes

    [SerializeField] AudioClip[] _sonsMarche;
    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] AudioSource _audioSource;
    private float _positionCameraIni; // La position de la caméra en z

    private Vector3 _directionsMouvement = Vector3.zero; // La direction vers lequel le personnage bouge

    [SerializeField] Animator _animator; // l'animateur du personnage avec ses animations et les paramètres
    [SerializeField] CharacterController _controller; // Le controlleur de personnage sur le prefab
    public CharacterController controller
    {
        get { return _controller; }
    }
    float _dernierSonMarche = 0; // Le temps du dernier son de marche

    void Awake()
    {
        _animator = GetComponent<Animator>(); // On récupère l'animateur sur le personnage
        _controller = GetComponent<CharacterController>(); // On récupère le CharacterController sur le personnage
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // On change la rotation du personnage selon si on touches les touches "A" ou "D" (ou similaires) mulitiplié par la vitesse de rotation
        transform.Rotate(0, Input.GetAxis("Horizontal") * _vitesseRotation, 0);

        // On calcule la vitesse du personnage par le input reçu sur l'axe y ("W" ou "S") multiplié par la vitesse du personnage
        float vitesse = Input.GetAxis("Vertical") * _vitesseMouvement;

        // On met le booléen enCourse dans l'animateur selon si la vitesse est plus grand que 0
        _animator.SetBool("enCourse", Mathf.Abs(vitesse) > 0);
        //On ajuste la variable _directionsMouvement selon la vitesse actuelle
        _directionsMouvement = new Vector3(0, 0, vitesse);

        // On ajuste la valeur selon la position dans le monde du personnage
        _directionsMouvement = transform.TransformDirection(_directionsMouvement);


        // //On ajoute la valeur de vitesse saut au vecteur 3 de directions mouvement
        _directionsMouvement.y += _vitesseSaut;

        // // Si le joueur est dans les airs, on applique la gravité à vitesseSaut
        if (!_controller.isGrounded) _vitesseSaut -= _gravite;

        // On applique le mouvement finale du personnage selon le vecteur 3 obtenu multiplié par time.DeltaTime pour éviter des valeurs éxagéré
        _controller.Move(Time.deltaTime * _directionsMouvement);

        JouerSonDeMarche(vitesse);

    }

    public void Ralentir()
    {
        if(_vitesseMouvement == 5f)
        {
            _vitesseMouvement = 2.5f;
        }
    }
    public void RestaurerVitesse()
    {
        if (_vitesseMouvement == 2.5f)
        {
            _vitesseMouvement = 5f;

        }
    }

    void JouerSonDeMarche(float vitesse)
    {
        // Vérifier si le personnage est au sol et se déplace
        if (_controller.isGrounded && Mathf.Abs(vitesse) > 0)
        {
            if (Time.time - _dernierSonMarche > _intervallePas)
            {
                AudioClip sonMarche = _sonsMarche[_donneesPerso.biomeActuel - 1]; // On prend le son selon le biome actuel
                float pitchAleatoire = Random.Range(0.9f, 1.1f); // On prend un pitch aléatoire entre 0.8 et 1.2
                _audioSource.pitch = pitchAleatoire;
                _audioSource.PlayOneShot(sonMarche, 0.3f);
                _dernierSonMarche = Time.time;
            }
        }
        else
        {
            _audioSource.Stop();
        }
    }

}
