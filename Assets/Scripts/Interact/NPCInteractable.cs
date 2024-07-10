using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    [SerializeField] private string interactText;

    public void Interact()
    {
        Debug.Log("Interact");
    }

    public string GetInteractText()
    {
        return interactText;
    }
}
