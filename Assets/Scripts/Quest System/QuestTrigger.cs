using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    // In Charge of advancing components on player entering trigger
    [SerializeField]
    string questAdvance;
    [SerializeField]
    string componentAdvance;
    [SerializeField]
    GameObject objectDelete;
    [SerializeField]
    int incrementAmount;

    // If player enters quest component advances
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (QuestSys.QuestList[questAdvance].components[componentAdvance].incremental)
            {
                QuestSys.incrementComponentAttempt(questAdvance, componentAdvance, incrementAmount);
            }
            else
            {
                QuestSys.fufillComponentAttempt(questAdvance, componentAdvance);
            }
            if(QuestSys.QuestList[questAdvance].getActivationState())
            {
                Destroy(objectDelete);
            }
        }
    }
}
