using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.UI;

/// <summary>
/// Classe qui s'occupe de générer une ile selon un tableau 2D de coordonnées
/// Utilise le bruit de Perlin pour générer les hauteurs des cubes
/// </summary>
public class GenerateurIle : MonoBehaviour
{
    [SerializeField, Range(10, 1000)] int _largeur; // Définit la largeur de l'ile (axe des x)
    [SerializeField, Range(10, 1000)] int _profondeur; // Définit la profondeur de l'ile (axe des z)
    [SerializeField] GameObject _prefabCube; // Préfab de cube instancié pour le terrain de l'ile
    // [SerializeField] Renderer _textureRenderer; // Renderer du panel
    [SerializeField, Range(1.1f, 100f)] float _forcePerlin = 0.5f; // Amplitude de l'éffet perlin
    [SerializeField, Range(1.1f, 100f)] float _forcePerlinMeteo = 0.5f; // Amplitude de l'éffet perlin pour la carte meteo
    [SerializeField] float _magnitudeHauteur; // Détermine la hauteur des cubes
    [SerializeField, Range(2, 200)] float _k = 2f; //degré d'immersion de l'ile
    [SerializeField, Range(0, 1f)] float _c = 5f; //pourcentage de l'ile hors de l'eau
    [SerializeField] GameObject _prefabPerso; // Préfab du personnage blender
    [SerializeField] float _hauteurPerso; // Hauteur du personnage blender
    [SerializeField, Range(0, 100)] int _fertilite = 50; // Pourcentage de change qu'un item soit placé sur un cube
    [SerializeField, Range(0, 1f)] float ratioDecoItems = 0.5f; // Pourcentage de change qu'un item soit placé sur un cube
    List<List<Material>> _biomesMats = new List<List<Material>>();  // Liste de listes de matériaux pour les biomes
    List<List<GameObject>> _biomesItems = new List<List<GameObject>>();  // Liste de listes de items pour les biomes
    List<List<GameObject>> _biomeDecos = new List<List<GameObject>>();  // Liste de listes de items pour les biomes

    [SerializeField] TextMeshProUGUI _infoTexte;
    [SerializeField] int _nbEnnemis = 50;
    [SerializeField] int _nbAutels = 5;
    [SerializeField] int _radiusAutels = 10;
    [SerializeField] Slider _slider;
    [SerializeField] UIJeu _uiJeu;
    [SerializeField] UICraft _uiCraft;
    [SerializeField] SoundManager _soundManager;
    [SerializeField] SOPerso _donneesPerso;
    [SerializeField] Image _imageRadio;
    [SerializeField] FeuDeCamp _prefabFeuDeCamp;
    GameObject _instancePerso;
    List<BiomesEtatsManager> _biomeListe = new List<BiomesEtatsManager>();
    List<EnnemisEtatsManager> _ennemisListe = new List<EnnemisEtatsManager>();

    void Awake()
    {
        LoadResources(); // Appelle la fonction pour charger les matériaux
        GenererCarte(); // Appelle la fonction pour générer l'ile
        // Coroutine coroutine = StartCoroutine(InfosMonde());
        GetComponent<NavMeshSurface>().BuildNavMesh();

    }

    // Start is called before the first frame update
    void Start()
    {
        PlacerPerso(); // Appelle la fonction pour placer les personnages
        PlacerRunes();
        PlacerAutels();
    }

    IEnumerator CoroutineAjusterMusique()
    {
        while(true)
        {
            yield return new WaitForSeconds(2);
            List<EnnemisEtatsManager> ennemis = GetEtatEnnemis();
            
            if(ennemis.Count > 0)
            {
                _soundManager.ChangerMusique(2);
            }
            else if(_donneesPerso.sanite < 50)
            {
                _soundManager.ChangerMusique(1);
            }
            else
            {
                _soundManager.ChangerMusique(0);
            }
        }
    }

