using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance {get; private set;}

    // UI Reference
    public GameObject DialogueParent;                                       // Main containter for dialogue UI
    public TextMeshProUGUI DialogueTitleText, DialogueBodyText;             // Text components for title and body
    public GameObject responseButtonPrefab;                                 // Prefab for generating response buttons
    public List<GameObject> responseButtons = new List<GameObject>();       // Container to hold repsonse buttons
    public Transform[] ResponseList;                                        // The locations where response buttons end up
    private void Awake()
    {
        // Singleton patter to ensure only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initially hide the dialogue UI
        HideDialogue();
    }

    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(string title, DialogueNode node)
    {
        Debug.LogError(node);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Display the dialogue UI
        ShowDialogue();

        // Set dialogue title and body text
        DialogueTitleText.text = title;
        DialogueBodyText.text = node.dialogueText;

        // Increments quest if dialogue is set to do so
        if (node.advancesComponent)
        {
            QuestSys.fufillComponentAttempt(node.questLinked, node.componentLinked);
        }

        foreach (GameObject child in responseButtons)
        {
            Destroy(child.gameObject);
        }

        // Create and stup response buttons based on current dialogue node
        int iter = 0;
        foreach (DialogueResponse response in node.responses)
        {
            Debug.Log(response);
            // Does not have any effect on press
            GameObject buttonObject = Instantiate(responseButtonPrefab, ResponseList[iter]);
            responseButtons.Add(buttonObject);
            if (response.locked())
            {
                buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = " < Complete |" + response.questNeeded + "| To  Unlock>";
                buttonObject.GetComponent<Button>().interactable = false;
            }
            else
            {
                buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

                // Setup button to trigger SelectResponse when clicked
                buttonObject.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title));
            }
            iter += 1;
        }
    }

    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response, string title)
    {
        // Check if there is a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode);    //Start next dialogue
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
        }
    }

    public void HideDialogue()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        DialogueParent.SetActive(false);
    }

    public void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }

    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
}
