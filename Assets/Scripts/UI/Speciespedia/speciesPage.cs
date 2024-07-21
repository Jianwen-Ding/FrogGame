using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speciesPage : MonoBehaviour
{
    // This page controls which parts of the speciespedia are currently active
    #region variables
    [Header("Save Variabes")]
    // Controls how the page saves
    [SerializeField]
    string title;

    [Header("Cache Variabes")]
    // Controls which gameobjects to turn off or on
    [SerializeField]
    GameObject titleBox;
    [SerializeField]
    GameObject confusionBox;
    [SerializeField]
    GameObject animalImage;
    [SerializeField]
    GameObject footPrints;
    [SerializeField]
    GameObject sound;
    [SerializeField]
    GameObject[] funFacts;
    [SerializeField]
    GameObject[] preyPredFacts;

    [Header("Access Variables")]
    // Variables that allow other components to access the page

    public static Dictionary<string, speciesPage> pageList;
    #endregion
    #region functions

    // Init is called before the first frame update
    public void init()
    {
        updateState();
        pageList.Add(title, this);
    }

    // >>> GET/SET of page states<<<
    // Title
    public bool getTitleState()
    {
        return PlayerPrefs.GetInt(title + "titleBox", 0) == 1;
    }
    public void activateTitle()
    {
        PlayerPrefs.SetInt(title + "titleBox", 1);
        updateState();
    }

    // Image
    public void activateImage()
    {
        PlayerPrefs.SetInt(title + "animalImage", 1);
        updateState();
    }
    public bool getImageState()
    {
        return PlayerPrefs.GetInt(title + "animalImage", 0) == 1;
    }

    // Footprints
    public void activateFootPrints()
    {
        PlayerPrefs.SetInt(title + "footPrints", 1);
        updateState();
    }
    public bool getFootPrintsState()
    {
        return PlayerPrefs.GetInt(title + "footPrints", 0) == 1;
    }

    // Sound
    public void activateSound()
    {
        PlayerPrefs.SetInt(title + "sound", 1);
        updateState();
    }
    public bool getSoundState()
    {
        return PlayerPrefs.GetInt(title + "sound", 0) == 1;
    }

    // Fun facts
    public void activateFunFacts(int i)
    {
        PlayerPrefs.SetInt(title + "funFacts" + i, 1);
        updateState();
    }
    public bool getFunFactsState(int i)
    {
        return PlayerPrefs.GetInt(title + "funFacts" + i, 0) == 1;
    }

    // Prey Pred
    public void activatePreyPred(int i)
    {
        PlayerPrefs.SetInt(title + "preyPredFacts" + i, 1);
        updateState();
    }
    public bool getPreyPredState(int i)
    {
        return PlayerPrefs.GetInt(title + "preyPredFacts" + i, 0) == 1;
    }

    // Saves state of page as playerprefs
    // MAKE SURE TO CALL THIS AFTER EDITING STATE
    // 1 is true
    // 0 is false
    public void saveState()
    {
        PlayerPrefs.SetInt(title + "titleBox", titleBox.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt(title + "animalImage", animalImage.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt(title + "footPrints", footPrints.activeSelf ? 1 : 0);
        PlayerPrefs.SetInt(title + "sound", sound.activeSelf ? 1 : 0);
        for(int i = 0; i < funFacts.Length; i++)
        {
            PlayerPrefs.SetInt(title + "funFacts" + i, funFacts[i].activeSelf ? 1 : 0);
        }
        for(int i = 0; i < preyPredFacts.Length; i++)
        {
            PlayerPrefs.SetInt(title + "preyPredFacts" + i, preyPredFacts[i].activeSelf ? 1 : 0);
        }
    }

    // Updates states of page through playerprefs
    // 1 is true
    // 0 is false
    public void updateState()
    {
        titleBox.SetActive(PlayerPrefs.GetInt(title + "titleBox", 0) == 1);
        confusionBox.SetActive(PlayerPrefs.GetInt(title + "titleBox", 0) == 0);
        animalImage.SetActive(PlayerPrefs.GetInt(title + "animalImage", 0) == 1);
        footPrints.SetActive(PlayerPrefs.GetInt(title + "footPrints", 0) == 1);
        sound.SetActive(PlayerPrefs.GetInt(title + "sound", 0) == 1);
        for (int i = 0; i < funFacts.Length; i++)
        {
            funFacts[i].SetActive(PlayerPrefs.GetInt(title + "funFacts" + i, 0) == 1);
        }
        for (int i = 0; i < preyPredFacts.Length; i++)
        {
            preyPredFacts[i].SetActive(PlayerPrefs.GetInt(title + "preyPredFacts" + i, 0) == 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
