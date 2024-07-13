using UnityEngine;

[System.Serializable]
public class DialogueResponse
{
    [Header("Quest Text Component")]
    public string responseText;
    public DialogueNode nextNode;
    [Header("Quest Locked Component")]
    // If response is locked under a quest
    public bool lockedBehindQuest;
    public string questNeeded;

    public bool locked()
    {
        if(lockedBehindQuest && !QuestSys.QuestList[questNeeded].getCompletionState())
        {
            return true;
        }
        return false;
    }
}
