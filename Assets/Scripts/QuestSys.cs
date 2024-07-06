using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSys : MonoBehaviour
{
    #region vars
    // Main quest system
    // Non public variables meant to only be modified specific ways
    public class Quest
    {
        #region psuedocode
        // The quests are mainly comprised of three parts in game play
        // 1 - Clues - Clues activate information, certain clues are locked by other information
        // 2 - Component - If a clue is discovered and all the prerequisite information is found, the corresponding
        // Information piece is activated. This information peice is logged in speciestopia and allows the player to analyze other previously
        // Locked clues.
        // 3 - Final - Once a certain set of information is found the missions is complete and the player
        // is give a overview of the situation and/or a extra tool in regards to catching the animal.
        #endregion
        #region vars
        // Whether the quest is part of the main story
        public bool mainLine;
        // Title of the quest
        public string QuestDescription;
        // Whether components have been fufilled
        bool completed;
        // Whether the quest has been activated or not
        bool activated;

        // Quests that will potentially activate on completion of quest
        public List<Quest> nextQuests;
        // Quests that need to be activated in order to attempt activation of quest
        public List<Quest> previousQuests;

        // Stores all effects of the 
        // Functions that will fire off upon quest being completed
        // Meant to be inserted from the outside
        public delegate void onComponentFufill();
        List<onComponentFufill> fufillEffects;

        // This stores all of the information of each component
        public class Component
        {
            // >>> VARIABLES <<<
            // The description of the component
            public string description;

            // The Quest that the component is attached to
            public Quest attachedQuest;

            // Whether the title is incremental or not
            // Incremental IE: Get frogs 5/10
            // Non-incremental IE: Get frog [Done/Not Done]
            public bool incremental;

            // How much the component has been incremented
            int gatheredAmount;

            // The amount until the component will be completed
            public int totalAmount;

            // Whether the player has fufilled this component
            private bool completed;

            // Functions that will fire off upon component being fufilled
            // Meant to be inserted from the outside
            public delegate void onComponentFufill();
            List<onComponentFufill> fufillEffects;

            // >>> FUNCTIONS <<<
            // Increments component
            public void increment(int amount)
            {
                if (incremental)
                {
                    gatheredAmount += amount;
                    if(gatheredAmount >= totalAmount)
                    {
                        completed = true;
                    }
                }
                else
                {
                    print("ERROR- tried to increment when not incremental");
                }
            }
            // Fufills the component
            public void fufillComponent()
            {
                completed = true;
                for(int i = 0; i < fufillEffects.Count; i++)
                {
                    fufillEffects[i]();
                }
                attachedQuest.attemptComplete();
            }

            // Inserts effect on component completion
            public void insertFufillEffect(onComponentFufill newFunc)
            {
                fufillEffects.Add(newFunc);
            }
            // Gathered amount accessed here
            public int getGathered()
            {
                return gatheredAmount;
            }

            // Finds if component has been completed yet
            public bool getCompleted()
            {
                return completed;
            }

        }
        [SerializeField]
        public Dictionary<string, Component> components = new Dictionary<string, Component>();
        #endregion
        #region functions
        //public void activateComponent(string infoKey)
        //{
        //    components[infoKey] = ;
        //}

        // Checks if every single component has been completed or not
        // Updates completion status accordingly
        public void attemptComplete()
        {
            // Checks every component for completion
            bool allComplete = true;
            foreach(string label in components.Keys)
            {
                if (components[label].getCompleted())
                {
                    allComplete = false;
                }
            }
            completed = allComplete || completed;
            // Attempts to activate next quests
            for(int i = 0; i < nextQuests.Count; i++)
            {
                nextQuests[i].attemptActivate();
            }
            // Activates all fufill effects
            for(int i = 0; i < fufillEffects.Count; i++)
            {
                fufillEffects[i]();
            }
        }

        // Checks if every proceeding quest has been activated or not
        public void attemptActivate()
        {
            bool allComplete = true;
            for(int i = 0; i < previousQuests.Count; i++)
            {
                if (!previousQuests[i].getCompletionState())
                {
                    allComplete = false;
                }
            }
            activated = activated || allComplete;
        }

        // Returns whether quest has been activated
        public bool getActivationState()
        {
            return activated;
        }

        // Returns whether components have been fuffilled
        public bool getCompletionState()
        {
            return completed;
        }

        // Inserts effect on component completion
        public void insertFufillEffect(onComponentFufill newFunc)
        {
            fufillEffects.Add(newFunc);
        }
        #endregion
    }

    // Full list of all quests
    public static Dictionary<string, Quest> QuestLists;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
