using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class questTracker : MonoBehaviour
{
    // This class controls how quests are shown through text.
    // Any activated and uncompleted components/quests are displayed

    #region variables
    // >>> Cache Variables <<<
    [Header("Cache Variables")]

    TextMeshProUGUI textField;

    // >>> Text Fillers <<<
    [Header("Text Fillers")]

    [SerializeField]
    string mainLineIndicator;

    [SerializeField]
    int componentSpacing;
    #endregion
    #region functions

    // Creates a string with n spaces
    private string getSpacing(int n)
    {
        string retString = "";
        for(int i = 0; i < n; i++) {
            retString = retString + " ";
        }
        return retString;
    }
    // Updates text when quest system has been updated
    public void updateText()
    {
        string finalText = "";
        foreach(string key in QuestSys.QuestList.Keys)
        {
            QuestSys.Quest currentQuest = QuestSys.QuestList[key];
            if(!currentQuest.getCompletionState() && currentQuest.getActivationState())
            {
                finalText += "<b>" + key;
                if (currentQuest.mainLine)
                {
                    finalText += mainLineIndicator;
                }
                finalText += ": </b><br>";
                foreach (string componentKey in currentQuest.components.Keys)
                {
                    QuestSys.Quest.QuestComponent currentComponent = currentQuest.components[componentKey];
                    if (!currentComponent.getCompleted())
                    {
                        finalText += getSpacing(componentSpacing) + componentKey;
                        if (currentComponent.incremental)
                        {
                            finalText += ": " + currentComponent.getGathered() + " / " + currentComponent.totalAmount;
                        }
                        finalText += "<br>";
                    }
                }
                finalText += "<br>";
            }
        }
        textField.text = finalText;
    }
    // Start is called before the first frame update
    void Start()
    {
        textField = gameObject.GetComponent<TextMeshProUGUI>();
        updateText();
        QuestSys.insertFunc(updateText);
    }

    // Update is called once per frame
    void Update()
    {
        updateText();
    }

    #endregion
}
