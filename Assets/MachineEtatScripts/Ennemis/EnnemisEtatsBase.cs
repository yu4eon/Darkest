using UnityEngine;

public abstract class EnnemisEtatsBase
{

    public abstract void InitEtat(EnnemisEtatsManager ennemi);
    public abstract void UpdateEtat(EnnemisEtatsManager ennemi);
    public abstract void TriggerEnterEtat(EnnemisEtatsManager ennemi, Collider other);
    public abstract void ExitEtat(EnnemisEtatsManager ennemi);
    
}