    List<EnnemisEtatsManager> GetEtatEnnemis()
    {
        List<EnnemisEtatsManager> templiste = new List<EnnemisEtatsManager>();
        foreach(EnnemisEtatsManager ennemi in _ennemisListe)
        {
            Debug.Log(ennemi.infos["etat"]);
            if (ennemi.infos["etat"] == "Chasse" || ennemi.infos["etat"] == "Attaque" || ennemi.infos["etat"] == "Etourdi")
            {
                // Debug.Log("Ennemi en chasse ou attaque");
                templiste.Add(ennemi);
            }
        }
        return templiste;
    }


    /// <summary>
    /// Charge les ressources de matériaux pour les biomes
    /// Les matériaux sont placés dans un dossier "mats" dans le dossier "Resources"
    /// Les matériaux sont nommés "bX_Y" où X est le numéro du biome et Y est le numéro de la variante
    /// </summary>
    void LoadResources()
    {
        int nbBiomes = 1; // Numéro du biome
        int nbVariants = 1; // Numéro de la variante
        bool resteDesMats = true; // Booléen pour savoir s'il reste des matériaux à charger
        List<Material> tpBiome = new List<Material>(); // Liste temporaire pour les matériaux d'un biome
        do // Boucle pour charger les matériaux, tant qu'il reste des matériaux à charger
        {
            // Charge le matériel selon le nom du fichier
            Material mats = (Material)Resources.Load("mats/b" + nbBiomes + "_" + nbVariants);
            if (mats) // Si le matériel est chargé avec succès
            {
                tpBiome.Add(mats); // Ajoute le matériel à la liste temporaire
                nbVariants++; // Incrémente le numéro de la variante
            }
            else // Si le matériel n'est pas chargé
            {
                if (nbVariants == 1) // Si le numéro de la variante est 1, cela signifie qu'il n'y a pas de matériel pour ce biome
                {
                    resteDesMats = false; // Il n'y a plus de matériaux à charger
                }
                else // Si le numéro de la variante est plus grand que 1, cela signifie qu'il n'y a plus de matériaux pour cette variante
                {
                    _biomesMats.Add(tpBiome); // Ajoute la liste temporaire à la liste de listes de matériaux
                    tpBiome = new List<Material>(); // Réinitialise la liste temporaire
                    nbBiomes++; // Incrémente le numéro du biome
                    nbVariants = 1; // Réinitialise le numéro de la variante
                }
            }

        } while (resteDesMats); // Tant qu'il reste des matériaux à charger

        int nbItems = 1; // Numéro de l'item
        int nbVariantsItem = 1; // Numéro de la variante de l'item
        bool resteDesItems = true; // Booléen pour savoir s'il reste des items à charger
        List<GameObject> tpItem = new List<GameObject>(); // Liste temporaire pour les items d'un biome
        do
        {
            GameObject items = (GameObject)Resources.Load("Items/i" + nbItems + "_" + nbVariantsItem);
            if (items)
            {
                tpItem.Add(items);
                nbVariantsItem++;
            }
            else
            {
                if (nbVariantsItem == 1)
                {
                    resteDesItems = false;
                }
                else
                {
                    _biomesItems.Add(tpItem);
                    tpItem = new List<GameObject>();
                    nbItems++;
                    nbVariantsItem = 1;
                }
            }
        } while (resteDesItems);

        int nbDeco = 1; // Numéro de l'item
        int nbVariantesDeco = 1; // Numéro de la variante de l'item
        bool resteDesDecos = true; // Booléen pour savoir s'il reste des items à charger
        List<GameObject> tpDeco = new List<GameObject>(); // Liste temporaire pour les items d'un biome
        do
        {
            GameObject decos = (GameObject)Resources.Load("deco/d" + nbDeco + "_" + nbVariantesDeco);
            if (decos)
            {
                tpDeco.Add(decos);
                nbVariantesDeco++;
            }
            else
            {
                if (nbVariantesDeco == 1)
                {
                    resteDesDecos = false;
                }
                else
                {
                    _biomeDecos.Add(tpDeco);
                    tpDeco = new List<GameObject>();
                    nbDeco++;
                    nbVariantesDeco = 1;
                }
            }
        } while (resteDesDecos);

    }



