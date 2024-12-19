using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesEtatsManager : MonoBehaviour
{
    BiomesEtatsBase _etatActuel;
    public BiomeEtatActivable activable = new BiomeEtatActivable();
    public BiomeEtatRamassable ramassable = new BiomeEtatRamassable();
    public BiomeEtatVide vide = new BiomeEtatVide();
    public float yIni { get; set; }
    public BiomeEtatInteractif interactif = new BiomeEtatInteractif();
    public BiomeEtatOccupe occupe = new BiomeEtatOccupe();
    public BiomeEtatDangeureux dangeureux = new BiomeEtatDangeureux();
    public Dictionary<string, dynamic> infos {get; set; } = new Dictionary<string, dynamic>();

    // Start is called before the first frame update
    void Start()
    {
        // ChangerEtat(activable);
        _etatActuel = activable;
        infos["etat"] = _etatActuel.GetType().Name.Replace("BiomeEtat", "");
        infos.Add("vitesseApparition", 3f);
        yIni = transform.position.y;

        
        if(infos.ContainsKey("biome") && infos["biome"] == 1)
        {
            infos.Add("biomeActif", dangeureux);
        }
        else if(infos.ContainsKey("item"))
        {
            infos.Add("biomeActif", ramassable);
        }
        else if(infos.ContainsKey("deco"))
        {
            infos.Add("biomeActif", occupe);
        }
        else
        {
            infos.Add("biomeActif", vide);
        }


        

        if(!infos.ContainsKey("item"))
        {
            // infos["item"] = null;
            infos.Add("item", null);
        }

        if(!infos.ContainsKey("deco"))
        {
            // infos["deco"] = null;
            infos.Add("deco", null);

        }
        _etatActuel.InitEtat(this);
    }


    public void ChangerEtat(BiomesEtatsBase nouvelEtat)
    {
        _etatActuel.ExitEtat(this);
        _etatActuel = nouvelEtat;
        infos["etat"] = _etatActuel.GetType().Name.Replace("BiomeEtat", "");
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
    }

    void OnTriggerStay(Collider other)
    {
        _etatActuel.TriggerStayEtat(this, other);
    }

    void OnTriggerExit(Collider other)
    {
        _etatActuel.TriggerExitEtat(this, other);
    }

    
}