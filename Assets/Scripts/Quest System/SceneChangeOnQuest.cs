using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangeOnQuest : MonoBehaviour
{
    // In Charge of advancing scene on correct scene completion
    [SerializeField]
    string questLock;
    [SerializeField]
    string newScene;

    // Start is called before the first frame update
    void Start()
    {
        checkScene();
        QuestSys.insertFunc(checkScene);
    }

    public void checkScene()
    {
        if (QuestSys.QuestList[questLock].getCompletionState())
        {
            SceneManager.LoadScene(newScene);
        }
    }
}
