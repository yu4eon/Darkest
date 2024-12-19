using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Radio : MonoBehaviour
{
    List<Rune> _runesSurScene = new();
    [SerializeField] AudioSource _audioSource;
    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] AudioClip _clipRadio;
    [SerializeField] TextMeshProUGUI _texteRadio;

    Image _imageRadio;
    // Rune _runeProche;

    float _currentBlinkSpeed = 0f; // Store the current blinking speed
    bool _isBlinking = false; // Track if the blinking effect is active


    public void Initialiser(Image imageRadio)
    {
        _imageRadio = imageRadio;
    }
    public void AjouterRuneSurScene(Rune rune)
    {
        _runesSurScene.Add(rune);
    }

    public void EnleverRuneSurScene(Rune rune)
    {
        _runesSurScene.Remove(rune);
    }
    // Update is called once per frame
    void Update()
    {
        VerifierDistance();
    }

    void VerifierDistance()
    {
        if (_runesSurScene.Count == 0) return;

        float[] _distances = new float[_runesSurScene.Count];

        for (int i = 0; i < _runesSurScene.Count; i++)
        {
            _distances[i] = Vector3.Distance(_runesSurScene[i].transform.position, transform.position);
        }

        
        _donneesPerso.DistanceMinRune = Mathf.Min(_distances);
        // _donneesPerso.evenementMiseAJour.Invoke();
        if (_donneesPerso.DistanceMinRune < _donneesPerso.rangeDetection)
        {
            HandleRadioEffect(_donneesPerso.DistanceMinRune);
        }
        else
        {
            StopRadioEffect();
        }
    }

    void HandleRadioEffect(float distance)
    {
        float proximity = 1 - (distance / _donneesPerso.rangeDetection); // Closer means higher proximity

        // Adjust audio feedback
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = _clipRadio;
            _audioSource.Play();
        }
        _audioSource.volume = Mathf.Lerp(0.1f, 1.0f, proximity); // Louder as closer
        _audioSource.pitch = Mathf.Lerp(0.8f, 1.2f, proximity); // Higher pitch as closer

        // Adjust visual feedback (controlled blinking)
        CanvasGroup canvasGroup = _imageRadio.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = _imageRadio.gameObject.AddComponent<CanvasGroup>(); // Add CanvasGroup if not present
        }

        // Blink only if close enough
        // float blinkingThreshold = _rangeDetection / 2; // Start blinking when within half the detection range
        if (distance <= _donneesPerso.rangeDetection)
        // if (distance <= blinkingThreshold)
        {
            if (!_isBlinking)
            {
                // Calculate and lock in the blinking speed when entering the blinking range
                _currentBlinkSpeed = Mathf.Lerp(2.0f, 10.0f, proximity); // Faster blinking as closer
                _isBlinking = true; // Set blinking as active
            }

            // Use the locked-in blinking speed for consistent behavior
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * _currentBlinkSpeed)); // Oscillate alpha between 0 and 1
            canvasGroup.alpha = alpha;
        }
        else
        {
            _isBlinking = false; // Stop blinking
            canvasGroup.alpha = 1.0f; // Fully visible when not blinking
        }
    }

    void StopRadioEffect()
    {
        // Stop audio if it's playing
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        // Ensure the image is fully visible and not blinking
        CanvasGroup canvasGroup = _imageRadio.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1.0f;
        }

        _isBlinking = false; // Reset blinking state
    }
}