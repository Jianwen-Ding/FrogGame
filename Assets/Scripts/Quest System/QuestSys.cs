using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class QuestSys : MonoBehaviour
{
    
    #region variables
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
        // Description of the quest
        public string questDescription;
        // What will change on completion of quest
        public string questCompletionEffect;
        // Whether components have been fufilled
        bool completed;
        // Whether the quest has been activated or not
        bool activated;

        // Quests that will potentially activate on completion of quest
        public List<Quest> nextQuests;
        // Quests that need to be activated in order to attempt activation of quest
        public List<Quest> previousQuests;

        // This stores all of the information of each component
        public class QuestComponent
        {
            // >>> VARIABLES <<<
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
            bool completed;

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
                attachedQuest.attemptComplete();
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
            // Updates state of component
            public void updateState(int amount, bool complete)
            {
                gatheredAmount = amount;
                completed = complete;
            }
            // >>> CONSTRUCTOR <<<
            public QuestComponent(Quest attQuest, bool increm, int amountNeeded, int amount, bool complete)
            {
                attachedQuest = attQuest;
                incremental = increm;
                totalAmount = amountNeeded;
                gatheredAmount = amount;
                completed = complete;
            }
        }
        [SerializeField]
        public Dictionary<string, QuestComponent> components = new Dictionary<string, QuestComponent>();
        #endregion
        #region functions
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
            if (completed) {
                // Attempts to activate next quests
                for (int i = 0; i < nextQuests.Count; i++)
                {
                    nextQuests[i].attemptActivate();
                }
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

        // Inserts components
        public void insertComponents(Dictionary<string ,QuestComponent> set)
        {
            components = set;
        }
        // Updates the state of a quest
        public void updateState(bool complete, bool active)
        {
            completed = complete;
            activated = active;
        }
        #endregion
        #region constructor
        // Full construction
        public Quest(bool main, string description, string completionEff, bool complete, bool active, List<Quest> next, List<Quest> prev, Dictionary<string, QuestComponent> comps)
        {
            mainLine = main;
            questDescription = description;
            questCompletionEffect = completionEff;
            completed = complete;
            activated = active;
            nextQuests = next;
            previousQuests = prev;
            components = comps;
        }
        // Leaves out next, previous quests, and components to be added later
        public Quest(bool main, string description, string completionEff, bool complete, bool active)
        {
            mainLine = main;
            questDescription = description;
            questCompletionEffect = completionEff;
            completed = complete;
            activated = active;
            nextQuests = new List<Quest>();
            previousQuests = new List<Quest>();
        }
        #endregion
    }

    // Full list of all quests
    private static Dictionary<string, Quest> QuestList = new Dictionary<string, Quest>();

    [SerializeField]
    GameObject questRepPrefab;
    #endregion

    #region functions

    // Stores the states of quests
    // 1 equals true
    // 0 equals false
    // Effectively saves the game
    public static void saveStateAsPrefs(Dictionary<string, Quest> questList)
    {
        PlayerPrefs.SetString("SavedState", "yes");
        foreach (string key in questList.Keys)
        {
            PlayerPrefs.SetInt(key + "completed", questList[key].getCompletionState() ? 1 : 0);
            PlayerPrefs.SetInt(key + "activated", questList[key].getActivationState() ? 1 : 0);
            foreach(string componentKey in questList[key].components.Keys)
            {
                PlayerPrefs.SetInt(key + componentKey + "gatheredAmount", questList[key].components[componentKey].getGathered());
                PlayerPrefs.SetInt(key + componentKey + "completed", questList[key].components[componentKey].getCompleted() ? 1 : 0);
            }
        }
    }
    // Creates new quests using the states recorded in player prefs
    // Effectively loads the game
    public static void updateQuestStates(Dictionary<string, Quest> baseQuests)
    {
        if(PlayerPrefs.GetString("SavedState", "no") == "yes")
        {
            foreach (string key in baseQuests.Keys)
            {
                baseQuests[key].updateState(
                    PlayerPrefs.GetInt(key + "completed", 0) == 1,
                    PlayerPrefs.GetInt(key + "activated", 0) == 1
                    );
                foreach(string componentKey in baseQuests[key].components.Keys)
                {
                    baseQuests[key].components[componentKey].updateState(
                        PlayerPrefs.GetInt(key + componentKey + "gatheredAmount", 0),
                        PlayerPrefs.GetInt(key + componentKey + "completed", 0) == 1
                        );
                }
            }
        }
    }
    private void ChangedActiveScene(Scene current, Scene next)
    {
        saveStateAsPrefs(QuestList);
    }
    // Start is called before the first frame update
    void Start()
    {
        QuestRepresentationManager repManager =  questRepPrefab.GetComponent<QuestRepresentationManager>();
        QuestList = repManager.repToQuests();
        updateQuestStates(QuestList);
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }
    
    #endregion
}