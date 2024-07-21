using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadOnQuestComplete : QuestInsertionBase
{
    // This class enables a transform child on the completion of a quest/component

    // Start is called before the first frame update
    void Start()
    {
        if (QuestSys.QuestList[totalQuestList[0]].getCompletionState() && QuestSys.QuestList[totalQuestList[0]].components[totalComponentList[0]].getCompleted())
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            QuestSys.insertFunc(attemptLoadChild);
        }
    }

    // Attempts to load child on load
    public void attemptLoadChild()
    {
        if (QuestSys.QuestList[totalQuestList[0]].getCompletionState() && QuestSys.QuestList[totalQuestList[0]].components[totalComponentList[0]].getCompleted())
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
