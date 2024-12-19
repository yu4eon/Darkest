using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _audioSourceMusique;
    [SerializeField] AudioClip[] _tMusiques;
    AudioClip _musiqueActuelle;

    void Start()
    {
        _musiqueActuelle = _tMusiques[0];
        // ChangerMusique(0);
        // _audioSourceMusique.clip = _tMusiques[0];

        // _audioSourceMusique.Play();
    }
    public void JouerSon(AudioClip son, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(son, Camera.main.transform.position, volume);
    }

    public void ChangerMusique(int index)
    {
        if(_musiqueActuelle == _tMusiques[index])
        {
            return;
        }
        StopAllCoroutines();
        _musiqueActuelle = _tMusiques[index];
        StartCoroutine(FadeOut(1f, index));
        
        // Changer la musique
    }
    /// <summary>
    /// Coroutine pour effectuer un fade-in(fondu en entrée) sur une piste musicale.
    /// </summary>
    /// <param name="piste">La piste musicale à laquelle appliquer le fondu.</param>
    /// <param name="dureeFade">La durée du fondu en secondes.</param>
    public IEnumerator FadeIn( float dureeFade)
    {
        float tempsEcoule = 0f;
        float volumeInitial = 0f;

        while(tempsEcoule < dureeFade)
        {
            _audioSourceMusique.volume = Mathf.Lerp(volumeInitial, _audioSourceMusique.volume, tempsEcoule/dureeFade);
            tempsEcoule += Time.deltaTime;
            yield return null;
        }
        _audioSourceMusique.volume = 0.75f;
    }

    /// <summary>
    /// Coroutine pour effectuer un fade-out(fondu en sortie) sur une piste musicale.
    /// </summary>
    /// <param name="piste">La piste musicale à laquelle appliquer le fondu.</param>
    /// <param name="dureeFade">La durée du fondu en secondes.</param>
    public IEnumerator FadeOut(float dureeFade, int index)
    {
        float tempsEcoule = 0f;
        float volumeInitial = _audioSourceMusique.volume;

        while(tempsEcoule < dureeFade)
        {
            _audioSourceMusique.volume = Mathf.Lerp(volumeInitial, 0f, tempsEcoule/dureeFade);
            tempsEcoule += Time.deltaTime;
            yield return null;
        }
        _audioSourceMusique.volume = 0f;
        _audioSourceMusique.clip = _musiqueActuelle;
        _audioSourceMusique.Play();
        StartCoroutine(FadeIn(dureeFade));
        // piste.estActif = false;
    }
}