    // Fonction native de Unity, utilisé pour déboguer avec le panel
    void OnValidate()
    {
        // Activer pour déboguer avec le panel, important de déactiver la 
        // ligne qui instancie les cubes (ligne 62)
        // GenererCarte(); 
    }


    /// <summary>
    /// S'occupe d'instancier les cubes selon un tableau à deux dimensions, contenant les 3 coordonnés
    /// S'occupe aussi de l'affichage d'un panel, utilisé pour le débogage principalement
    /// </summary>
    /// <param name="ile">Contient les coordonées de chaque cube</param>
    void AfficherIle(float[,] ile, float[,] ileVariants, float[,] ileMeteo)
    {

        List<GameObject> _listeCubesValides = new List<GameObject>(); // Liste de cubes au-dessus de l'eau

        // On assigne la longueur des tableaux à des variables,
        // utilisé plus tard pour la boucle imbriqué
        float l = ile.GetLength(0);
        float p = ile.GetLength(1);

        //Créer une texture vide
        // Texture2D texture = new Texture2D(_largeur, _profondeur);

        // Boucle Imbriqué qui place les cube selon une grille dépendemment de la grandeur des tableaux
        for (int x = 0; x < l; x++)
        {
            for (int z = 0; z < p; z++)
            {
                // On cherche la valeur en y du cube spécifique
                float y = ile[x, z];

                // On instancie seulement un cube s'il est plus haut que 0
                if (y > 0)
                {
                    // On instancie un cube à une position selon les coordonnées x, y et z donné par le tableau généré

                    // le y est multiplié par une magnitude pour pouvoir rendre les cubes plus hauts, on ajoute aussi le transform de l'ile pour s'assurer que les cubes
                    // sont décalé avec l'ile parent
                    GameObject cube = Instantiate(_prefabCube, transform.position + new Vector3(x - _largeur / 2, y * _magnitudeHauteur, z - _profondeur / 2), Quaternion.identity, transform);

                    // On ajoute le cube à une liste de cubes valides
                    // _listeCubesValides.Add(cube);

                    // On assigne un biome au cube selon la valeur de y de la carte meteo
                    float yMeteo = ileMeteo[x, z];
                    // float adjustedValue = Mathf.Pow(yMeteo, 3);
                    // float adjustedValue =Mathf.Sin(yMeteo * Mathf.PI * 0.5f);
                    float adjustedValue = 1f / (1f + Mathf.Exp(-10f * (yMeteo - 0.5f))); ;
                    // float adjustedValue = Mathf.Pow(yMeteo * 2 - 1, 2) * 0.5f + 0.5f;
                    // On assigne un variant au cube selon la valeur de y de la carte variant
                    float yVariants = ileVariants[x, z];



                    // On assigne un biome au cube selon la valeur de y de la carte meteo
                    int indexMaterial = Mathf.RoundToInt(Mathf.Clamp(adjustedValue * (_biomesMats.Count - 1), 0, _biomesMats.Count - 1));
                    // int indexMaterial = Mathf.RoundToInt(Mathf.Clamp(y > 0.1f ? (yMeteo * (_biomesMats.Count - 1)) : 0, 0, _biomesMats.Count - 1));

                    // On assigne un variant au cube selon la valeur de y de la carte variant
                    int indexVariante = Mathf.RoundToInt(Mathf.Clamp(yVariants * (_biomesMats[indexMaterial].Count - 1), 0, _biomesMats[indexMaterial].Count - 1));

                    // cube.GetComponent<BiomesEtatsManager>().matBiome = indexMaterial + 1;
                    cube.GetComponent<BiomesEtatsManager>().infos.Add("biome", indexMaterial + 1);
                    cube.GetComponent<BiomesEtatsManager>().infos.Add("variant", indexVariante + 1);

                    cube.GetComponent<BiomesEtatsManager>().infos.Add("ile", this);
                    // cube.GetComponent<BiomesEtatsManager>().infos.Add("perso", _instancePerso);

                    _biomeListe.Add(cube.GetComponent<BiomesEtatsManager>());

                    cube.transform.eulerAngles = new Vector3(0, Random.Range(0, 4) * 90, 0);

                    // _biomeListe = Shuffle(_biomeListe);

                    // for (int i = 0; i < _biomeListe.Count; i++)
                    // {
                    //     GameObject unCube = _biomeListe[i].gameObject;
                    //     GameObject unEnnemi = Instantiate(Resources.Load<GameObject>("Ennemis/ennemi"), unCube.transform.position + new Vector3(0, 1, 0), Quaternion.identity, cube.transform);

                    //     unEnnemi.GetComponent<EnnemisEtatsManager>().infos.Add("maison", unCube);
                    //     unEnnemi.GetComponent<EnnemisEtatsManager>().infos.Add("ile", this);
                    //     unEnnemi.GetComponent<EnnemisEtatsManager>().infos.Add("perso", _prefabPerso);
                    // }

                    // cube.GetComponent<Transform>().localEulerAngles = new Vector3(0,Random.Range(0,4)*90,0);
                    // cube.GetComponent<BiomesEtatsManager>().matVariant = indexVariante + 1;

                    // Debug.Log("Biome: " + (indexMaterial + 1) + " Variant: " + (indexVariante + 1));

                    // On assigne le matériel au cube selon le biome et le variant
                    // cube.GetComponent<Renderer>().material = _biomesMats[indexMaterial][indexVariante];



                    if (Random.value * 100 <= _fertilite) // Si le cube est fertile selon un pourcentage, on place un item
                    {

                        // string type = Random.value > 0.5f ? "Items/i" : "deco/d";

                        string type = Random.value > ratioDecoItems ? "Item" : "deco";

                        if (type == "Item")
                        {
                            int indexVarianteItem = Mathf.RoundToInt(Mathf.Clamp(Random.value * (_biomesItems[indexMaterial].Count - 1), 0, _biomesItems[indexMaterial].Count - 1));
                            GameObject item = Resources.Load<GameObject>("Items/i" + (indexMaterial + 1) + "_" + (indexVarianteItem + 1));
                            // On instancie l'item selon le type, le biome et le variant
                            if (item)
                            {
                                GameObject instance = Instantiate(item, cube.transform.position + new Vector3(0, 0.55f, 0), Quaternion.Euler(item.transform.eulerAngles.x, Random.Range(0, 360), item.transform.eulerAngles.z), cube.transform);
                                float scale = Random.Range(0.8f, 1.2f); // on change la taille de l'item aléatoirement entre 0.8 et 1.2
                                instance.transform.localScale = new Vector3(scale, scale, scale);  // on applique la taille à l'item
                                cube.GetComponent<BiomesEtatsManager>().infos.Add("item", instance); // on ajoute l'item au dictionnaire du biome
                                // cube.GetComponent<BiomesEtatsManager>().infos.Add("deco", null); // on ajoute null à l'item du dictionnaire du biome
                            }
                        }
                        else
                        {
                            int indexVarianteDeco = Mathf.RoundToInt(Mathf.Clamp(Random.value * (_biomeDecos[indexMaterial].Count - 1), 0, _biomeDecos[indexMaterial].Count - 1));
                            GameObject deco = Resources.Load<GameObject>("deco/d" + (indexMaterial + 1) + "_" + (indexVarianteDeco + 1));
                            // cube.GetComponent<BiomesEtatsManager>().infos.Add("item", null); // on ajoute null à l'item du dictionnaire du biome
                            // On instancie l'item selon le type, le biome et le variant
                            if (deco)
                            {
                                GameObject instance = Instantiate(deco, cube.transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(deco.transform.eulerAngles.x, Random.Range(0, 360), deco.transform.eulerAngles.z), cube.transform);
                                float scale = Random.Range(0.8f, 1.2f); // on change la taille de l'item aléatoirement entre 0.8 et 1.2
                                instance.transform.localScale = new Vector3(scale, scale, scale);  // on applique la taille à l'item
                                cube.GetComponent<BiomesEtatsManager>().infos.Add("deco", instance); // on ajoute l'item au dictionnaire du biome
                            }

                        }

                    }



                }

                // Applique un pixel sur la texture à une coordonné spécifique
                // texture.SetPixel(x, z, new Color(y, y, y));

            }
        }


    }

