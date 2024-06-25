using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRadii : MonoBehaviour
{
    // This script controls the behavior of Animals when deloaded away from the players view.
    // This allows animal movements to be simplified greatly when dealing with far out of camera animals

    #region psuedocode
    // General animal behavior
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
    // Size of radius, that triggers spawn upon entering
    [SerializeField]
    float radiusSize;
    // Time between each chance that the animal will leave a marking
    [SerializeField]
    float markerWindowTime;
    // Chance out of 100 that the animal will leave a marking on a window
    [SerializeField]
    float markerWindowChance;

    // >> STATE VARS <<
    // Variables that change constantly to record state
    // of animal in trying to change it

    // Whether animal is awake
    [SerializeField]
    bool awake;
    // Whether the animal is locked in a position
    [SerializeField]
    bool lockedPosition;
    // The object the animal is locked into
    [SerializeField]
    GameObject lockedObject;
    // Time left until animal moves onto next object
    [SerializeField]
    float timeUntilMove;
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
    #endregion
} 
