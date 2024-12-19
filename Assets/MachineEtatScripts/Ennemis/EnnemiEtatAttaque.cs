using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiEtatAttaque : EnnemisEtatsBase
{

    public override void InitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.anim.SetBool("enCourse", false);
        ennemi.anim.SetBool("enAttaque", true);
        ennemi.agent.isStopped = false;
        ennemi.colliderAttaque.enabled = true;
        ennemi.transform.LookAt(ennemi.infos["perso"].position);
        // ennemi.StopAllCoroutines();
        
        ennemi.StartCoroutine(Attaque(ennemi));
    }

    public override void UpdateEtat(EnnemisEtatsManager ennemi)
    {

    }

    public override void ExitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.StopAllCoroutines();
        ennemi.anim.SetBool("enAttaque", false);
        ennemi.colliderAttaque.enabled = false;
    }

    public override void TriggerEnterEtat(EnnemisEtatsManager ennemi, Collider other)
    {
        Debug.Log("Attaque");
        if(other.CompareTag("Player"))
        {
            other.GetComponent<Perso>().PrendreDegats(10f);
        }
        
    }

    IEnumerator Attaque(EnnemisEtatsManager ennemi)
    {
        ennemi.audioSource.PlayOneShot(ennemi.sonAttaque);
        yield return new WaitForSeconds(1f);
        if(ennemi.agent.remainingDistance < 10f)
        {
            ennemi.ChangerEtat(ennemi.chasse);
            yield break;
        }
        else
        {
            ennemi.ChangerEtat(ennemi.repos);
            yield break;
        }

    }
}