    public List<BiomesEtatsManager> Shuffle(List<BiomesEtatsManager> liste)
    {
        for (int i = 0; i < liste.Count; i++)
        {
            BiomesEtatsManager temp = liste[i];
            int randomIndex = Random.Range(i, liste.Count);
            liste[i] = liste[randomIndex];
            liste[randomIndex] = temp;
        }
        return liste;
    }


    /// <summary>
    /// Génère le tableau 2D de coordonnés
    /// </summary>
    void GenererCarte()
    {
        int l = _largeur + _donneesPerso.niveau * 10;
        int p = _profondeur + _donneesPerso.niveau * 10;

        // appelle de la fonction avec les propriétés pour la dimensions de l'ile et l'amplitude de l'effet perlin
        float[,] uneCarte = Terraformer(l, p, _forcePerlin);


        // fonction qui change les y en forme rectangulaire
        // uneCarte = Aquaformer(uneCarte);

        // fonction qui change les y en forme circulaire
        uneCarte = AquaformerFormeCirculaire(uneCarte);

        // Carte pour appliquer des patchs de variantes a l'interieur d'un biome
        float[,] uneCarteVariants = Terraformer(l, p, _forcePerlin / 2);

        // Carte pour appliquer des patchs de biome sur l'ile
        float[,] uneCarteMeteo = Terraformer(l, p, _forcePerlinMeteo);

        // Envoie les cartes à la fonction pour afficher l'ile
        AfficherIle(uneCarte, uneCarteVariants, uneCarteMeteo);

    }

