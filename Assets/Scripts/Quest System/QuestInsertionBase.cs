using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestInsertionBase : MonoBehaviour
{
    public const string emptyConst = "|empty|";

    // This class allows for drag down representation for quests in Unity UI
    // It circumvents the common problem of needing to go back and forth to find
    // Quest or component names
    // In order to use, create a script that uses this as base

    #region variables
    // Whether the script is currently adds fields or not
    // Turn true to turn on GUI schenanigans
    public bool editorChange;

    // Takes quest layout from this representation
    public GameObject questRepOb;



    // What headers that the UI will have
    public string[] questsNeededRoles;
    // This is what children components take
    public string[] totalQuestList;
    public string[] totalComponentList;
    #endregion
}

// FOR EACH CLASS THAT INHERITS, CREATE A CLASS THAT INHERITS THIS
#if UNITY_EDITOR
[CustomEditor(typeof(QuestInsertionBase), true)]
public class QuestInsertionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var insertionPoint = (QuestInsertionBase)target;

        base.OnInspectorGUI();

        if (insertionPoint.editorChange)
        {
            List<string> questTitles = new List<string>();
            questTitles.Add(QuestInsertionBase.emptyConst);
            foreach (string title in insertionPoint.questRepOb.GetComponent<QuestRepresentationManager>().repToQuests().Keys)
            {
                questTitles.Add(title);
            }
            // Creates a field on each quest needed
            for (int i = 0; i < insertionPoint.questsNeededRoles.Length; i++)
            {
                GUIContent label = new GUIContent("Quest: " + insertionPoint.questsNeededRoles[i]);
                // Index that was previously picked
                // Empty on default
                int fromIndex;
                if (questTitles.Contains(insertionPoint.totalQuestList[i]))
                {
                    fromIndex = questTitles.IndexOf(insertionPoint.totalQuestList[i]);
                }
                else
                {
                    fromIndex = 0;
                }
                insertionPoint.totalQuestList[i] = questTitles[EditorGUILayout.Popup(label, fromIndex, questTitles.ToArray())];
                // Creates a field on every component needed
                if (insertionPoint.totalQuestList[i] != QuestInsertionBase.emptyConst)
                {
                    GUIContent componentLabel = new GUIContent("Component: " + insertionPoint.questsNeededRoles[i]);
                    List<string> componentTitles = new List<string>();
                    foreach (string title in insertionPoint.questRepOb.GetComponent<QuestRepresentationManager>().repToQuests()[insertionPoint.totalQuestList[i]].components.Keys)
                    {
                        componentTitles.Add(title);
                    }
                    int compIndex;
                    if (componentTitles.Contains(insertionPoint.totalComponentList[i]))
                    {
                        compIndex = componentTitles.IndexOf(insertionPoint.totalComponentList[i]);
                    }
                    else
                    {
                        compIndex = 0;
                    }
                    insertionPoint.totalComponentList[i] = componentTitles[EditorGUILayout.Popup(componentLabel, compIndex, componentTitles.ToArray())];
                }
                else
                {
                    insertionPoint.totalComponentList[i] = QuestInsertionBase.emptyConst;
                }
            }
        }
    }
}
#endif