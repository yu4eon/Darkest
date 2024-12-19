using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe pour l'état final du biome
/// </summary>
public class BiomeEtatOccupe : BiomesEtatsBase
{
    Material _matCube; // Matériel pour le biome
    float _posyInit;

    /// <summary>
    /// Initialisation de l'état
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    public override void InitEtat(BiomesEtatsManager biome)
    {
        _matCube = (Material)Resources.Load("mats/b" + biome.infos["biome"] + "_" + biome.infos["variant"]); // charge le matériel du cube pour celui du biome
        biome.GetComponent<Renderer>().material = _matCube; // applique le matériel au cube
        _posyInit = biome.transform.localPosition.y; // sauvegarde la position y initiale du biome
        if(biome.infos["deco"] != null)
        {
            biome.StartCoroutine(ActiverDeco(biome));
            // biome.infos["deco"].SetActive(true);
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
        
    }
    /// <summary>
    /// Objet qui reste en collision avec le biome
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui reste en collision avec le cube</param>
    public override void TriggerStayEtat(BiomesEtatsManager biome, Collider other)
    {
        if (other.CompareTag("ChampDeForce"))
        {
            // S'assure que le biome a toujours le bon matériel
            _matCube = (Material)Resources.Load("mats/b" + biome.infos["biome"] + "_" + biome.infos["variant"]); // charge le matériel du cube pour celui du biome
            biome.GetComponent<Renderer>().material = _matCube;

        }
    }   

    /// <summary>
    /// Objet qui sort de la collision avec le biome
    /// </summary>
    /// <param name="biome">Manager de l'etat du biome</param>
    /// <param name="other">L'objet qui sort de la collision avec le cube</param>
    public override void TriggerExitEtat(BiomesEtatsManager biome, Collider other)
    {
        if (other.CompareTag("ChampDeForce"))
        {
            Coroutine coroutine = biome.StartCoroutine(ChangerActivable(biome)); // change l'état du biome pour celui activable après la coroutine
        }
    }

    IEnumerator ChangerActivable(BiomesEtatsManager biome)
    {
        bool transition = false; // booléen pour la deuxième transition
        // int randomDuree = Random.Range(2, 10); // durée aléatoire pour l'attente initiale entre 2 et 10 secondes
        // yield return new WaitForSeconds(randomDuree); // attend la durée aléatoire

        // Instancie le système de particules ombre sur le cube

        float t = 0; // temps écoulé depuis le début de la boucle while
        float duree = 0.3f; // durée totale de la boucle while
        // float posyInit = biome.transform.localPosition.y; // position y initiale du biome
        GameObject reveal = GameObject.Instantiate((GameObject)Resources.Load("FX/reveal"), biome.transform.position, Quaternion.identity, biome.transform);

        while (t < duree) // tant que le temps écoulé est inférieur à la durée
        {
            t += Time.deltaTime; // incrémente le temps écoulé selon le delta time
            float position = Mathf.Lerp(0, 0.5f, t / duree); // position y du biome selon le temps écoulé et la durée

            // Applique la position y au biome
            biome.transform.localPosition = new Vector3(biome.transform.localPosition.x, _posyInit + Mathf.PingPong(position, 0.25f), biome.transform.localPosition.z);
            if (t >= duree / 2 && !transition) // si le temps écoulé est égale ou supérieur à la durée moins le quart de la durée et que la deuxième transition n'a pas encore été effectuée
            {
                Object.Destroy(reveal); // détruit le système de particules reveal
                // Object.Destroy(ombre); // détruit le système de particules ombre
                transition = true; // la deuxième transition est effectuée
                _matCube = (Material)Resources.Load("etats/matActivable"); // charge le matériel du cube pour celui du biome
            }

            biome.GetComponent<Renderer>().material = _matCube; // applique le matériel au cube
            yield return null; // attend la prochaine frame

        }

        biome.ChangerEtat(biome.activable); // change l'état du biome pour celui final après la coroutine
    }
    IEnumerator ActiverDeco(BiomesEtatsManager biome)
    {
        biome.infos["deco"].SetActive(true);
        Vector3 scaleInit = biome.infos["deco"].transform.localScale;
        biome.infos["deco"].transform.localScale = new Vector3(0, 0, 0);
        while(biome.infos["deco"].transform.localScale.x < scaleInit.x)
        {
            biome.infos["deco"].transform.localScale = Vector3.Lerp(biome.infos["deco"].transform.localScale, scaleInit, Time.deltaTime * biome.infos["vitesseApparition"]);
            yield return null;
        }
        yield break;
        
    }
}
