using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestRepresentationManager : MonoBehaviour
{
    // ALL QUEST PAGES NEED TO BE ATTACHED TO SOMETHING FOR THIS TO WORK
    // This class is meant to extract quest information from quest representations and
    // to update quest representations with new save data

    #region variables
    [SerializeField]
    QuestRepresentation startingRepresentation;
    #endregion
    #region functions
    public Dictionary<string, QuestSys.Quest> repToQuests()
    {
        // Dictionary of quests to be returned
        Dictionary<string, QuestSys.Quest> retDict = new Dictionary<string, QuestSys.Quest>();
        // Dictionary of quest representation made to help link quests together
        List<QuestRepresentation> repList = new List<QuestRepresentation>();
        // Recursively iterates through previous and next quest representations to generate quests
        void generationIterate(QuestRepresentation newRep)
        {
            repList.Add(newRep);
            retDict.Add(newRep.questTitle, newRep.generateQuest());
            for(int i = 0; i < newRep.previousQuests.Count; i++)
            {
                if (newRep.previousQuests[i] != null && !retDict.ContainsKey(newRep.previousQuests[i].questTitle))
                {
                    generationIterate(newRep.previousQuests[i]);
                }
            }
            for (int i = 0; i < newRep.nextQuests.Count; i++)
            {
                if (newRep.nextQuests[i]  != null && !retDict.ContainsKey(newRep.nextQuests[i].questTitle))
                {
                    generationIterate(newRep.nextQuests[i]);
                }
            }
        }
        generationIterate(startingRepresentation);
        // Connects quests together
        for(int i = 0; i < repList.Count; i++)
        {
            for(int z = 0; z < repList[i].nextQuests.Count; z++)
            {
                retDict[repList[i].questTitle].nextQuests.Add(retDict[repList[i].nextQuests[z].questTitle]);
            }
            for (int z = 0; z < repList[i].previousQuests.Count; z++)
            {
                retDict[repList[i].questTitle].previousQuests.Add(retDict[repList[i].previousQuests[z].questTitle]);
            }
        }
        return retDict;
    }

    // Updates all possible representations with dictionary of quests
    public void updateRepresentation(Dictionary<string, QuestSys.Quest> quests)
    {
        HashSet<QuestRepresentation> repSet = new HashSet<QuestRepresentation>();
        void updateIterate(QuestRepresentation representation)
        {
            repSet.Add(representation);
            representation.insertQuest(quests[representation.questTitle]);
            for (int i = 0; i < representation.previousQuests.Count; i++)
            {
                if (!repSet.Contains(representation.previousQuests[i]))
                {
                    updateIterate(representation.previousQuests[i]);
                }
            }
            for (int i = 0; i < representation.nextQuests.Count; i++)
            {
                if (!repSet.Contains(representation.nextQuests[i]))
                {
                    updateIterate(representation.nextQuests[i]);
                }
            }
        }
        updateIterate(startingRepresentation);
    }

    // Visually regenerates all possible representations with dictionary of quests
    public static void regenerateRepresentation(QuestRepresentation startingQuest)
    {
        HashSet<QuestRepresentation> repSet = new HashSet<QuestRepresentation>();
        void updateIterate(QuestRepresentation representation)
        {
            repSet.Add(representation);
            representation.regenerateVisual();
            for (int i = 0; i < representation.previousQuests.Count; i++)
            {
                if (!repSet.Contains(representation.previousQuests[i]))
                {
                    updateIterate(representation.previousQuests[i]);
                }
            }
            for (int i = 0; i < representation.nextQuests.Count; i++)
            {
                if (!repSet.Contains(representation.nextQuests[i]))
                {
                    updateIterate(representation.nextQuests[i]);
                }
            }
        }
        updateIterate(startingQuest);
    }

    // Updates quests with player prefs
    public void Start()
    {
        Dictionary<string, QuestSys.Quest> orginalQuests = repToQuests();
        QuestSys.updateQuestStates(orginalQuests);
        updateRepresentation(orginalQuests);
    }
    #endregion

}
