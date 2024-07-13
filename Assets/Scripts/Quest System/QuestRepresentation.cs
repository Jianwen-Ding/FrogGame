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
    GameObject arrowBasePrefab;
    // Arrow prefab
    [SerializeField]
    GameObject arrowTipPrefab;
    // Arrow prefab
    [SerializeField]
    GameObject arrowTailPrefab;


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
    // Overall scale of the quest page
    [SerializeField]
    float pageScale;
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
    // Variables in charge of arrows
    public float distanceFromOriginArrow;
    [SerializeField]
    float distanceToLengthConstant;
    #endregion
    #region functions
    // Start is called before the first frame update
    void Start()
    {
        if (!activated)
        {
            gameObject.SetActive(false);
        }
        else
        {
            regenerateVisual();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generates visualization of quest
    public void regenerateVisual()
    {
        // >>> CLEARING PREVIOUS CHANGES <<<
            for (int i = transform.GetChild(0).childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).GetChild(i).gameObject);
        }
        // >>> PAGE CREATION FUNCTION SETUP <<<
        Vector2 currentDisplacement = startPoint * pageScale;
        void setOb(float displacement, float xDisplace, GameObject givenObject)
        {
            currentDisplacement = currentDisplacement + Vector2.down * displacement;
            givenObject.transform.position = (Vector2)gameObject.transform.position + currentDisplacement + Vector2.right * xDisplace;
        }
        GameObject createOb(GameObject givenPrefab)
        {
            GameObject madeObject = Instantiate(givenPrefab, gameObject.transform.position, Quaternion.identity.normalized);
            madeObject.transform.localScale = madeObject.transform.localScale * pageScale;
            madeObject.transform.SetParent(transform.GetChild(0));
            return madeObject;
        }
        void setImage(float displacement, float xDisplace, GameObject givenObject, Sprite givenSprite)
        {
            setOb(displacement, xDisplace, givenObject);
            Image gotImage = givenObject.GetComponent<Image>();
            if (givenSprite != null)
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
            TextMeshProUGUI textMesh = givenObject.GetComponent<TextMeshProUGUI>();
            textMesh.text = text;
        }
        // >>> PAGE CREATION <<<
        questBackDrop.GetComponent<Image>().sprite = backDropSprite;
        questBackDrop.transform.position = gameObject.transform.position;
        setText(titleDisplacement * pageScale, titleXDisplacement * pageScale, titleTextbox, questTitle);
        if (mainLine)
        {
            setImage(0, indicatorXDisplacement * pageScale, mainlineIndicator, mainLineIndicatorSprite);
        }
        else
        {
            setImage(0, indicatorXDisplacement * pageScale, mainlineIndicator, null);
        }
        if (completed)
        {
            setImage(0, checkMarkXDisplacement * pageScale, questCheckbox, questCheckedBoxSprite);
        }
        else
        {
            setImage(0, checkMarkXDisplacement * pageScale, questCheckbox, questNonCheckedBoxSprite);
        }
        setImage(imageDisplacement * pageScale, 0, relatedImage, relatedSprite);
        setText(descriptionDisplacement * pageScale, 0, descriptionTextbox, questDescription);
        for (int i = 0; i < components.Length; i++)
        {
            GameObject componentBackDrop = createOb(componentBackDropPrefab);
            setImage(componentDisplacement * pageScale, 0, componentBackDrop, componentBackDropSprite);
            GameObject compTitleTextBox = createOb(componentTitlePrefab);
            GameObject compCountTextBox = createOb(componentCountPrefab);
            GameObject compCheckbox = createOb(componentCheckboxPrefab);
            setText(0, componentTitleXDisplace * pageScale, compTitleTextBox, components[i] + ": ");
            if (increments[i])
            {
                setText(0, componentCountXDisplace * pageScale, compCountTextBox, gatheredAmount[i] + " / " + componentTotal[i]);
                setImage(0, componentCheckmarkXDisplace * pageScale, compCheckbox, null);
            }
            else
            {
                setText(0, componentCountXDisplace * pageScale, compCountTextBox, "");
                if (componentCompletion[i])
                {
                    setImage(0, componentCheckmarkXDisplace * pageScale, compCheckbox, componentCheckedBoxSprite);
                }
                else
                {
                    setImage(0, componentCheckmarkXDisplace * pageScale, compCheckbox, componentNonCheckedBoxSprite);
                }
            }
        }
        if (completed)
        {
            setText(effectDisplacement * pageScale, 0, effectTextbox, questCompletionEffect);
        }
        else
        {
            setText(0, 0, effectTextbox, "");
        }
        // >>> ARROW GENERATION <<<
        // Each quest page is responsible for generating arrows of
        // the previous quest pages
        for (int i = 0; i < previousQuests.Count; i++)
        {
            GameObject prevOb = previousQuests[i].gameObject;
            Vector3 diff = prevOb.transform.position - gameObject.transform.position;
            float diffAng = customMathf.pointToAngle(diff.x, diff.y);
            Vector3 towardsPoint = gameObject.transform.position + diff.normalized * distanceFromOriginArrow;
            Vector3 fromPoint = prevOb.transform.position + -diff.normalized * previousQuests[i].distanceFromOriginArrow;
            Vector3 middlePoint = (towardsPoint + fromPoint) / 2;
            float pointDist = (towardsPoint - fromPoint).magnitude;
            GameObject tail = createOb(arrowTailPrefab);
            tail.transform.position = fromPoint;
            tail.transform.rotation = Quaternion.Euler(new Vector3(0, 0, diffAng));
            GameObject head = createOb(arrowTipPrefab);
            head.transform.position = towardsPoint;
            head.transform.rotation = Quaternion.Euler(new Vector3(0, 0, diffAng));
            GameObject arrowBase = createOb(arrowBasePrefab);
            arrowBase.transform.position = middlePoint;
            arrowBase.transform.localScale = new Vector3(pointDist * distanceToLengthConstant, head.transform.localScale.y, head.transform.localScale.z);
            arrowBase.transform.rotation = Quaternion.Euler(new Vector3(0, 0, diffAng));
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

    // Insert quest states into the representation
    // Connections with other quest representations are seperatedly made
    public void insertQuest(Quest insertQuest)
    {
        mainLine = insertQuest.mainLine;
        questDescription = insertQuest.questDescription;
        questCompletionEffect = insertQuest.questCompletionEffect;
        completed = insertQuest.getCompletionState();
        activated = insertQuest.getActivationState();
        int i = 0;
        components = new string[insertQuest.components.Count];
        increments = new bool[insertQuest.components.Count];
        gatheredAmount = new int[insertQuest.components.Count];
        componentTotal = new int[insertQuest.components.Count];
        componentCompletion = new bool[insertQuest.components.Count];
        foreach(string key in insertQuest.components.Keys)
        {
            components[i] = key;
            increments[i] = insertQuest.components[key].incremental;
            gatheredAmount[i] = insertQuest.components[key].getGathered();
            componentTotal[i] = insertQuest.components[key].totalAmount;
            componentCompletion[i] = insertQuest.components[key].getCompleted();
            i = i + 1;
        }
        if (activated)
        {
            gameObject.SetActive(true);
            regenerateVisual();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}

#if UNITY_EDITOR
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
            QuestRepresentationManager.regenerateRepresentation(Quest);
        }
    }
}
#endif
