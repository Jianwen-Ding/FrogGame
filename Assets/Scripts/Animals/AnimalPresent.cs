using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPresent : MonoBehaviour
{
    // -----ALL OF THE DETECTION PARAMETERS ARE TO BE DONE LATER------
    // FOR NOW RADIUS IS THE ONLY THING

    // This class controls a manifested animal
    // It is spawned in by the AnimalRadii class
    // This is meant to be a parent class to be inherited
    // By more specialized movement

    #region psuedocode
    // After detecting predator/prey object through sight or sound
    // The animal becomes aware of the object for a set amount of time
    // Each animal 
    #endregion
    #region vars
    // >>> CACHE PARAMETERS <<<
    // Stores objects to be used throughout lifetime of object

    [Header("Cache Parameters")]
    public AnimalDistantDetector detector;
    public AnimalRadii animalController;
    [SerializeField]
    Renderer animalRender;
    [SerializeField]
    GameObject playerObject;


    // >>> BEHAVIOR PARAMETERS <<<
    // Behaviors set to differential overall behavior between species

    [Header("Predator prey Parameters")]
    // Tags of animals that this animal considers prey
    public string[] preyTags;
    // Tags of animals that this animal considers predator
    public string[] predatorTags;

    [Header("Detection Parameters")]
    // Scale of the detector
    [SerializeField]
    float detectorSize;

    [Header("Sight Parameters")]
    // How wide the animal's field of view is 
    [SerializeField]
    float fovAngle;
    // How far the animal will be able to see
    [SerializeField]
    float detectDistance;
    // What the animal can see through
    [SerializeField]
    float sightLayerMask;

    [Header("Hearing Parameters")]
    // How far the animal is able to hear
    [SerializeField]
    float hearDistance;
    // The multiplier that the animal has
    [SerializeField]
    float soundMultiplier;

    [Header("Overarching")]
    // Whether the Animal Radii was moving before
    [SerializeField]
    bool preManifestMoving;
    // Direction that the radii was going pre
    // manifestation on the x,z plane
    [SerializeField]
    float preManifestDirection;
    // Distance until animal attempts reconnection with radii
    [SerializeField]
    float distanceUntilDeload;

    // >>> State Vars <<<
    // Variables that change constantly to record state
    // of animal

    // Panic- Aware of predator
    // Panic- No predators and aware of prey
    // Wander- No predators or prey and radii was not moving before
    // Move- No predators or prey and radii was moving before
    public enum animalState
    {
        Panic,
        Hunt,
        Wander,
        Move
    }
    [Header("State Variables")]
    // Current state of animal
    [SerializeField]
    animalState currentState;
    // Whether the object has been initialized yet
    [SerializeField]
    bool initialized = false;
    // Predators that the animal is aware of
    [SerializeField]
    List<GameObject> predatorsAwareOf;
    // Prey that the animal is aware of
    [SerializeField]
    List<GameObject> preyAwareOf;

    #endregion
    #region functions
    // Initializes the class
    public virtual void init(bool moving ,float previousDirection, AnimalRadii setController)
    {
        initialized = true;
        preManifestMoving = moving;
        animalController = setController;
        preManifestDirection = previousDirection;
        animalRender = gameObject.GetComponent<Renderer>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        detector = transform.GetChild(0).GetComponent<AnimalDistantDetector>();
        detector.init(preyTags, predatorTags, detectorSize);

    }

    // Checks if object can be seen
    public bool inView()
    {
        return animalRender.isVisible;
    }
    // Signals to animal radii that the animal is ready to rejoin the radii
    public virtual void rejoinRadii()
    {
        animalController.removeManifestedAnimal(this);
        Destroy(gameObject);
    }

    // Scans all object if they are within view
    // currently TBD
    public virtual bool withinView(GameObject objectCheck)
    {
        /*ArrayList objectsOfIntrest = new ArrayList();
        for(int i = 0; i < predatorTags.Length; i++)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(predatorTags[i]);
            for(int ob = 0; ob < objectsWithTag.Length; ob++)
            {
                if (objectsOfIntrestDetected.Contains(objectsWithTag[ob]))
                {
                    obj
                }
            }
        }
        for(int i = 0; i < preyTags.Length; i++)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(predatorTags[i]);
            for (int ob = 0; ob < objectsWithTag.Length; ob++)
            {
                if (objectsOfIntrestDetected.Contains(objectsWithTag[ob]))
                {

                }
            }
        }*/
        return true;
    }
    // Scans object if they are making sound
    public virtual bool withinHearingRange(GameObject objectCheck)
    {
        return true;
    }

    // Checks a
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            float distance = customMathf.distanceBetweenPoints(gameObject.transform.position, playerObject.transform.position);
            if (distance > distanceUntilDeload && !inView())
            {
                rejoinRadii();
            }
            if()
        }
    }
    #endregion
}
