using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeFromBlack : MonoBehaviour
{
    private Image _img; // Inéragir avec l'image
    private float _alpha =1f; // Variable de l'alpha a à 100%

    // manipuler la vitesse de la diminution de l'opacité de l'image
    [SerializeField]
    [Range(0.1f, 5f)]
    private float _fadeSpeed; // Variable de la vitesse de la diminution de l'alpha  

    void Start()
    {
        _img = GetComponent<Image>();  // Récupère le componant Image
        ChangeAlpha(_alpha); // Appelle fontion Changer scene
    }

    // Progessivement changer l'opacité de l'imgage
    void Update()
    {
        _alpha -= Time.deltaTime * _fadeSpeed;

        if (_alpha <= 0f)
        {
            ChangeAlpha(0f);
        }
        else
        {
            ChangeAlpha(_alpha);
        }

    }

    public void resetFade()
    {
        _alpha = 1f;

        ChangeAlpha(_alpha);
    }



    // changer La couler de l'image
    public void ChangeAlpha(float value)
    {
        _img.color = new Color(_img.color.r, _img.color.g, _img.color.b,    value);
                            //      red         green           blue        opacity
    }
}