    /// <summary>
    /// Genère de manière brute aléatoirement une ile avec des valeurs en y aléatoire
    /// </summary>
    /// <param name="largeur">largeur de l'ile</param>
    /// <param name="profondeur">profondeur de l'ile</param>
    /// <param name="fp">abbréviation pour force perlin</param>
    /// <returns>Renvoie un tableau 2D contenant les coordonnées en x et z comme clé et y comme valeur</returns>
    float[,] Terraformer(int largeur, int profondeur, float fp)
    {

        // Déclaration du tableau 2D
        float[,] terrain = new float[largeur, profondeur];

        // nombre aléatoire pour avoir des maps aléatoires plus tard, puisque le perlin est généré une fois par Play
        int newNoise = Random.Range(0, 10000);

        // boucle imbriqué
        for (int x = 0; x < largeur; x++)
        {
            for (int z = 0; z < profondeur; z++)
            {
                // perlin calcul
                float y = Mathf.PerlinNoise(x / fp + newNoise, z / fp + newNoise);


                // affecte perlin à la carte
                terrain[x, z] = y;
            }
        }

        // retourne le terrain
        return terrain;
    }

    /// <summary>
    /// On ajuste les valeurs de y pour que les extrémités de l'ile soient plus basses
    /// et que le centre soit plus haut, l'ile sera en forme de cercle
    /// </summary>
    /// <param name="terrain">tableau 2D des coordonnées de la carte</param>
    /// <returns>Retourne le terrain modifié</returns>
    float[,] AquaformerFormeCirculaire(float[,] terrain)
    {
        // On assigne la longueur des tableaux à des variables,
        // utilisé plus tard pour la boucle imbriqué
        int l = terrain.GetLength(0);
        int p = terrain.GetLength(1);

        //boucle imbriqué pour ajuster le y
        for (int x = 0; x < l; x++)
        {
            for (int z = 0; z < p; z++)
            {
                // On détermine la position du cube en x et z selon sa distance par rapport au centre
                float dx = x / (float)l * 2 - 1;
                float dz = z / (float)p * 2 - 1;

                // On passe les valeurs dans l'équation de pythagore pour obtenir la distance du centre
                // ce qui donnera une forme circulaire à l'ile
                float val = Mathf.Sqrt(dx * dx + dz * dz);

                // On envoie la valeur à la fonction logistique
                val = Logistique(val);

                // On modifie la valeur de y du cube selon la valeur de val, en s'assurant que la valeur reste entre 0 et 1
                terrain[x, z] = Mathf.Clamp01(terrain[x, z] - val);
            }
        }

        // retourne le terrain
        return terrain;
    }

