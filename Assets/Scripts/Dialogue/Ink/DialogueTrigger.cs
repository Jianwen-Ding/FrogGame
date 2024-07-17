using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : QuestInsertionBase
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Dialogue Options")]
    // The role of the quest that the dialogue is locked behind
    // If dialogue is locked behind quest, dialogue will only play if quest is complete  
    [SerializeField] string dialogueLockRole;
    // The role of the quest that the dialogue advances.
    // DON'T PUT INCREMENTAL QUESTS HERE
    [SerializeField] string questAdvanceRole;
    // The priority of the quest
    // The highest priority dialogue option that can be played will play
    [SerializeField] int[] dialoguePriority;
    // The actual dialogue that is played
    [SerializeField] private TextAsset[] inkJSONs;

    [Header("Stored Vars")]
    // Locked stored dialogue
    List<string> lockedQuests = new List<string>();
    // Whether quest field has been filled
    // If it is empty then the dialogue is not locked
    List<bool> lockedQuestAvalability = new List<bool>();

    // To advance stored dialogue
    List<string> advanceQuests = new List<string>();
    List<string> advanceQuestComponents = new List<string>();
    // Whether quest field has been filled
    // If quest is empty nothing happens
    List<bool> advanceQuestAvalability = new List<bool>();
    private static bool playerInRange = false;

    private void Awake()
    {
        visualCue = GameObject.FindGameObjectWithTag("Interactable");
        playerInRange = false;
        // Caches the locked quest requirements and quests to advance
        // Also stores if empty or not
        for(int i = 0; i < questsNeededRoles.Length; i++)
        {
            if (questsNeededRoles[i] == dialogueLockRole)
            {
                string quest = totalQuestList[i];
                lockedQuests.Add(quest);
                lockedQuestAvalability.Add(quest != emptyConst);
            }
            else if (questsNeededRoles[i] == questAdvanceRole)
            {
                string quest = totalQuestList[i];
                advanceQuests.Add(quest);
                string component = totalComponentList[i];
                advanceQuestComponents.Add(component);
                advanceQuestAvalability.Add(quest != emptyConst && component != emptyConst);
            }
        }
    }

    private void Start()
    {
        visualCue.SetActive(false);
    }

    // Plays on dialogue end
    public void questAdvance(string quest, string component)
    {
        QuestSys.QuestList[quest].components[component].fufillComponent();
    }


    private void Update()
    {
        if (playerInRange && !DialogueManager2.GetInstance().dialogueIsPlaying)
        {
            DebugDisplay.updateDisplay(gameObject.name + " in range", "true");
            // Searches for highest valid priority option
            int highestPrio = -1;
            int highestPrioIndex = -1;
            for (int i = 0; i < dialoguePriority.Length; i++)
            {
                bool avaliable;
                if (lockedQuestAvalability[i])
                {
                    avaliable = QuestSys.QuestList[lockedQuests[i]].getCompletionState();
                }
                else
                {
                    avaliable = true;
                }
                if (avaliable && highestPrio < dialoguePriority[i])
                {
                    highestPrio = dialoguePriority[i];
                    highestPrioIndex = i;
                }
            }
            if(highestPrioIndex != 1)
            {
                visualCue.SetActive(true);
                if (Input.GetKeyUp(KeyCode.F))
                {
                    DialogueManager2.playOnEnd newFunc = null;
                    // Fufills quest at index
                    if (advanceQuestAvalability[highestPrioIndex])
                    {
                        void plug()
                        {
                            questAdvance(advanceQuests[highestPrioIndex], advanceQuestComponents[highestPrioIndex]);
                        }
                        newFunc = plug;
                    }
                    DialogueManager2.GetInstance().EnterDialogueMode(inkJSONs[highestPrioIndex], newFunc);
                }
            }
        }
        else
        {
            DebugDisplay.updateDisplay(gameObject.name + " in range", "false");
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
