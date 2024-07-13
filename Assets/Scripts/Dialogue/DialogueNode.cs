using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [Header("Text Component")]
    public string dialogueText;
    public List<DialogueResponse> responses;

    [Header("Quest Completion Component")]
    // If the piece of dialouge advances a component of a quest
    public bool advancesComponent;
    public string questLinked;
    public string componentLinked;

    internal bool IsLastNode()
    {
        for(int i = 0; i < responses.Count; i++)
        {
            if (!responses[i].locked())
            {
                return false;
            }
        }
        return true;
    }
}