    /// <summary>
    /// Place le personnage sur l'ile
    /// seulement sur les cubes au-dessus de l'eau
    /// </summary>
    void PlacerPerso()
    {

        // On prend un cube aléatoire dans la liste de cubes valides
        // GameObject cube = cubesValides[Random.Range(0, cubesValides.Count)];
        _biomeListe = Shuffle(_biomeListe);
        BiomesEtatsManager cube = _biomeListe[0];

        // On s'assure que le personnage ne soit pas placé sur un cube dangereux
        while (cube.infos["biome"] == 1 || cube.infos.ContainsKey("deco") || cube.infos.ContainsKey("item"))
        {
            _biomeListe = Shuffle(_biomeListe);
            cube = _biomeListe[0];
        }

        // On instancie un personnage sur le cube
        GameObject perso = Instantiate(_prefabPerso, cube.transform.position + new Vector3(0, _hauteurPerso, 0), Quaternion.identity, transform);

        perso.GetComponent<Perso>().Initialiser(_uiJeu, _soundManager, _uiCraft);
        perso.GetComponent<Radio>().Initialiser(_imageRadio);
        // _camera.Init(perso);
        // _uiJeu.Init(_cameraUI);
        _instancePerso = perso;
        FeuDeCamp feuDeCamp = Instantiate(_prefabFeuDeCamp, cube.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity, cube.transform);
        feuDeCamp.Init(10000, perso.GetComponent<Perso>());
        perso.GetComponent<Perso>().lFeuxDeCamp.Add(feuDeCamp);
        // perso.GetComponent<Perso>().sliderSanite = _slider;
        // perso.GetComponent<Perso>().canvasInventaire = _canvasInventaire;

        foreach (BiomesEtatsManager biome in _biomeListe)
        {
            biome.infos.Add("perso", perso);
            if (biome.infos.ContainsKey("item"))
            {
                biome.infos["item"].GetComponent<Ressource>().Init(perso, biome);
            }
        }
        // _instancePerso = perso;
        // On enlève le cube de la liste pour ne pas placer deux personnages sur le même cube
        // cubesValides.Remove(cube);

        // On fait en sorte que le personnage regarde dans une direction aléatoire
        perso.transform.Rotate(0, Random.Range(0, 360), 0);

        // StartCoroutine(CoroutinePlacerEnnemis(perso.transform));
        PlacerEnnemis(perso.transform);

    }

    // Mis en Coroutine pour attendre un peu avant de placer les ennemis pour éviter les bugs
    void PlacerEnnemis(Transform perso)
    // IEnumerator CoroutinePlacerEnnemis(Transform perso)
    {
        // yield return new WaitForSeconds(1);
        List<BiomesEtatsManager> tempListe = new List<BiomesEtatsManager>(_biomeListe);
        tempListe = Shuffle(tempListe);


        GameObject prefabEnnemi = Resources.Load<GameObject>("Ennemis/ennemi");

        int enemiesToSpawn = _nbEnnemis + _donneesPerso.niveau * 3;
        int attempts = 0;
        int maxAttempts = tempListe.Count * 2; // Arbitrary large number to prevent infinite loop

        for (int i = 0; i < enemiesToSpawn && attempts < maxAttempts; attempts++)
        {
            GameObject cube = tempListe[Random.Range(0, tempListe.Count)].gameObject;

            // On s'assure que les ennemis ne sont pas placés trop près du personnage
            if (Vector3.Distance(cube.transform.position, perso.position) < 20)
            {
                continue;
            }

            GameObject ennemi = Instantiate(prefabEnnemi, cube.transform.position + new Vector3(0, 1, 0), Quaternion.identity, cube.transform);
            _ennemisListe.Add(ennemi.GetComponent<EnnemisEtatsManager>());
            ennemi.GetComponent<EnnemisEtatsManager>().infos.Add("maison", cube.transform);
            ennemi.GetComponent<EnnemisEtatsManager>().infos.Add("ile", this);
            ennemi.GetComponent<EnnemisEtatsManager>().infos.Add("perso", perso);
            // Debug.Log(cube.transform.position);
            tempListe.Remove(cube.GetComponent<BiomesEtatsManager>());
            i++;
        }
        StartCoroutine(CoroutineAjusterMusique());
    }
    
