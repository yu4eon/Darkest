using System.Collections;
using UnityEngine;


/// <summary>
/// Classe pour l'état activable du biome
/// qui permet de changer l'état du biome en cultivable
/// après une collision avec un champ de force
/// </summary>
public class BiomeEtatActivable : BiomesEtatsBase
{
    Material _matCube; // Matériel pour le biome
    float _posyInit;
    Coroutine _coroutineChangement;
    bool _revealEnCours = false;

    /// <summary>
    /// Initialisation de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void InitEtat(BiomesEtatsManager biome)
    {
        _matCube = (Material)Resources.Load("etats/matActivable"); // charge le matériel du cube pour celui du biome
        biome.GetComponent<Renderer>().material = _matCube;
        _posyInit = biome.transform.localPosition.y;
        if (biome.infos["item"] != null)
        {
            biome.infos["item"].SetActive(false);
        }
        else if (biome.infos["deco"] != null)
        {
            biome.infos["deco"].SetActive(false);
        }
    }
    /// <summary>
    /// Sortie de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void ExitEtat(BiomesEtatsManager biome)
    {
        biome.transform.localPosition = new Vector3(biome.transform.localPosition.x, _posyInit, biome.transform.localPosition.z); // remet le biome à sa position initiale
        biome.StopAllCoroutines();
        _matCube = (Material)Resources.Load("mats/b" + biome.infos["biome"] + "_" + biome.infos["variant"]);
        biome.GetComponent<Renderer>().material = _matCube;
        // _matCube = (Material)Resources.Load("mats/b" + biome.infos["biome"] + "_" + biome.infos["variant"]);
        // biome.GetComponent<Renderer>().material = _matCube;
        // if (biome.infos["item"] != null)
        // {
        //     biome.infos["item"].SetActive(true);
        // }
        // biome.transform.localPosition = new Vector3(biome.transform.localPosition.x, _posyInit, biome.transform.localPosition.z);
    }

    /// <summary>
    /// Mise à jour de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void UpdateEtat(BiomesEtatsManager biome)
    {
    }

    /// <summary>
    /// Collision avec un autre objet
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui se collisionne avec le cube</param>
    public override void TriggerEnterEtat(BiomesEtatsManager biome, Collider other)
    {
        // Si l'objet qui se collisionne avec le cube est un champ de force, commence la coroutine de changement d'état
        if (other.CompareTag("ChampDeForce"))
        {
            _coroutineChangement = biome.StartCoroutine(ChangerEtat(biome));
        }
    }

    /// <summary>
    /// Objet qui reste en collision avec le biome
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui reste en collision avec le cube</param>
    public override void TriggerStayEtat(BiomesEtatsManager biome, Collider other)
    {
        // Si l'objet qui reste en collision avec le cube est un champ de force, change de matériel car le biome est activable
        if (other.CompareTag("ChampDeForce"))
        {
            // _coroutineEnCours = true;
            // if(biome.GetComponent<Renderer>().material != (Material)Resources.Load("etats/matActivable"))
            // {
            //     return;
            // }
            _coroutineChangement = biome.StartCoroutine(ChangerEtat(biome));
        }
    }

    /// <summary>
    /// Objet qui sort de la collision avec le biome
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui sort de la collision avec le cube</param>
    public override void TriggerExitEtat(BiomesEtatsManager biome, Collider other)
    {

    }

    /// <summary>
    /// Coroutine de changement d'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    IEnumerator ChangerEtat(BiomesEtatsManager biome)
    {
        // if (_coroutineEnCours) yield break;
        float t = 0;
        float duree = 0.3f;
        bool transition = false;
        // float posyInit = biome.transform.localPosition.y;

        if (!_revealEnCours)
        {
            GameObject reveal = GameObject.Instantiate((GameObject)Resources.Load("FX/reveal"), biome.transform.position, Quaternion.identity, biome.transform);
            _revealEnCours = true;
        }

        while (t < duree)
        {
            t += Time.deltaTime;
            float position = Mathf.Lerp(0, 1, t / duree);
            biome.transform.localPosition = new Vector3(biome.transform.localPosition.x, _posyInit + Mathf.PingPong(position, 0.5f), biome.transform.localPosition.z);
            yield return null;

            if (t >= duree / 2 && !transition)
            {
                transition = true;
                _matCube = (Material)Resources.Load("mats/b" + biome.infos["biome"] + "_" + biome.infos["variant"]);
                biome.GetComponent<Renderer>().material = _matCube;
            }
        }
        // Object.Destroy(reveal);
        _revealEnCours = false;
        if (biome.infos.ContainsKey("biomeActif"))
        {
            biome.ChangerEtat(biome.infos["biomeActif"]);
        }
        else
        {
            biome.ChangerEtat(biome.ramassable);
        }
        // _coroutineEnCours = false;
        // if (biome.infos["biome"] == 1)
        // {
        //     biome.ChangerEtat(biome.dangeureux); // change l'état du biome pour dangeureux après la coroutine
        // }
        // else
        // {
        //     biome.ChangerEtat(biome.ramassable); // change l'état du biome pour cultivable après la coroutine

        // }
    }
}
