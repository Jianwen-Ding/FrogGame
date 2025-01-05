using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalRadii : MonoBehaviour
{
    // This script controls the behavior of Animals when deloaded away from the players view.
    // This allows animal movements to be simplified greatly when dealing with far out of camera animals

    #region psuedocode
    // >>>General animal behavior<<<
    // 1 - Activates past wake up time
    // 2 - Initiates algorithm to find close object of interest
    // 3 - Moves towards object of interest at set speed
    // 4 - Stay at object of interest for set time
    // 5 - Checks if the time is past sleep time
    // 6 - Goes to den if it exists, otherwise pathfinder to preferred object to sleep on.
    // 7 - Sleeps at object

    // On radius enter
    // If at object of interest or asleep, spawn animal directly at object animal is attached to
    // Otherwise, spawn animal in nearest non visible area from the center of radii (maybe in bush or ect)
    // Deactivate radii until animal is out of sight and out of a certain radius.

    // Periodically, the animal has a chance to drop a marker


    // >>>Object of Interest Algorithm<<<
    // Searches for all objects (excluding current object of interest) with tags of interest
    // Finds magnitudes of all tags of interest
    // Sorts magnitudes and randomly picks a object based on distance order

    // >>>Surface finding algorithm<<<
    // Sends raycast down from x-z coordinates from a high place.
    // Ignore certain objects.

    // >>>Spawning Animal Algorithm<<<
    // Locked in:
    // Sends raycast down at same x-z coords. Spawn animal directly on resulting location
    // Moving:
    // Searches for nearby appropriate hiding area in radius and spawns animal directly in object
    // If none can be found, send raycast down at same x-z coords. Spawns animal directly on resulting location.
    #endregion
    
    #region vars

    [Header("Identifiers Parameters")]
    // >>> Identifier Parameters <<<
    // Sets parameters or aspects of frog AI that persist between scenes

    // ID that corresponds to player prefab information that persists between scenes
    // Consists of "Animal-[ObjectName]-[Scene Name]-[Starting X axis int]-[Starting Y axis int]-[Starting Z axis int]-[Identical ID Instance]"
    // Animals with same name that spawn in same area can be mixed up by identifier system
    [SerializeField]
    string obID;
    string baseID;

    // Stores amount of marked animals present
    // Store in "[ID]-marked"
    // Can only increase
    [SerializeField]
    int markedAnimalAmount = 0;

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
    // Does animal wake up again
    [SerializeField]
    bool wakeUpAgain;
    [SerializeField]
    int rewakeHour;
    [SerializeField]
    int rewakeMinute;
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

    // Whether the animal spawns in their sleep
    [SerializeField]
    bool avoidsSpawnInSleep;
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
    // How much the animal is shifted up on spawn
    [SerializeField]
    float spawnYDisplacement;
    // Amount of animals in radii
    [SerializeField]
    int animalsPerRadii;
    // Base of chance of object spawning an animal
    [SerializeField]
    float objectSpawnBase;

    [Header("Marker Spawn Parameters")]
    // The distance the marker is displaced up on spawn
    [SerializeField]
    float markerYDisplace;
    // The degrees the marker is angled
    [SerializeField]
    float markerAngleDisplace;
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
    // How steep the angle of the surface needs to be to work for a marker
    [SerializeField]
    float markerSteepness;
    // The layer needed to mark a surface
    [SerializeField]
    int rightMarkLayer;

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
        baseID = "Animal-" + gameObject.name + "-" + gameObject.scene.name +"-"+ (int)transform.position.x + "-" + (int)transform.position.y + "-" + (int)transform.position.z;
        int identicalInstance = PlayerPrefs.GetInt(baseID + "-iden", 0);
        PlayerPrefs.SetInt(baseID + "-iden", identicalInstance + 1);
        obID = baseID + "-" + identicalInstance;

        markedAnimalAmount = PlayerPrefs.GetInt(obID + "-marked",0);
    }
    
    // Rests spawn instances
    void OnDestroy(){
        PlayerPrefs.SetInt(baseID + "-iden", 0);
    }

    // Increments the amount of marked animals
    public void markIncrement(){
        PlayerPrefs.SetInt(obID + "-marked",PlayerPrefs.GetInt(obID + "-marked") + 1);
    }

    // Clears the marks
    public void clearMark(){
        markedAnimalAmount = 0;
        PlayerPrefs.SetInt(obID + "-marked", 0);
        for(int i = 0; i < manifestedAnimals.Count; i++){
            if(manifestedAnimals != null){
                ((AnimalPresent)manifestedAnimals[i]).unmarkAnimal();
            }
        }
    }

    // Removes a manifested animal
    public void removeManifestedAnimal(AnimalPresent animal)
    {
        if(manifestedAnimals.Count == 1)
        {
            gameObject.transform.position = animal.transform.position;
        }
        if (animal.getMark())
        {
            markedAnimalAmount += 1;
        }
        manifestedAnimals.Remove(animal);
    }

    // Find sorted list of nearest objects with tag
    public customMathf.SortContainer<GameObject>[] findObjectsWithTag(string[] tagList, float baseModifier, float cutoff)
    {
        // List of floats recording distance from current animalRaddii
        List < customMathf.SortContainer < GameObject >> magObjectList = new List<customMathf.SortContainer<GameObject>>();
        for (int i = 0; i < tagList.Length; i++)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagList[i]);
            for (int z = 0; z < objectsWithTag.Length; z++)
            {
                if (objectsWithTag[z] != lockedObject)
                {
                    Vector3 difference = objectsWithTag[z].transform.position - gameObject.transform.position;
                    float magnitude = difference.magnitude;
                    // Does not consider object if object is outside of radius
                    if (cutoff == -1 || cutoff > magnitude)
                    {
                        magObjectList.Add(new customMathf.SortContainer<GameObject>(objectsWithTag[z], -magnitude));
                    }
                }
            }
        }
        if (magObjectList.Count == 0)
        {
            return null;
        }
        // Sorts list
        return magObjectList.ToArray();
    }
    // Uses list of tags to find nearby object.
    // Automatically exculdes object already locked onto
    // If cutoff is -1, search for all objects
    // Returns null if no object with tag or in radius are found
    GameObject findNearbyObjectWithTags(string[] tagList, float baseModifier, float cutoff)
    {
        // Finds sorted list
        customMathf.SortContainer<GameObject>[] sortedList = findObjectsWithTag(tagList, baseModifier, cutoff);
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
        GameObject retObject = findNearbyObjectWithTags(objectsOfIntrest, objectIntrestBase, -1);
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
        GameObject retObject = findNearbyObjectWithTags(sleepObjects, objectSleepBase, -1);
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
        return findNearbyObjectWithTags(appropiateSpawnObjects, objectSpawnBase, radiusAnimalSpawnSize); 
    }

    // Searches for nearest avaliable gameobject for creature to spawn in
    // Plugs into object finding algo
    GameObject[] findNearestSpawnAreas(int cutoff)
    {
        customMathf.SortContainer<GameObject>[] list =  findObjectsWithTag(appropiateSpawnObjects, objectSpawnBase, radiusAnimalSpawnSize);
        int totalAmount;
        if(list != null && cutoff > list.Length)
        {
            totalAmount = list.Length;
        }
        else
        {
            totalAmount = cutoff;
        }
        GameObject[] retList = new GameObject[totalAmount];
        for(int i = 0; i < retList.Length; i++)
        {
            retList[i] = list[list.Length - 1 - i].contained;
        }
        return retList;
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

    // Find steepness of surface
    float findSurfaceSteepness(LayerMask givenLayerMask, float yAdjust, float raycastLength)
    {
        RaycastHit hit;
        if (!Physics.Raycast(gameObject.transform.position + Vector3.up * yAdjust, Vector3.down, out hit, raycastLength, givenLayerMask))
        {
            print("ERROR- suitable surface not found");
        }
        //DebugDisplay.updateDisplay(" " + gameObject.name + " marker steepness", customMathf.angleBetweenTwoVecs(hit.normal, Vector3.up) + "");
        //Debug.DrawRay(hit.point, hit.normal * 5, Color.yellow, 4);
        //Debug.DrawRay(hit.point, Vector3.up * 5, Color.cyan, 4);
        return customMathf.angleBetweenTwoVecs(hit.normal, Vector3.up);
    }

    // Find steepness of surface
    int findSurfaceLayer(LayerMask givenLayerMask, float yAdjust, float raycastLength)
    {
        RaycastHit hit;
        if (!Physics.Raycast(gameObject.transform.position + Vector3.up * yAdjust, Vector3.down, out hit, raycastLength, givenLayerMask))
        {
            return -1;
        }
        //DebugDisplay.updateDisplay(" " + gameObject.name + " marker steepness", customMathf.angleBetweenTwoVecs(hit.normal, Vector3.up) + "");
        //Debug.DrawRay(hit.point, hit.normal * 5, Color.yellow, 4);
        //Debug.DrawRay(hit.point, Vector3.up * 5, Color.cyan, 4);
        
        return hit.transform.gameObject.layer;
    }
    // Spawns animal at position
    // Current has not implemented crowd spawn
    void spawnAnimal()
    {
        // If only one radii
        if(animalsPerRadii <= 1)
        {
            Vector3 foundSurface;
            float angleTowardsLocked = -1;
            if (lockedPosition)
            {
                foundSurface = findSurface(spawnRaycastLayerMask, spawnRaycastYAdjust, spawnRaycastLength) + Vector3.up * spawnYDisplacement;
            }
            else
            {
                GameObject foundSpawn = findNearestAvaliableSpawnArea();
                if (foundSpawn == null)
                {
                    foundSurface = findSurface(spawnRaycastLayerMask, spawnRaycastYAdjust, spawnRaycastLength) + Vector3.up * spawnYDisplacement;
                }
                else
                {
                    foundSurface = foundSpawn.transform.position;
                }
                Vector3 diffrence = lockedObject.transform.position - gameObject.transform.position;
                angleTowardsLocked = customMathf.pointToAngle(diffrence.x, diffrence.z);
            }
            GameObject prefab = Instantiate(animalPrefab, foundSurface, Quaternion.identity.normalized);
            AnimalPresent present = prefab.GetComponent<AnimalPresent>();
            present.init(!lockedPosition, angleTowardsLocked, this);
            manifestedAnimals.Add(present);
            manifested = true;
            if (markedAnimalAmount > 0)
            {
                present.markAnimal(false);
                markedAnimalAmount -= 1;
            }
        }
        // If multiple animals per radii
        else
        {
            int bushIter = 0;
            GameObject[] spawnAreas;
            if (lockedPosition)
            {
                spawnAreas = findNearestSpawnAreas(animalsPerRadii - 1);
            }
            else
            {
                spawnAreas = findNearestSpawnAreas(animalsPerRadii);
            }
            float angleTowardsLocked = -1;
            if (!lockedPosition)
            {
                if(lockedObject != null)
                {
                    Vector3 diffrence = lockedObject.transform.position - gameObject.transform.position;
                    angleTowardsLocked = customMathf.pointToAngle(diffrence.x, diffrence.z);
                }
                else
                {
                    angleTowardsLocked = 0;
                }
            }
            for (int i = 0; i < animalsPerRadii; i++)
            {
                Vector3 foundSurface;
                if (lockedPosition && i <= 0)
                {
                    foundSurface = findSurface(spawnRaycastLayerMask, spawnRaycastYAdjust, spawnRaycastLength) + Vector3.up * spawnYDisplacement;
                }
                else
                {
                    if (bushIter > spawnAreas.Length - 1)
                    {
                        foundSurface = findSurface(spawnRaycastLayerMask, spawnRaycastYAdjust, spawnRaycastLength) + Vector3.up * spawnYDisplacement;
                    }
                    else
                    {
                        foundSurface = spawnAreas[bushIter].transform.position;
                        bushIter = bushIter + 1;
                    }
                }
                print(foundSurface);
                GameObject prefab = Instantiate(animalPrefab, foundSurface, Quaternion.identity.normalized);
                AnimalPresent present = prefab.GetComponent<AnimalPresent>();
                present.init(!lockedPosition, angleTowardsLocked, this);
                manifestedAnimals.Add(present);
                if (markedAnimalAmount > 0)
                {
                    present.markAnimal(false);
                    markedAnimalAmount -= 1;
                }
            }
            manifested = true;
        }
    }


    // Spawns a marker
    void spawnMarker()
    {
        if(findSurfaceLayer(markerRaycastLayerMask, markerRaycastYAdjust, markerRaycastLength) == rightMarkLayer && findSurfaceSteepness(markerRaycastLayerMask, markerRaycastYAdjust, markerRaycastLength) < markerSteepness)
        {
            Vector3 foundPos = findSurface(markerRaycastLayerMask, markerRaycastYAdjust, markerRaycastLength);
            Vector3 diffrence = lockedObject.transform.position - gameObject.transform.position;
            float angleTowardsLocked = customMathf.pointToAngle(diffrence.x, diffrence.z);
            Instantiate(markerPrefab, foundPos + Vector3.up * markerYDisplace, Quaternion.Euler(0, markerAngleDisplace - angleTowardsLocked, 0));
        }
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
                    if (universalClock.mainGameTime.greater(sleepHour, sleepMinute) && (!wakeUpAgain || universalClock.mainGameTime.hours < rewakeHour))
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
                if(lockedObject != null)
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
                    // DebugDisplay.updateDisplay(" " + gameObject.name + " time until mark", timeUntilMarkChanceLeft + "");
                    timeUntilMarkChanceLeft -= Time.deltaTime;
                    if (timeUntilMarkChanceLeft < 0)
                    {
                        timeUntilMarkChanceLeft = markerWindowTime;
                        // Generates a number between 1-100
                        // Determines if marker gets spawned
                        float randomInt = Random.Range(0, 100);
                        // Display.updateDisplay(" " + gameObject.name + " mark chance pulled", randomInt + "");
                        if (randomInt < markerWindowChance)
                        {
                            spawnMarker();
                        }
                    }
                }
            }
            else if (currentState == state.NighSleep)
            {
                // Waits until clock past waking hour
                if (wakeUpAgain && universalClock.mainGameTime.greater(rewakeHour, rewakeMinute))
                {
                    lockedPosition = false;
                    nightSleepMove = false;
                    currentState = state.Moving;
                    lockedObject = findNearbyObjectOfIntrest();
                }
            }
            // Checks if player is within radii
            Vector3 diff = transform.position - playerObject.gameObject.transform.position;
            // DebugDisplay.updateDisplay("distance from player" ,diff.magnitude + "");
            if (diff.magnitude < radiusPlayerTriggerSize && (!avoidsSpawnInSleep || currentState != state.NighSleep))
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
        // DebugDisplay.updateDisplay("has manifested", manifested + "");
    }
    #endregion
} 
