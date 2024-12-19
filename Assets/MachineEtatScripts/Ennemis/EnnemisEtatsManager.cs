using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemisEtatsManager : MonoBehaviour
{
    EnnemisEtatsBase _etatActuel;
    public EnnemiEtatRepos repos = new EnnemiEtatRepos();
    public EnnemiEtatChasse chasse = new EnnemiEtatChasse();
    public EnnemiEtatAttaque attaque = new EnnemiEtatAttaque();
    public EnnemiEtatEtourdi etourdi = new EnnemiEtatEtourdi();
    public EnnemiEtatFuite fuite = new EnnemiEtatFuite();
    public Transform perso { get; set; }
    public NavMeshAgent agent { get; set; }
    
    public BoxCollider colliderAttaque { get; set; }
    public Animator anim { get; set; }
    // public Transform cible { get; set; }
    public Dictionary<string, dynamic> infos { get; set; } = new Dictionary<string, dynamic>();
    public AudioSource audioSource { get; set; }
    [SerializeField] AudioClip _sonAttaque;
    public AudioClip sonAttaque { get { return _sonAttaque; } }
    [SerializeField] AudioClip _sonEtourdi;
    public AudioClip sonEtourdi { get { return _sonEtourdi; } }
    [SerializeField] AudioClip _sonCri;
    public AudioClip sonCri { get { return _sonCri; } }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        transform.Rotate(0, Random.Range(0, 360), 0);
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        infos.Add("vision", 10f);
        infos.Add("cible", infos["maison"]);
        infos.Add("speed", agent.speed);
        // ChangerEtat(repos);
        _etatActuel = repos;
        // infos["etat"] = _etatActuel.GetType().Name.Replace("BiomeEtat", "");
        infos.Add("etat", _etatActuel.GetType().Name.Replace("EnnemiEtat", ""));
        // Debug.Log(infos["etat"]);
        _etatActuel.InitEtat(this);
        colliderAttaque = GetComponent<BoxCollider>();
        colliderAttaque.enabled = false;
        
        // infos["vision"] = 10f;
    }


    public void ChangerEtat(EnnemisEtatsBase nouvelEtat)
    {
        _etatActuel.ExitEtat(this);
        _etatActuel = nouvelEtat;
        infos["etat"] = _etatActuel.GetType().Name.Replace("EnnemiEtat", "");
        _etatActuel.InitEtat(this);
    }

    // Update is called once per frame
    void Update()
    {
        _etatActuel.UpdateEtat(this);
    }

    void OnTriggerEnter(Collider other)
    {

        _etatActuel.TriggerEnterEtat(this, other);
        if (other.CompareTag("ChampDeForce"))
        {
            infos["perso"].GetComponent<Perso>().lEnnemisAProximite.Add(this);

        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ChampDeForce") )
        {

            infos["perso"].GetComponent<Perso>().lEnnemisAProximite.Remove(this);

        }
        // _etatActuel.TriggerExitEtat(this, other);
    }
}