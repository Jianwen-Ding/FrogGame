using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universalClock : MonoBehaviour
{
    // The purpose of this is to provide a universal clock for all behaviors such as wakeup or sleep to prevent
    // Desyncs in time by using Time.timeSinceLevelLoad

    // Instance of the singular clock
    static public universalClock mainClock = null;

    // The amount of days passed in the game
    public const string daySaveKey = "daySave";
    static private int day = 0;

    // The game clock time
    static public TimeRep mainGameTime;

    // The actual time from the start of the scene
    static public float realSeconds;

    // Clock that all gametime adherese to
    [SerializeField]
    float initialTime;
    // How many seconds in real time need to be spend until a minute passes in game time
    [SerializeField]
    float timePerGameMin;

    // When does the scene start
    [SerializeField]
    int startHour;
    [SerializeField]
    int startMinute;
    int startTotalMinutes;
    // Class made for representing time
    public class TimeRep
    {
        public int hours;
        public int minutes;
        public TimeRep(int hourSet, int minuteSet)
        {
            hours = hourSet;
            minutes = minuteSet;
        }

        // Set time using overall minutes
        public void setTime(int minutesSet)
        {
            hours = Mathf.FloorToInt((float)minutesSet / 60);
            minutes = minutesSet - hours * 60;
        }

        // Real seconds does not play into comparisons
        public bool greater(int hourCompare, int minuteCompare)
        {
            return timeToMinutes(hours, minutes) > timeToMinutes(hourCompare, minuteCompare);
        }
        public bool greater(int minuteCompare)
        {
            return timeToMinutes(hours, minutes) > minuteCompare;
        }

        // ToString
        public override string ToString()
        {
            if(minutes < 10)
            {
                return hours + " : 0" + minutes;
            }
            else {
                return hours + " : " + minutes;
            }

        }
    }

    // For converting from time measurements
    public static int timeToMinutes(int hours, int minutes)
    {
        return hours * 60 + minutes;
    }
    public static int timeToMinutes(TimeRep time)
    {
        return timeToMinutes(time.hours, time.minutes);
    }
    public static TimeRep minutesToTime(int minutes)
    {
        int hours = Mathf.FloorToInt((float)minutes / 60);
        int minutesLeft = minutes - hours * 60;
        return new TimeRep(hours, minutesLeft);
    }

    // Gets/Sets current day
    public static void incrementDay()
    {
        day += 1;
        PlayerPrefs.SetInt(daySaveKey, day);
    }

    public static int getDay()
    {
        return day;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Saves current day
        if(PlayerPrefs.GetInt(daySaveKey, -1) == -1)
        {
            day = 0;
            PlayerPrefs.SetInt(daySaveKey, 0);
        }
        else
        {
            day = PlayerPrefs.GetInt(daySaveKey, -1);
        }
        // Checks if there is only one debug displayer
        if (GameObject.FindObjectsOfType<universalClock>().Length > 1)
        {
            Destroy(gameObject);
        }
        mainClock = this;
        mainGameTime = new TimeRep(startHour, startMinute);
        initialTime = Time.timeSinceLevelLoad;
        startTotalMinutes = timeToMinutes(startHour, startMinute);
    }

    // Update is called once per frame
    void Update()
    {
        realSeconds = Time.timeSinceLevelLoad - initialTime;
        int minutes = Mathf.FloorToInt(realSeconds / timePerGameMin);
        mainGameTime.setTime(minutes + startTotalMinutes);
        DebugDisplay.updateDisplay("clock", mainGameTime + "");
    }
}
