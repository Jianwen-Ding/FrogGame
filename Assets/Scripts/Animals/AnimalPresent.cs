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
    // >>> MISCELLANEOUS <<<
    // Stores variables related to tools and whatnot
    [Header("Miscellaneous")]
    // Whether the animal needs an corresponding animal Radii
    public bool disconnectedFromRadii;
    // Whether animal was marked or not
    public bool marked;

    // >>> SOUND PARAMETER <<<
    [Header("Sound Parameters")]
    // Variables that control how frequently the animal makes sounds
    // minimum time until the animal cry recharges

    [SerializeField]
    float minCryRecharge;
    // max time until the animal cry recharges
    [SerializeField]
    float maxCryRecharge;
    // Amount of time before animal cry recharges
    float cryRechargeLeft = 0;

    // >>> CACHE PARAMETERS <<<
    // Stores objects to be used throughout lifetime of object

    [Header("Cache Parameters")]
    public AnimalDistantDetector detector;
    public AnimalRadii animalController;
    public GameObject playerObject;
    public Rigidbody animalRigid;
    [SerializeField]
    Renderer animalRender;
    [SerializeField]
    AudioSource source;


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

    [Header("Stun Parameters")]
    // The time an animal is stunned
    [SerializeField]
    float stunMultiplier;
    float stunTimeLeft = 0;

    [Header("Overarching")]
    // Whether the Animal Radii was moving before
    public bool preManifestMoving;
    // Direction that the radii was going pre
    // manifestation on the x,z plane
    public float preManifestDirection;
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
        Stun,
        Hunt,
        Wander,
        Move
    }

    [Header("State Variables")]
    // Current state of animal
    public animalState currentState;
    // Whether the object has been initialized yet
    [SerializeField]

    bool initialized = false;
    // Predators that the animal is aware of
    // Stores predators and time left aware of
    public Dictionary<GameObject, float> predatorsAwareOf = new Dictionary<GameObject, float>();
    // Prey that the animal is aware of
    // Stores prey and time left aware of
    public Dictionary<GameObject, float> preyAwareOf = new Dictionary<GameObject, float>();
    // Angle the animal is looking at
    // USE THIS FOR ALL MOVEMENT PLEASE
    public float faceAngle;

    #endregion
    #region functions

    // >>> INIT AND DELOAD FUNCTIONS <<<
    // Initializes the class
    public virtual void init(bool moving ,float previousDirection, AnimalRadii setController)
    {
        initialized = true;
        preManifestMoving = moving;
        animalController = setController;
        preManifestDirection = previousDirection;
        source = gameObject.GetComponent<AudioSource>();
        animalRender = gameObject.GetComponent<Renderer>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        detector = transform.GetChild(0).GetComponent<AnimalDistantDetector>();
        detector.init(preyTags, predatorTags, detectorSize, this);
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
        if (!disconnectedFromRadii)
        {
            animalController.removeManifestedAnimal(this);
            Destroy(gameObject);
        }
    }

    // >>> DETECTION FUNCTIONS <<<
    // Refactors angle of movement based on collisions with raycasts
    public static float refactorDirection(float orgAngle, Vector3 orgPoint, float checkDistance, float angleOffset, int attempts, float dividePerAttempt, LayerMask mask, float objectRad)
    {
        // DebugDisplay.updateDisplay(" Col Detect ", "No Col Found");
        // Debug.DrawLine(orgPoint, orgPoint + customMathf.angleToPoint(orgAngle, checkDistance), Color.black);
        RaycastHit hit;
        if(!Physics.SphereCast(orgPoint, objectRad, customMathf.angleToPoint(orgAngle, checkDistance), out hit, checkDistance, mask))
        {
            return orgAngle;
        }
        for (int i = 1; i < attempts; i++)
        {
            float newDist = checkDistance / (dividePerAttempt * attempts);
            // Display.updateDisplay(" Col Detect ", "Opening in " + i + " attempts");
            float newAngle = orgAngle + angleOffset * i;
            // DrawLine(orgPoint, orgPoint + customMathf.angleToPoint(newAngle, newDist), Color.black);
            RaycastHit hitNew;
            if (!Physics.SphereCast(orgPoint, objectRad, customMathf.angleToPoint(newAngle, newDist), out hitNew, newDist, mask))
            {
                return newAngle;
            }
            float newAngleInverse = orgAngle - angleOffset * i;
            // DrawLine(orgPoint, orgPoint +  customMathf.angleToPoint(newAngleInverse, newDist), Color.black);
            RaycastHit hitNewInverse;
            if (!Physics.SphereCast(orgPoint, objectRad, customMathf.angleToPoint(newAngleInverse, newDist), out hitNew, newDist, mask))
            {
                return newAngleInverse;
            }
        }
        return orgAngle;
    }

    // Scans all object if they are within view
    // Uses angle
    public virtual bool withinView(GameObject objectCheck)
    {
        // Checks if object is within FOV range before sending raycast
        Vector3 diff = objectCheck.transform.position - gameObject.transform.position;
        // DrawRay(gameObject.transform.position, diff, Color.cyan);
        if(diff.magnitude < sightDistance)
        {
            // DrawRay(gameObject.transform.position, customMathf.angleToPoint(faceAngle, 1) * 5, Color.red);
            float angle = Mathf.Abs(customMathf.angleBetweenTwoVecs(diff, customMathf.angleToPoint(faceAngle, 1)));
            // Display.updateDisplay("Sight angle on " + objectCheck.name, angle + " degrees");
            if(angle < fovAngle)
            {
                RaycastHit[] hits = Physics.RaycastAll(gameObject.transform.position, diff, diff.magnitude, sightLayerMask);
                // Display.updateDisplay(objectCheck.name + " raycasted", DebugDisplay.arrayToString(hits, (hit) => hit.transform.gameObject.name));
                // print(DebugDisplay.arrayToString(hits, (hit) => hit.transform.gameObject.name));
                // Checks if only two objects were hit, gameobject and object 
                if(hits.Length == 1 && (hits[0].transform.gameObject == objectCheck))
                {
                    return true;
                }
            }
        }
        //DebugDisplay.deleteDisplay(objectCheck.name + " raycasted");
        return false;
    }

    // Scans object if they are making sound
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
        animalSoundEmmiter emmiterScript = objectCheck.GetComponent<animalSoundEmmiter>();
        if(emmiterScript != null && diff.magnitude < emmiterScript.soundDistance * hearDistance)
        {
            return true;
        }
        return false;
    }

    // Updates current state based on surroundings
    private void updateState()
    {
        // Controls state
        if (predatorsAwareOf.Count != 0)
        {
            if (currentState != animalState.Panic)
            {
                transitionMovementUpdate();
            }
            currentState = animalState.Panic;
        }
        else
        {
            if (preyAwareOf.Count != 0)
            {
                if (currentState != animalState.Hunt)
                {
                    transitionMovementUpdate();
                }
                currentState = animalState.Hunt;
            }
            else
            {
                if (preManifestMoving)
                {
                    if (currentState != animalState.Move)
                    {
                        transitionMovementUpdate();
                    }
                    currentState = animalState.Move;
                }
                else
                {
                    if (currentState != animalState.Wander)
                    {
                        transitionMovementUpdate();
                    }
                    currentState = animalState.Wander;
                }
            }
        }
    }

    // >>> FUNCTIONS TO BE CALLLED ONTO ANIMAL BY EXTERNAL SOURCES <<<

    // Stuns the animal for a set amount of time
    public void stunAnimal(float timeAttempt, bool fixedTime)
    {
        float stunTime;
        if (fixedTime)
        {
            stunTime = timeAttempt;
        }
        else
        {
            stunTime = timeAttempt * stunMultiplier;
        }
        stunTimeLeft = stunTime;
        currentState = animalState.Stun;
    }

    // >>> TEMPLATE MOVEMENT FUNCTIONS <<<
    // Following functions to be used by inherity objects
    // Movement on panic
    public virtual void panicMovementUpdate()
    {
        
    }
    // Movement on hunt
    public virtual void huntMovementUpdate()
    {

    }
    // Movement on stun
    public virtual void stunMovementUpdate()
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
    // Movement on transition
    public virtual void transitionMovementUpdate()
    {

    }
    // Checks a
    // Start is called before the first frame update
    public virtual void Start()
    {
        if (disconnectedFromRadii)
        {
            init(false, 0, null);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (initialized)
        {
            // Counts down time until next cry
            cryRechargeLeft -= Time.deltaTime;
            if (cryRechargeLeft <= 0)
            {
                source.Play();
                cryRechargeLeft = Random.Range(minCryRecharge, maxCryRecharge);
            }

            // Checks all detected objects if they are seen or heard
            // Refreshes time if detected again
            for (int i = 0; i < detector.predatorWithinField.Count; i++)
            {
                if (detector.predatorWithinField[i] != null)
                {
                    if (withinView(detector.predatorWithinField[i]) || withinHearingRange(detector.predatorWithinField[i]))
                    {
                        print("" + detector.predatorWithinField[i].name);
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
            }
            for (int i = 0; i < detector.preyWithinField.Count; i++)
            {
                if(detector.preyWithinField[i] != null)
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
            }
            // Checks if objecs of intrest have not been scanned for a set amount of time
            GameObject[] predsToRemove = new GameObject[predatorsAwareOf.Keys.Count];
            int predIter = 0;
            foreach(GameObject pred in predatorsAwareOf.Keys)
            {
                predsToRemove[predIter] = pred;
                predIter += 1;
            }
            int preyIter = 0;
            GameObject[] preyToRemove = new GameObject[preyAwareOf.Keys.Count];
            foreach (GameObject prey in preyAwareOf.Keys)
            {
                preyToRemove[preyIter] = prey;
                preyIter += 1;
            }
            if (!customMathf.contains<GameObject>(predsToRemove, null))
            {
                // Display.updateDisplay(gameObject.name + " predators", DebugDisplay.arrayToString(predsToRemove, (ob) => ob.name + " has " + Mathf.Round(predatorsAwareOf[ob]) + " seconds left"));
            }
            for (int i = predsToRemove.Length - 1; i >= 0; i--)
            {
                predatorsAwareOf[predsToRemove[i]] -= Time.deltaTime;
                if (predatorsAwareOf[predsToRemove[i]] < 0)
                {
                    predatorsAwareOf.Remove(predsToRemove[i]);
                }
            }
            if (!customMathf.contains<GameObject>(preyToRemove, null))
            {
                // Display.updateDisplay(gameObject.name + " prey", DebugDisplay.arrayToString(preyToRemove, (ob) => ob.name + " has " + Mathf.Round(preyAwareOf[ob]) + " seconds left"));
            }
            for (int i = preyToRemove.Length - 1; i >= 0; i--)
            {
                preyAwareOf[preyToRemove[i]] -= Time.deltaTime;
                if (preyAwareOf[preyToRemove[i]] < 0)
                {
                    preyAwareOf.Remove(preyToRemove[i]);
                }
            }
            // Rejoins radii if animal gets too far and leaves view
            float distance = customMathf.distanceBetweenPoints(gameObject.transform.position, playerObject.transform.position);
            if (distance > distanceUntilDeload && !inView())
            {
                rejoinRadii();
            }
            // Locks animal into stun for set amount of time
            if(currentState == animalState.Stun)
            {
                stunMovementUpdate();
                stunTimeLeft -= Time.deltaTime;
                if(stunTimeLeft <= 0)
                {
                    updateState();
                }
            }
            else
            {
                // Updates state
                updateState();
                // Controls movement based on state
                // Display.updateDisplay(" state of " + gameObject.name, currentState + "");
                if (currentState == animalState.Panic)
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
    }
    #endregion
}
