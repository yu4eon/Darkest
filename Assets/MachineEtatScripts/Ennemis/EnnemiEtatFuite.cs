using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiEtatFuite : EnnemisEtatsBase
{

    public override void InitEtat(EnnemisEtatsManager ennemi)
    {
        Debug.Log("Fuite");
        ennemi.anim.SetBool("enCourse", true);
        ennemi.anim.SetBool("enAttaque", false);
        ennemi.agent.isStopped = false;
        ennemi.infos["cible"] = ennemi.infos["maison"];
        ennemi.agent.SetDestination(ennemi.infos["maison"].position);
        ennemi.StartCoroutine(Fuite(ennemi));

    }

    public override void UpdateEtat(EnnemisEtatsManager ennemi)
    {

    }

    public override void TriggerEnterEtat(EnnemisEtatsManager ennemi, Collider other)
    {

    }

    public override void ExitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.StopAllCoroutines();
    }

    IEnumerator Fuite(EnnemisEtatsManager ennemi)
    {
        while (ennemi.agent.remainingDistance < 2f && !ennemi.agent.pathPending)
        {

            ennemi.agent.destination = ennemi.infos["cible"].position;
            yield return new WaitForSeconds(2f);

            if(ennemi.infos["perso"].position < ennemi.infos["vision"])
            {
                ennemi.ChangerEtat(ennemi.chasse);
                yield break;
            }
            yield return null;
        }

        ennemi.ChangerEtat(ennemi.repos);
    }

    // IEnumerator Chasse(EnnemisEtatsManager ennemi)
    // {

        // ennemi.audioSource.PlayOneShot(ennemi.sonCri, 0.6f);
        // while (true)
        // {
        //     Debug.Log(ennemi.agent.remainingDistance > ennemi.infos["vision"] && ennemi.infos["cible"] == ennemi.infos["perso"]);
        //     if (ennemi.agent.remainingDistance > ennemi.infos["vision"] && ennemi.infos["cible"] == ennemi.infos["perso"] )
        //     {
        //         ennemi.infos["cible"] = ennemi.infos["maison"];
        //     }
        //     else if ((ennemi.agent.remainingDistance > 1f && ennemi.infos["cible"] == ennemi.infos["perso"]) || ennemi.agent.pathPending)
        //     {
        //         ennemi.infos["cible"] = ennemi.infos["perso"];

        //     }
        //     else if (ennemi.agent.remainingDistance < 2f && !ennemi.agent.pathPending)
        //     {
        //         if (ennemi.infos["cible"] == ennemi.infos["perso"])
        //         {
        //             ennemi.ChangerEtat(ennemi.attaque);
                    
        //             yield break;
        //             // ennemi.infos["infos"] = ennemi.infos["maison"];
        //             // ennemi.agent.destination = ennemi.infos["infos"].position;
        //         }
        //         else if (ennemi.infos["cible"] == ennemi.infos["maison"])
        //         {
        //             ennemi.ChangerEtat(ennemi.repos);
                    
        //             yield break;
        //         }
        //     }
        //     ennemi.agent.destination = ennemi.infos["cible"].position;
        //     yield return new WaitForSeconds(0.5f);

        // }
        // while (Vector3.Distance(ennemi.transform.position, ennemi.infos["perso"].position) > ennemi.infos["vision"])
        // {
        //     ennemi.agent.SetDestination(ennemi.infos["perso"].position);
        //     yield return new WaitForSeconds(0.5f);
        // }
        // ennemi.agent.SetDestination(ennemi.infos["maison"].position);
        // // ennemi.ChangerEtat(ennemi.repos);
    // }
}
