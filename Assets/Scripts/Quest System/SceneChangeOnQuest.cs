using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangeOnQuest : QuestInsertionBase
{
    // In Charge of advancing scene on correct scene completion
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
        if (QuestSys.QuestList[totalQuestList[0]].getCompletionState())
        {
            SceneManager.LoadScene(newScene);
        }
    }
}
