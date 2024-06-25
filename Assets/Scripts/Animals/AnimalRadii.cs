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

    // >>>Object of Intrest Algo<<<
    //
    #endregion
    #region vars
    // >> CACHE PARAMETERS<<
    [SerializeField]
    GameObject animalPrefab;
    [SerializeField]
    GameObject markerPrefab;

    // >> BEHAVIOR PARAMETERS<<
    // Behaviors set to differential overall behavior between species

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
    // The amount of time the animal spends at a single point of intrest
    [SerializeField]
    float timeAtIntrest;
    // Speed that the radii moves
    [SerializeField]
    float radiiSpeed;
    // Distance until radii snaps onto the object of intrest
    [SerializeField]
    float radiiSnap;
    // Size of radius, that triggers spawn upon entering
    [SerializeField]
    float radiusSize;
    // Time between each chance that the animal will leave a marking
    [SerializeField]
    float markerWindowTime;
    // Chance out of 100 that the animal will leave a marking on a window
    [SerializeField]
    float markerWindowChance;
    // Amount of animals in radii
    [SerializeField]
    bool animalsPerRadii;

    // >> STATE VARS <<
    // Variables that change constantly to record state
    // of animal in trying to change it

    public enum state
    {
        MorSleep,
        Locked,
        Moving,
        NighSleep
    }
    state currentState = state.MorSleep;
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
    #endregion

    #region functions
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Uses objects of intrest to find nearby object.
    // Automatically exculdes object already locked onto
    GameObject findNearbyObject()
    {
        return null;
    }

    // Finds where the animal should sleep
    GameObject findSleepingSpot()
    {
        return null;
    }

    // Spawns an 
    GameObject spawnAnimal()
    {
        return null;
    }


    // Finds where the animal should sleep
    GameObject spawnMarker()
    {
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Handles behavior of radii based on state
        if (currentState == state.MorSleep)
        {
            // Waits until clock past waking hour
            if(!universalClock.mainGameTime.greater(wakeHour, wakeMinute))
            {
                currentState = state.Moving;
                lockedObject = findNearbyObject();
            }
        }
        else if (currentState == state.Locked)
        {
            // Waits a while at the object of intrest
            timeUntilMove -= Time.deltaTime;
            if(timeUntilMove < 0) {
                currentState = state.Moving;
                if (universalClock.mainGameTime.greater(sleepHour, sleepMinute))
                {
                    lockedObject = findSleepingSpot();
                }
                else
                {
                    lockedObject = findNearbyObject();
                }
            }

        }
        else if (currentState == state.Moving)
        {
            // Moves towards object of intrest until a certain radius away
            Vector3 diffrence = lockedObject.transform.position - gameObject.transform.position;
            if(diffrence.magnitude <= radiiSnap)
            {
                if (universalClock.mainGameTime.greater(sleepHour, sleepMinute))
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
            if(timeUntilMarkChanceLeft < 0)
            {
                timeUntilMarkChanceLeft = markerWindowTime;
                // Generates a number between 1-100
                // Determines if marker gets spawned
                float randomInt = Random.Range(0, 100);
                if(randomInt < markerWindowChance)
                {

                }
            }
        }
        else if (currentState == state.NighSleep)
        {

        }
    }
    #endregion
} 
