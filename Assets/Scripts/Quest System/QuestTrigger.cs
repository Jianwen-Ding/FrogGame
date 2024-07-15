using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestTrigger : QuestInsertionBase
{
    // In Charge of advancing components on player entering trigger
    [SerializeField]
    GameObject objectDelete;
    [SerializeField]
    int incrementAmount;

    // If player enters quest component advances
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (QuestSys.QuestList[totalQuestList[0]].components[totalComponentList[0]].incremental)
            {
                QuestSys.incrementComponentAttempt(totalQuestList[0], totalComponentList[0], incrementAmount);
            }
            else
            {
                QuestSys.fufillComponentAttempt(totalQuestList[0], totalComponentList[0]);
            }
            if(QuestSys.QuestList[totalQuestList[0]].getActivationState())
            {
                Destroy(objectDelete);
            }
        }
    }
}

