using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static QuestSys;
using TMPro;
using UnityEngine.UI;

public class QuestRepresentation : MonoBehaviour
{
    #region variables
    // Meant to represent a single Quest in UI
    // Will be used by designers to actually edit quests and
    // To be shown to players

    // >>> Art Assets <<<
    // Art assets to be set in place by quest representation
    [Header("Art Assets")]

    [SerializeField]
    Sprite backDropSprite;
    [SerializeField]
    Sprite componentBackDropSprite;
    [SerializeField]
    Sprite relatedSprite;
    [SerializeField]
    Sprite mainLineIndicatorSprite;
    [SerializeField]
    Sprite questNonCheckedBoxSprite;
    [SerializeField]
    Sprite questCheckedBoxSprite;
    [SerializeField]
    Sprite componentNonCheckedBoxSprite;
    [SerializeField]
    Sprite componentCheckedBoxSprite;

    // >>> UI Objects <<<
    // Objects to be changed on editing
    [Header("UI Objects")]

    // > Non Prefabs <
    // Assets for
    // Backdrop of quest
    [SerializeField]
    GameObject questBackDrop;
    // Where title of quest will go
    [SerializeField]
    GameObject titleTextbox;
    // Where description of quest will go
    [SerializeField]
    GameObject descriptionTextbox;
    // Where effects of quest will go
    [SerializeField]
    GameObject effectTextbox;
    // Quest image
    [SerializeField]
    GameObject relatedImage;
    // Mainline quest indicator
    [SerializeField]
    GameObject mainlineIndicator;
    // Quest incomplete checkBox
    [SerializeField]
    GameObject questCheckbox;
    // List of generated objects
    List<GameObject> generatedObject = new List<GameObject>();

    // > Prefabs <
    // Backdrop of a component
    [SerializeField]
    GameObject componentBackDropPrefab;
    // Where title of quest will go
    [SerializeField]
    GameObject componentTitlePrefab;
    // Where title of quest will go
    [SerializeField]
    GameObject componentCheckboxPrefab;
    // Where count of quest will go
    [SerializeField]
    GameObject componentCountPrefab;
    // Arrow prefab
    [SerializeField]
    GameObject arrowPrefab;


    // >>> STORAGE VARIABLES <<<
    // Variables involved in the storage of the quest
    [Header("Quest Information Storage")]

    // Title of the quest
    public string questTitle;
    // Whether the quest is part of the main story
    public bool mainLine;
    // Description of the quest
    public string questDescription;
    // What will change on completion of quest
    public string questCompletionEffect;
    // Whether components have been fufilled
    public bool completed;
    // Whether the quest has been activated or not
    public bool activated;

    // Quests that will potentially activate on completion of quest
    public List<QuestRepresentation> nextQuests;
    // Quests that need to be activated in order to attempt activation of quest
    public List<QuestRepresentation> previousQuests;

    // List of component representations
    public string[] components;
    public bool[] increments;
    public int[] gatheredAmount;
    public int[] componentTotal;
    public bool[] componentCompletion;


    // >>> VISUALIZATION PARAMETERS <<<
    // Variables that send the art assets into place
    [Header("Visualization Parameters")]

    // Whether it is being edited or not
    [SerializeField]
    bool edited;
    // Start point
    [SerializeField]
    Vector2 startPoint;
    // Title and indicators are on same y
    [SerializeField]
    float titleDisplacement;
    [SerializeField]
    float titleXDisplacement;
    [SerializeField]
    float indicatorXDisplacement;
    [SerializeField]
    float checkMarkXDisplacement;
    // Rest on same X but displaces Y
    [SerializeField]
    float imageDisplacement;
    [SerializeField]
    float descriptionDisplacement;
    [SerializeField]
    float effectDisplacement;
    // Components are bunched up into a group
    [SerializeField]
    float componentDisplacement;
    [SerializeField]
    float componentTitleXDisplace;
    [SerializeField]
    float componentCountXDisplace;
    [SerializeField]
    float componentCheckmarkXDisplace;

