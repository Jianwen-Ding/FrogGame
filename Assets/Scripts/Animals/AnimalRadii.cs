using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRadii : MonoBehaviour
{
    // This script controls the behavior of Animals when deloaded away from the players view.
    // This allows animal movements to be simplified greatly when dealing with far out of camera animals

    #region psuedocode
    // >>>General animal behavior<<<
    // 1 - Activates past wake up time
    // 2 - Initiates algorithm to find close object of intrest
    // 3 - Moves towards object of intrest at set speed
    // 4 - Stay at object of intrest for set time
    // 5 - Checks if the time is past sleep time
    // 6 - Goes to den if it exists, otherwise pathfinds to preferred object to sleep on.
    // 7 - Sleeps at object

    // On radius enter
    // If at object of intrest or asleep, spawn animal directly at object animal is attached to
    // Otherwise, spawn animal in nearest non visible area from the center of radii (maybe in bush or ect)
    // Deactivate radii until animal is out of sight and out of a certain radius.

    // Periodically, the animal has a chance to drop a marker


    // >>>Object of Intrest Algorithm<<<
    // Searches for all objects (excluding current object of intrest) with tags of intrest
    // Finds magnitudes of all tags of intrest
    // Sorts magnitudes and randomly picks a object based on distance order

    // >>>Surface finding algortihm<<<
    // Sends raycast down from x-z coordinates from a high place.
    // Ignore certain objects.

    // >>>Spawning Animal Algorithm<<<
    // Locked in:
    // Sends raycast down at same x-z coords. Spawn animal directly on resulting location
    // Moving:
    // Searches for nearby appropiate hiding area in radius and spawns animal directly in object
    // If none can be found, send raycast down at same x-z coords. Spawns animal directly on resulting location.
    #endregion
    #region vars
    // >>> CACHE PARAMETERS <<<
    // Stores objects to be used throughout lifetime of object

    [Header("Cache Parameters")]
    [SerializeField]
    GameObject animalPrefab;
    [SerializeField]
    GameObject markerPrefab;
    [SerializeField]
    GameObject playerObject;
    // >>> BEHAVIOR PARAMETERS <<<
    // Behaviors set to differential overall behavior between species

    [Header("Overall Movement Behavior Parameters")]
    // Stays in constant position over the course of days
    [SerializeField]
    bool Permanent;
    // When does the animal wake
    [SerializeField]
    int wakeHour;
    [SerializeField]
    int wakeMinute;
    // When does the animal sleep
    [SerializeField]
    int sleepHour;
    [SerializeField]
    int sleepMinute;
    // What tags does the animal graviate towards
    [SerializeField]
    string[] objectsOfIntrest;
    // What tags does the animal graviate towards
    [SerializeField]
    string[] sleepObjects;
    // The amount of time the animal spends at a single point of intrest
    [SerializeField]
    float timeAtIntrest;
    // Speed that the radii moves
    [SerializeField]
    float radiiSpeed;
    // Distance until radii snaps onto the object of intrest
    [SerializeField]
    float radiiSnap;
    // Base of chance of object being pathfinded towards
    [SerializeField]
    float objectIntrestBase;
    // Base of chance of object being pathfinded towards when trying to sleep
    [SerializeField]
    float objectSleepBase;

    [Header("Animal Spawn Parameters")]

    // Size of radius that triggers spawn upon entering
    [SerializeField]
    float radiusPlayerTriggerSize;
    // Size of radius in which the animal spawns
    [SerializeField]
    float radiusAnimalSpawnSize;
    // What tags should the appropiate surface have
    [SerializeField]
    string[] appropiateSpawnObjects;
    // Layermask of raycast, what layers the raycast ignores
    [SerializeField]
    LayerMask spawnRaycastLayerMask;
    // How far out the raycast goes up
    [SerializeField]
    float spawnRaycastYAdjust;
    // How far the raycast hits
    [SerializeField]
    float spawnRaycastLength;
    // Amount of animals in radii
    [SerializeField]
    bool animalsPerRadii;

    [Header("Marker Spawn Parameters")]
    // Time between each chance that the animal will leave a marking
    [SerializeField]
    float markerWindowTime;
    // Chance out of 100 that the animal will leave a marking on a window
    [SerializeField]
    float markerWindowChance;
    // Layermask of raycast, what layers the raycast ignores
    [SerializeField]
    LayerMask markerRaycastLayerMask;
    // How far out the raycast goes up
    [SerializeField]
    float markerRaycastYAdjust;
    // How far the raycast hits
    [SerializeField]
    float markerRaycastLength;
    // Base of chance of object spawning an animal
    [SerializeField]
    float objectSpawnBase;

    public enum state
    {
        MorSleep,
        Locked,
        Moving,
        NighSleep
    }
    [Header("State Vars")]
    // Variables that change constantly to record state
    // of animal

    // Animals actually spawned
    // Implement swarm animals later
    ArrayList manifestedAnimals = new ArrayList();
    // Current state of the animal radii
    [SerializeField]
    state currentState = state.MorSleep;
    // Whether the animal has spawned in
    bool manifested = false;
    // Whether the animal is locked in a position
    [SerializeField]
    bool lockedPosition = true;
    // The object the animal is locked into
    [SerializeField]
    GameObject lockedObject = null;
    // Time left until animal moves onto next object
    [SerializeField]
    float timeUntilMove;
    [SerializeField]
    float timeUntilMarkChanceLeft = 0;
    // Whether animal has began to try to night sleep
    bool nightSleepMove = false;
    #endregion

    #region functions
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Removes a manifested animal
    public void removeManifestedAnimal(AnimalPresent animal)
    {
        if(manifestedAnimals.Count == 1)
        {
            gameObject.transform.position = animal.transform.position;
        }
        manifestedAnimals.Remove(animal);
    }

    // Uses list of tags to find nearby object.
    // Automatically exculdes object already locked onto
    // If cutoff is -1, search for all objects
    // Returns null if no object with tag or in radius are found
    GameObject findNearbyObjectsWithTags(string[] tagList, float baseModifier, float cutoff)
    {
        // List of floats recording distance from current animalRaddii
        List<customMathf.SortContainer<GameObject>> magObjectList = new List<customMathf.SortContainer<GameObject>>();
        for (int i = 0; i < tagList.Length; i++)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagList[i]);
            for (int z = 0; z < objectsWithTag.Length; z++)
            {
                if(objectsWithTag[z] != lockedObject)
                {
                    Vector3 difference = objectsWithTag[z].transform.position - gameObject.transform.position;
                    float magnitude = difference.magnitude;
                    DebugDisplay.updateDisplay(objectsWithTag[z].name + " distance", magnitude + "");
                    // Does not consider object if object is outside of radius
                    if(cutoff == -1 || cutoff > magnitude)
                    {
                        magObjectList.Add(new customMathf.SortContainer<GameObject>(objectsWithTag[z], -magnitude));
                    }
                }
            }
        }
        if(magObjectList.Count == 0)
        {
            return null;
        }
        // Sorts list
        customMathf.SortContainer<GameObject>[] sortedList = magObjectList.ToArray();
        customMathf.mergeSort(sortedList);
        // Creates weights based off of list sorted
        float[] weightsList = new float[sortedList.Length];
        for(int i = 0; i < sortedList.Length; i++)
        {
            weightsList[i] = i + Mathf.Pow(baseModifier, i);
        }
        // Picks random object based on weights
        return sortedList[customMathf.weightedRandomRange(weightsList)].contained;
    }

    // Finds objects of intrest list to find nearby objects.
    // Plugs into object finding algo
    GameObject findNearbyObjectOfIntrest()
    {
        GameObject retObject = findNearbyObjectsWithTags(objectsOfIntrest, objectIntrestBase, -1);
        if(retObject == null)
        {
            print("ERROR- There are no objects that corresponds to -" + gameObject.name + "-'s objects of intrest tags");
        }
        return retObject;
    }

    // Finds where the animal should sleep
    // Plugs into object finding algo
    GameObject findSleepingSpot()
    {
        GameObject retObject = findNearbyObjectsWithTags(sleepObjects, objectSleepBase, -1);
        if (retObject == null)
        {
            print("ERROR- There are no objects that corresponds to -" + gameObject.name + "-'s sleep objects tags");
        }
        return retObject;
    }

    // Searches for nearest avaliable gameobject for creature to spawn in
    // Plugs into object finding algo
    GameObject findNearestAvaliableSpawnArea()
    {
        return findNearbyObjectsWithTags(appropiateSpawnObjects, objectSpawnBase, radiusAnimalSpawnSize); 
    }

    // Searches for nearest avaliable position on Gameobject
    Vector3 findSurface(LayerMask givenLayerMask, float yAdjust, float raycastLength)
    {
        RaycastHit hit;
        if(!Physics.Raycast(gameObject.transform.position + Vector3.up * yAdjust, Vector3.down, out hit, raycastLength, givenLayerMask))
        {
            print("ERROR- suitable surface not found");
        }
        return hit.point;
    }

    // Spawns animal at position
    // Current has not implemented crowd spawn
    void spawnAnimal()
    {
        Vector3 foundSurface;
        if (lockedPosition)
        {
            foundSurface = findSurface(spawnRaycastLayerMask, spawnRaycastYAdjust, spawnRaycastLength);
        }
        else
        {
            GameObject foundSpawn = findNearestAvaliableSpawnArea();
            if(foundSpawn == null)
            {
                foundSurface = findSurface(spawnRaycastLayerMask, spawnRaycastYAdjust, spawnRaycastLength);
            }
            else
            {
                foundSurface = foundSpawn.transform.position;
            }
        }
        print(foundSurface);
        GameObject prefab = Instantiate(animalPrefab, foundSurface, Quaternion.identity.normalized);
        AnimalPresent present = prefab.GetComponent<AnimalPresent>();
        present.init(false, 0, this);
        manifestedAnimals.Add(present);
        manifested = true;
    }


    // Finds where the animal should sleep
    GameObject spawnMarker()
    {
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // If animal has not manifested and player has not come into view
        if (!manifested)
        {
            // Handles behavior of radii based on state
            if (currentState == state.MorSleep)
            {
                // Waits until clock past waking hour
                if (universalClock.mainGameTime.greater(wakeHour, wakeMinute))
                {
                    lockedPosition = false;
                    currentState = state.Moving;
                    lockedObject = findNearbyObjectOfIntrest();
                }
            }
            else if (currentState == state.Locked)
            {
                // Waits a while at the object of intrest
                timeUntilMove -= Time.deltaTime;
                if (timeUntilMove < 0)
                {
                    lockedPosition = false;
                    currentState = state.Moving;
                    if (universalClock.mainGameTime.greater(sleepHour, sleepMinute))
                    {
                        nightSleepMove = true;
                        lockedObject = findSleepingSpot();
                    }
                    else
                    {
                        lockedObject = findNearbyObjectOfIntrest();
                    }
                }

            }
            else if (currentState == state.Moving)
            {
                // Moves towards object of intrest until a certain radius away
                Vector3 diffrence = lockedObject.transform.position - gameObject.transform.position;
                if (diffrence.magnitude <= radiiSnap)
                {
                    lockedPosition = true;
                    if (nightSleepMove)
                    {
                        currentState = state.NighSleep;
                    }
                    else
                    {
                        currentState = state.Locked;
                        timeUntilMove = timeAtIntrest;
                    }
                    gameObject.transform.position = lockedObject.transform.position;
                }
                Vector3 movementVec = Vector3.Normalize(diffrence) * radiiSpeed * Time.deltaTime;
                gameObject.transform.position += movementVec;

                // Occasionally the animal will drop markers
                timeUntilMarkChanceLeft -= Time.deltaTime;
                if (timeUntilMarkChanceLeft < 0)
                {
                    timeUntilMarkChanceLeft = markerWindowTime;
                    // Generates a number between 1-100
                    // Determines if marker gets spawned
                    float randomInt = Random.Range(0, 100);
                    if (randomInt < markerWindowChance)
                    {
                        spawnMarker();
                    }
                }
            }
            else if (currentState == state.NighSleep)
            {
            }

            // Checks if player is within radii
            Vector3 diff = transform.position - playerObject.gameObject.transform.position;
            DebugDisplay.updateDisplay("distance from player" ,diff.magnitude + "");
            if (diff.magnitude < radiusPlayerTriggerSize)
            {
                spawnAnimal();
            }
        }
        // If animal has manifested
        else
        {
            if(manifestedAnimals.Count == 0)
            {
                manifested = false;
            }
        }
    }
    #endregion
} 
