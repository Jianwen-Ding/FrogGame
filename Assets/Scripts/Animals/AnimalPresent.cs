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

    // >>> Detection Procedure <<<
    // After detecting predator/prey object through sight or sound
    // Animal needs to be in detector collider object to be detected by sight or sound

    // Sight needs other object to have sightline betwen animal and object to occur
    // Hear distance = object multiplier * base hear distance

    // The animal becomes aware of the object for a set amount of time or is disconnected early if it exits the detector for whatever reason

    // >>> Behavior State Procedure <<<
    // Panic- Animal runs away on detection on any predator
    // Hunt- If no predator detected and prey has been detected
    // Wander- No initial command to move from radii
    // Move- Has initial command to move from radii, moves in a specific direction
    // while no other objects of intrest come into view
    #endregion
    #region vars
    // >>> CACHE PARAMETERS <<<
    // Stores objects to be used throughout lifetime of object

    [Header("Cache Parameters")]
    public AnimalDistantDetector detector;
    public AnimalRadii animalController;
    public GameObject playerObject;
    public Rigidbody animalRigid;
    [SerializeField]
    Renderer animalRender;


    // >>> BEHAVIOR PARAMETERS <<<
    // Behaviors set to differential overall behavior between species

    [Header("Predator prey Parameters")]
    // Tags of animals that this animal considers of prey
    // Tags of prey
    public string[] preyTags;
    // Tags of animals that this animal considers predator
    // Tags of predators
    public string[] predatorTags;

    [Header("Detection Parameters")]
    // Scale of the detector
    [SerializeField]
    float detectorSize;
    // How long an animal stays aware of a object
    [SerializeField]
    float detectionTimePrey;
    [SerializeField]
    float detectionTimePred;

    [Header("Sight Parameters")]
    // How wide the animal's field of view is 
    [SerializeField]
    float fovAngle;
    // How far the animal will be able to see
    [SerializeField]
    float sightDistance;
    // What the animal can see through
    [SerializeField]
    LayerMask sightLayerMask;

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
    // Stores predators and time left aware of
    public Dictionary<GameObject, float> predatorsAwareOf;
    // Prey that the animal is aware of
    // Stores predator and time left aware of
    public Dictionary<GameObject, float> preyAwareOf;
    // Angle the animal is looking at
    // USE THIS FOR ALL MOVEMENT PLEASE
    public float faceAngle;

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
        animalRigid = gameObject.GetComponent<Rigidbody>();

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
    // Uses angle
    public virtual bool withinView(GameObject objectCheck)
    {
        // Checks if object is within FOV range before sending raycast
        Vector3 diff = objectCheck.transform.position - gameObject.transform.position;
        if(diff.magnitude < sightDistance)
        {
            float angle = Mathf.Abs(customMathf.angleBetweenTwoVecs(diff, customMathf.angleToPoint(faceAngle, 1)));
            if(angle < fovAngle)
            {
                RaycastHit[] hits = Physics.RaycastAll(gameObject.transform.position, diff, diff.magnitude, sightLayerMask);
                // Checks if only two objects were hit, gameobject and object 
                if(hits.Length == 2 && (hits[0].transform.gameObject == objectCheck || hits[1].transform.gameObject == objectCheck))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Scans object if they are making sound
    // TBD
    public virtual bool withinHearingRange(GameObject objectCheck)
    {
        Vector3 diff = objectCheck.transform.position - transform.position;
        if(objectCheck.tag == "Player")
        {
            OdeionPlayer player = objectCheck.GetComponent<OdeionPlayer>();
            if (player != null && diff.magnitude < player.getCurrentSoundMultiplier() * hearDistance)
            {
                return true;
            }
        }
        AnimalPresent animalScript = objectCheck.GetComponent<AnimalPresent>();
        if(animalScript != null && diff.magnitude < animalScript.soundMultiplier * hearDistance)
        {
            return true;
        }
        return false;
    }

    // When a object moves the rigid collider
    // To be used by detection radius
    public void removeObject(GameObject start)
    {

    }
    // Following functions to be used by inherity objects
    // Movement on panic
    public virtual void panicMovementUpdate()
    {
        
    }
    // Movement on hunt
    public virtual void huntMovementUpdate()
    {

    }
    // Movement on wander
    public virtual void wanderMovementUpdate()
    {

    }
    // Movement on move
    public virtual void moveMovementUpdate()
    {

    }
    // Checks a
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (initialized)
        {
            // Checks all detected objects if they are seen or heard
            // Refreshes time if detected again
            for (int i = 0; i < detector.predatorWithinField.Count; i++)
            {
                if (withinView(detector.predatorWithinField[i]) || withinHearingRange(detector.predatorWithinField[i]))
                {
                    if (predatorsAwareOf.ContainsKey(detector.predatorWithinField[i]))
                    {
                        predatorsAwareOf[detector.predatorWithinField[i]] = detectionTimePred;
                    }
                    else
                    {
                        predatorsAwareOf.Add(detector.predatorWithinField[i], detectionTimePred);
                    }
                }
            }
            for (int i = 0; i < detector.preyWithinField.Count; i++)
            {
                if (withinView(detector.preyWithinField[i]) || withinHearingRange(detector.preyWithinField[i]))
                {
                    if (preyAwareOf.ContainsKey(detector.preyWithinField[i]))
                    {
                        preyAwareOf[detector.preyWithinField[i]] = detectionTimePrey;
                    }
                    else
                    {
                        preyAwareOf.Add(detector.preyWithinField[i], detectionTimePrey);
                    }
                }
            }
            // Checks if objecs of intrest have not been scanned for a set amount of time
            ArrayList predsToRemove = new ArrayList();
            foreach (GameObject pred in predatorsAwareOf.Keys)
            {
                predatorsAwareOf[pred] -= Time.deltaTime;
                if (predatorsAwareOf[pred] < 0)
                {
                    predsToRemove.Add(pred);
                }
            }
            ArrayList preysToRemove = new ArrayList();
            foreach (GameObject prey in preyAwareOf.Keys)
            {
                predatorsAwareOf[prey] -= Time.deltaTime;
                if (predatorsAwareOf[prey] < 0)
                {
                    preysToRemove.Add(prey);
                }
            }
            foreach (GameObject ob in predsToRemove)
            {
                predatorsAwareOf.Remove(ob);
            }
            foreach (GameObject ob in preysToRemove)
            {
                preyAwareOf.Remove(ob);
            }
            // Rejoins radii if animal gets too far and leaves view
            float distance = customMathf.distanceBetweenPoints(gameObject.transform.position, playerObject.transform.position);
            if (distance > distanceUntilDeload && !inView())
            {
                rejoinRadii();
            }
            // Controls movement based on state
            if(currentState == animalState.Panic)
            {
                panicMovementUpdate();
            }
            else if (currentState == animalState.Hunt)
            {
                huntMovementUpdate();
            }
            else if (currentState == animalState.Wander)
            {
                wanderMovementUpdate();
            }
            else if (currentState == animalState.Move)
            {
                moveMovementUpdate();
            }
            
        }
    }
    #endregion
}