    #endregion
    #region functions
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generates visualization of quest
    public void regenerateVisual()
    {
        if (edited)
        {
            // Destroys previous creations
            for(int i = generatedObject.Count - 1; i >= 0; i--)
            {
                GameObject ob = generatedObject[i];
                generatedObject.Remove(ob);
                Destroy(ob);
            }
            // Creates new objects and adds them as children
            Vector2 currentDisplacement = startPoint;
            void setOb(float displacement, float xDisplace, GameObject givenObject)
            {
                currentDisplacement = currentDisplacement + Vector2.down * displacement;
                givenObject.transform.position = (Vector2)givenObject.transform.position + currentDisplacement;
                currentDisplacement = currentDisplacement + Vector2.down * displacement;
            }
            GameObject createOb(float displacement, float xDisplace, GameObject givenPrefab)
            {
                GameObject madeObject = Instantiate(givenPrefab, gameObject.transform.position, Quaternion.identity.normalized);
                setOb(displacement, xDisplace, madeObject);
                madeObject.transform.parent = transform;
                generatedObject.Add(madeObject);
                return madeObject;
            }
            void setImage(float displacement, float xDisplace, GameObject givenObject, Sprite givenSprite)
            {
                setOb(displacement, xDisplace, givenObject);
                Image gotImage = givenObject.GetComponent<Image>();
                if(givenSprite != null )
                {
                    gotImage.color = Color.white;
                    gotImage.sprite = givenSprite;
                }
                else
                {
                    gotImage.color = Color.clear;
                }
            }
            void setText(float displacement, float xDisplace, GameObject givenObject, string text)
            {
                setOb(displacement, xDisplace, givenObject);
                TextMeshProUGUI textMesh =  givenObject.GetComponent<TextMeshProUGUI>();
                textMesh.text = text;
            }
            questBackDrop.GetComponent<Image>().sprite = backDropSprite;
            questBackDrop.transform.position = gameObject.transform.position;
            setText(titleDisplacement, titleXDisplacement, titleTextbox, questTitle);
            if (mainLine)
            {
                setImage(0, indicatorXDisplacement, mainlineIndicator, mainLineIndicatorSprite);
            }
            else
            {
                setImage(0, indicatorXDisplacement, mainlineIndicator, null);
            }
            if (completed)
            {
                setImage(0, checkMarkXDisplacement, questCheckbox, questCheckedBoxSprite);
            }
            else
            {
                setImage(0, checkMarkXDisplacement, questCheckbox, questNonCheckedBoxSprite);
            }
            setImage(imageDisplacement, 0, relatedImage, relatedSprite);
            setText(descriptionDisplacement, 0, descriptionTextbox, questDescription);
            for (int i = 0; i < components.Length; i++)
            {
                createOb(componentDisplacement, 0, componentCheckboxPrefab);
                GameObject compTitleTextBox = createOb(0, componentTitleXDisplace, componentTitlePrefab);
                GameObject compCountTextBox = createOb(0, componentCountXDisplace, componentCountPrefab);
                GameObject compCheckbox = createOb(0, componentCheckmarkXDisplace, componentCheckboxPrefab);
                setText(0, componentTitleXDisplace, compTitleTextBox, components[i] + ": ");
                if (increments[i])
                {
                    setText(0, componentCountXDisplace, compCountTextBox, gatheredAmount[i] + " / " + componentTotal[i]);
                    setImage(0, componentCheckmarkXDisplace, compCheckbox, null);
                }
                else
                {
                    setText(0, componentCountXDisplace, compTitleTextBox, "");
                    if (componentCompletion[i])
                    {
                        setImage(0, componentCheckmarkXDisplace, compCheckbox, componentCheckedBoxSprite);
                    }
                    else
                    {
                        setImage(0, componentCheckmarkXDisplace, compCheckbox, componentNonCheckedBoxSprite);
                    }
                }
            }
            if (completed)
            {
                setText(effectDisplacement, 0, effectTextbox, questCompletionEffect);
            }
            else
            {
                setText(0, 0, effectTextbox, "");
            }
        }

    }

    // Use parameters of quest representation to make
    // A seprate script will add in connections with next and prev quests
    // along with functions that add in effects later
    public Quest generateQuest()
    {
        Quest retQuest = new Quest(mainLine, questDescription, questCompletionEffect, completed, activated);
        Dictionary<string, Quest.QuestComponent> compList = new Dictionary<string, Quest.QuestComponent>();
        for(int i = 0; i < components.Length; i++)
        {
            compList.Add(components[i], new Quest.QuestComponent(retQuest, increments[i], componentTotal[i], gatheredAmount[i], componentCompletion[i]));
        }
        retQuest.insertComponents(compList);
        return retQuest;
    }
    #endregion
}

[CustomEditor(typeof(QuestRepresentation))]
public class QuestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var Quest = (QuestRepresentation)target;

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            Quest.regenerateVisual();
        }
    }
}
