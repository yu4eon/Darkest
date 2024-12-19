using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleRessource : MonoBehaviour
{
    GameObject _ressourcePrefab;
    public GameObject RessourcePrefab { get => _ressourcePrefab; set => _ressourcePrefab = value; }

    private Transform _player;
    public Transform Player { get => _player; set => _player = value; }


    // Start is called before the first frame update
    void Start()
    {
        Instantiate(_ressourcePrefab, transform.position, Quaternion.identity);

    }

    private IEnumerator MoveSpriteToPlayer(GameObject ressource, Transform _player)
    {
        float duration = 1f; // Time it takes for sprite to reach player
        Vector3 startPosition = ressource.transform.position;
        Vector3 endPosition = _player.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            ressource.transform.position = Vector3.Lerp(startPosition, endPosition, t); // Moves the sprite toward the player
            yield return null;
        }

        Destroy(ressource); // Destroy the sprite once it reaches the player
    }
}
