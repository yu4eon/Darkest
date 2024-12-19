using System.Collections;
using UnityEngine;

public class EnnemiEtatRepos : EnnemisEtatsBase
{

    public override void InitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.anim.SetBool("enCourse", false);
        ennemi.anim.SetBool("enAttaque", false);
        ennemi.agent.isStopped = false;
        ennemi.StartCoroutine(Repos(ennemi));
    }

    public override void UpdateEtat(EnnemisEtatsManager ennemi)
    {
        // ennemi.anim.SetBool("enCourse", ennemi.agent.velocity.magnitude > 1f);
    }

    public override void TriggerEnterEtat(EnnemisEtatsManager ennemi, Collider other)
    {

    }

    public override void ExitEtat(EnnemisEtatsManager ennemi)
    {
        ennemi.StopAllCoroutines();
    }

    IEnumerator Repos(EnnemisEtatsManager ennemi)
    {
        while (Vector3.Distance(ennemi.transform.position, ennemi.infos["perso"].position) > ennemi.infos["vision"])
        {
            Vector3 randomPosition = GenerateRandomPositionAroundHome(ennemi.infos["maison"].position, 10f);
            ennemi.agent.SetDestination(randomPosition);
            yield return new WaitForSeconds(5f);
        }
        ennemi.ChangerEtat(ennemi.chasse);
    }

    Vector3 GenerateRandomPositionAroundHome(Vector3 homePosition, float radius)
    {
        Vector2 randomDirection = Random.insideUnitCircle * radius;
        return new Vector3(homePosition.x + randomDirection.x, homePosition.y, homePosition.z + randomDirection.y);
    }
}
