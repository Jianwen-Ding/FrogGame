using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountFrogDeadline : QuestInsertionBase
{
    // Allows for count frog quest to have a deadline before splitting into two possibilities
    [SerializeField]
    bool completeBranch = false;
    [SerializeField]
    int hourDeadline;
    string baseQuest;
    string succeeedQuest;
    string failQuest;
    string otherQuest;
    string otherComponent;
    // Start is called before the first frame update
    void Start()
    {
        baseQuest = totalQuestList[0];
        succeeedQuest = totalQuestList[1];
        failQuest = totalQuestList[2];
        otherQuest = totalQuestList[3];
        otherComponent = totalComponentList[3];
        // Self destructs if branch has already been crossed
        if (QuestSys.QuestList[succeeedQuest].getCompletionState() || QuestSys.QuestList[failQuest].getCompletionState())
        {
            Destroy(gameObject);
        }
        checkBaseSuccess();
        QuestSys.insertFunc(checkBaseSuccess);
    }

    public void checkBaseSuccess()
    {
        // If both branch quests are not fulfilled yet and base quest is complete
        // Fufill succeed quest, as that mean player fufilled base quest before deadline
        if (!completeBranch && !QuestSys.QuestList[failQuest].getCompletionState() && !QuestSys.QuestList[succeeedQuest].getCompletionState() && QuestSys.QuestList[baseQuest].getCompletionState())
        {
            completeBranch = true;
            foreach (string component in QuestSys.QuestList[succeeedQuest].components.Keys)
            {
                QuestSys.fufillComponentAttempt(succeeedQuest, component);
            }
            QuestSys.QuestList[failQuest].attemptDectivate();
            QuestSys.fufillComponentAttempt(otherQuest, otherComponent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If deadline is crossed, everything in the base and fail quest is automatically fufilled.
        // Gameobject is deleted soon afterwards
        if(!completeBranch && universalClock.mainGameTime.hours >= hourDeadline)
        {
            completeBranch = true;
            foreach (string component in QuestSys.QuestList[baseQuest].components.Keys)
            {
                QuestSys.fufillComponentAttempt(baseQuest, component);
            }
            foreach (string component in QuestSys.QuestList[failQuest].components.Keys)
            {
                QuestSys.fufillComponentAttempt(failQuest, component);
            }
            QuestSys.QuestList[succeeedQuest].attemptDectivate();
            QuestSys.fufillComponentAttempt(otherQuest, otherComponent);
        }
    }
}
