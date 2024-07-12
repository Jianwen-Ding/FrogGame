using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue2 Dialogue;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpeakTo();
        }
    }

    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
    }
}
