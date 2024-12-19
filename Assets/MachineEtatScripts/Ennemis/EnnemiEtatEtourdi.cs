using System.Collections;
using UnityEngine;

public class EnnemiEtatEtourdi : EnnemisEtatsBase
{

    public override void InitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.anim.SetBool("enCourse", false);
        ennemi.anim.SetBool("enAttaque", false);
        ennemi.anim.SetBool("enEtourdi", true);
        ennemi.agent.isStopped = true;
        // ennemi.StopAllCoroutines();
        ennemi.StartCoroutine(Etourdi(ennemi));
    }

    public override void UpdateEtat(EnnemisEtatsManager ennemi)
    {

    }

    public override void TriggerEnterEtat(EnnemisEtatsManager ennemi, Collider other)
    {

    }

    public override void ExitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.anim.SetBool("enEtourdi", false);
        ennemi.StopAllCoroutines();
    }

    IEnumerator Etourdi(EnnemisEtatsManager ennemi)
    {
        // ennemi.audioSource.PlayOneShot(ennemi.sonEtourdi, 0.6f);
        yield return new WaitForSeconds(5);
        ennemi.ChangerEtat(ennemi.repos);
    }

}
