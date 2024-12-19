using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    GameObject _perso;

    public void Init(GameObject perso)
    {
        _perso = perso;
    }


    // Update is called once per frame
    void Update()
    {
        if (_perso != null)
        {
            transform.position = _perso.transform.position + new Vector3(0, 4, -3);
        }
    }
}
