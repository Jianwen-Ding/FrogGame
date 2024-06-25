using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRadii : MonoBehaviour
{
    // This script controls the behavior of Animals when deloaded away from the players view.
    // This allows animal movements to be simplified greatly when dealing with far out of camera animals

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

    #region vars
    // Stays in constant position over the course of days
    [SerializeField]
    bool Permanent;
    [SerializeField]
    int wakeHour;
    [SerializeField]
    int wakeMinute;
    [SerializeField]
    int sleepHour;
    [SerializeField]
    int sleepMinute;
    [SerializeField]
    string[] objectsOfIntrest;
    [SerializeField]
    float radiiSpeed;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
} 
