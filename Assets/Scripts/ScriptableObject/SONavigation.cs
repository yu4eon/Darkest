using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// ScriptableObject qui permet de gérer la navigation entre les scènes du jeu
/// </summary>
[CreateAssetMenu(fileName = "Navigation", menuName = "ScriptableObject/Navigation")]
public class SONavigation : ScriptableObject
{
    [SerializeField] SOPerso _donneesPerso;
    
    /// <summary>
    /// Méthode appelée pour démarrer le jeu.
    /// </summary>
    public void Jouer()
    {
        _donneesPerso.Initialiser(); // Initialise les données du personnage
        _donneesPerso.ResetNiveau(); // Réinitialise le niveau du personnage
        SceneManager.LoadScene("Niveau");
    }
    /// <summary>
    /// Méthode appelée pour retourner au menu.
    /// </summary>
    public void AllerMenu()
    {
        _donneesPerso.Initialiser(); // Initialise les données du personnage
        _donneesPerso.ResetNiveau(); // Réinitialise le niveau du personnage
        SceneManager.LoadScene("SceneTitre");
    }

    /// <summary>
    /// Méthode appelée pour aller au niveau suivant.
    /// </summary>
    public void AllerProchainNiveau()
    {
        _donneesPerso.niveau++; // Incrémente le niveau du personnage
        SceneManager.LoadScene("Niveau");
    }

    /// <summary>
    /// Méthodes appelées pour charger la scène précédente dans l'ordre de la build.
    /// </summary>
    public void AllerScenePrecedente()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // Charge la scène précédente dans l'ordre de la build
    }

    /// <summary>
    /// Méthode appelée pour charger la scène de tutoriel.
    /// </summary>
    public void AllerSceneTutoriel()
    {
        SceneManager.LoadScene("Tutoriel");
    }

    /// <summary>
    /// Méthode appelée pour charger la scène de fin de jeu.
    /// </summary>
    public void AllerSceneWin()
    {
        SceneManager.LoadScene("SceneWin");
    }


    /// <summary>
    /// Méthode appelée pour charger la scène suivante dans l'ordre de la build.
    /// </summary>
    public void AllerSceneSuivante()
    {
    
        int indexSceneCourante = SceneManager.GetActiveScene().buildIndex;
        int indexSceneSuivante = indexSceneCourante + 1;
        if (indexSceneSuivante < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(indexSceneSuivante);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