    /// <summary>
    /// fonction basé sur la fonction Sigmoide, en son état logistique
    /// retourne un nombre selon l'équation.
    /// </summary>
    /// <param name="value"> la variable x de la fonction</param>
    /// <returns>Une valeur selon l'équation</returns>
    float Logistique(float value)
    {
        // retourne la valeur selon l'équation logistique
        return 1 / (1 + Mathf.Exp(-_k * (value - _c)));
    }

    private void PlacerRunes()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject rune = Resources.Load<GameObject>("Rune/r" + (i + 1));
            _biomeListe = Shuffle(_biomeListe);
            BiomesEtatsManager cube = _biomeListe[0];
            while (Vector3.Distance(cube.transform.position, _instancePerso.transform.position) < 20f || cube.infos.ContainsKey("item") || cube.infos.ContainsKey("deco"))
            {
                _biomeListe = Shuffle(_biomeListe);
                cube = _biomeListe[0];
            }
            GameObject instance = Instantiate(rune, cube.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity, cube.transform);
            instance.GetComponent<Rune>().Init(_instancePerso, cube);
            _instancePerso.GetComponent<Radio>().AjouterRuneSurScene(instance.GetComponent<Rune>());
            cube.infos.Add("item", instance);
            // cle.transform.Rotate(-90, 0, 0);
            // cube.infos.Add("cle", cle);
        }
    }

    private void PlacerAutels()
    {
        if (_instancePerso == null)
        {
            return;
        }

        Vector3 playerPosition = _instancePerso.transform.position; // Get the player's position

        float angleStep = 360f / _nbAutels; // Divide the circle evenly

        for (int i = 0; i < _nbAutels; i++)
        {
            // Compute the angle for this autel
            float angle = i * angleStep;
            float radian = Mathf.Deg2Rad * angle;

            // Compute the target position using polar coordinates
            Vector3 targetPosition = playerPosition + new Vector3(
                _radiusAutels * Mathf.Cos(radian),
                0,
                _radiusAutels * Mathf.Sin(radian)
            );

            // Find the nearest valid block in _biomeListe
            BiomesEtatsManager closestBlock = null;
            float closestDistance = float.MaxValue;

            foreach (BiomesEtatsManager cube in _biomeListe)
            {
                if (cube.infos.ContainsKey("item") || cube.infos.ContainsKey("deco"))
                    continue; // Skip occupied blocks

                float distance = Vector3.Distance(cube.transform.position, targetPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlock = cube;
                }
            }

            if (closestBlock != null)
            {
                // Load and instantiate the pillar
                GameObject pillar = Resources.Load<GameObject>("Pillar/p" + (i + 1));
                GameObject autel = Instantiate(pillar, closestBlock.transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity, closestBlock.transform);
                autel.GetComponent<Pillier>().Init(_instancePerso, _uiJeu);

                closestBlock.infos.Add("deco", autel);

                Vector3 directionToPlayer = _instancePerso.transform.position - autel.transform.position;
                directionToPlayer.y = 0; // Ignore vertical difference to keep x and z rotations unchanged.
                autel.transform.rotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

                // Optionally, apply a fixed x rotation adjustment if needed (e.g., -90 for horizontal alignment).
                autel.transform.Rotate(-90, 0, 0);
            }
            else
            {
                Debug.LogWarning($"No valid block found near position {targetPosition} for autel {i + 1}.");
            }
        }
    }

}