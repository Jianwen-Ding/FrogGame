using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fulfillQuestOnDiscover : QuestInsertionBase
{
    // Fulfills a quest if this entry is discovered
    [SerializeField]
    string speciesTitle;

    // Start is called before the first frame update
    void Start()
    {
        if (QuestSys.QuestList[totalQuestList[0]].getCompletionState())
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (speciesPage.pageList[speciesTitle].getTitleState())
        {
            QuestSys.fufillComponentAttempt(totalQuestList[0], totalComponentList[0]);
        }
    }
}
